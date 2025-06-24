using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using BepInEx;
using fastJSON;
using HarmonyLib;
using Splatform;
using UnityEngine;
using UnityEngine.Rendering;
using WardIsLove.Extensions;
using WardIsLove.Util;
using WardIsLove.Util.RPCShit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WardIsLove;

public partial class WardIsLovePlugin
{
    internal static bool IsServer;
    internal static bool IsSinglePlayer;
    public static int MaxDaysDifference = 99999;
    private static WardManager _manager = null!;

    public enum PlayerStatus
    {
        Admin,
        VIP,
        User
    }

    // Helper method to safely extract just the user ID part (without platform prefix)
    private static string SafeExtractUserID(string platformUserIdString)
    {
        if (string.IsNullOrEmpty(platformUserIdString))
            return string.Empty;

        // Try to parse as PlatformUserID first
        if (PlatformUserID.TryParse(platformUserIdString, out PlatformUserID platformUserId))
        {
            return platformUserId.m_userID ?? string.Empty;
        }

        // If parsing fails, check if it's already just a user ID (no platform prefix)
        if (!platformUserIdString.Contains('_'))
        {
            // Already a clean user ID
            return platformUserIdString;
        }

        // If it contains underscore but failed to parse, try to extract the part after the first underscore
        int underscoreIndex = platformUserIdString.IndexOf('_');
        if (underscoreIndex >= 0 && underscoreIndex < platformUserIdString.Length - 1)
        {
            return platformUserIdString.Substring(underscoreIndex + 1);
        }

        // Last resort: return the original string
        WILLogger.LogWarning($"Failed to extract user ID from: {platformUserIdString}");
        return platformUserIdString;
    }

    // Helper method to safely get normalized platform user ID string with Steam_ prefix fallback
    private static string SafeGetNormalizedUserID(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // If it already has a platform prefix, try to parse it
        if (input.Contains('_'))
        {
            if (PlatformUserID.TryParse(input, out PlatformUserID platformUserId))
            {
                // Return the full platform_userID format
                return platformUserId.ToString();
            }

            // If parsing failed but has underscore, assume it's malformed but usable
            return input;
        }

        // If no platform prefix found, assume it's a Steam ID and add Steam_ prefix
        return $"Steam_{input}";
    }


    private IEnumerator WardCountUpdateCoroutine()
    {
        while (true)
        {
            UpdateWardCounts();
            yield return new WaitForSeconds(60); // Wait for 60 seconds before next update
        }
    }

    public static ZDO[] GetZDOs(int hash)
    {
        return ZDOMan.instance.m_objectsByID.Values.Where(zdo => hash == zdo.m_prefab).ToArray();
    }

    private void UpdateWardCounts()
    {
        if (ZDOMan.instance == null) return;
        int wardPrefabHash = "Thorward".GetStableHashCode();

        ZDO[] wardZDOs = GetZDOs(wardPrefabHash);

        var playerWardCounts = new Dictionary<string, int>();

        foreach (var zdo in wardZDOs)
        {
            string rawSteamId = zdo.GetString(ZdoInternalExtensions.steamID);
            string steamId = SafeGetNormalizedUserID(rawSteamId);
            if (!string.IsNullOrEmpty(steamId))
            {
                if (!playerWardCounts.ContainsKey(steamId))
                {
                    playerWardCounts[steamId] = 0;
                }

                playerWardCounts[steamId]++;
            }
        }

        var playerPeers = new Dictionary<string, ZNetPeer>();
        foreach (ZNetPeer? player in ZNet.instance.m_peers)
        {
            string hostName = player.m_socket.GetHostName();
            string userId = SafeGetNormalizedUserID(hostName);
            if (!string.IsNullOrEmpty(userId))
            {
                playerPeers[userId] = player;
            }
        }

        // Update the file with the new counts and sync to clients
        foreach (var kvp in playerWardCounts)
        {
            _manager.PlayersWardData[kvp.Key] = kvp.Value;
            if (playerPeers.ContainsKey(kvp.Key))
            {
                bool canPlace = _manager.CanPlaceWard(kvp.Key);
                ZRoutedRpc.instance.InvokeRoutedRPC(playerPeers[kvp.Key].m_uid, "WILLimitWard UpdatePermission", canPlace);
            }
        }


        _manager.Save();
    }


    internal static void WardLimitServerCheck()
    {
        IsServer = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        if (ZNet.instance != null)
            IsSinglePlayer = ZNet.instance.IsServer() && !ZNet.instance.IsDedicated();
        if (IsServer || (ZNet.instance != null && IsSinglePlayer))
        {
            _manager = new WardManager(Path.Combine(Paths.ConfigPath, "Azumatt.WardIsLoveData.yml"));
            _maxWardCountConfig = Instance.Config.Bind("General", "Max Wards Per Player", 3);
            _maxWardCountVipConfig = Instance.Config.Bind("General", "Max Wards Per Player (VIP)", 5);
            MaxDaysDifferenceConfig = Instance.Config.Bind("General", "Days For Deactivate", 30);
            ViPplayersListConfig = Instance.Config.Bind("General", "VIP players list", "steam ids");
        }
    }

    static PlayerStatus GetPlayerStatus(string steam)
    {
        if (ZNet.instance.ListContainsId(ZNet.instance.m_adminList, steam)) return PlayerStatus.Admin;
        if (ViPplayersListConfig.Value.Contains(steam)) return PlayerStatus.VIP;
        return PlayerStatus.User;
    }

    private class WardManager
    {
        private string _path;
        public Dictionary<string, int> PlayersWardData = new();

        public WardManager(string path)
        {
            _path = path;
            ReadWardData:
            WILLogger.LogInfo($"Initializing WardManager with path: {_path}");
            if (!File.Exists(_path))
            {
                File.Create(_path).Dispose();
                if (File.Exists(_path.Replace("Azumatt.WardIsLoveData.yml", "wardIsLoveData")))
                {
                    ConvertJsonToYamlAndDelete(_path.Replace("Azumatt.WardIsLoveData.yml", "wardIsLoveData"));
                    goto ReadWardData;
                }
            }
            else if (File.Exists(_path))
            {
                string data = File.ReadAllText(_path);
                if (!string.IsNullOrEmpty(data))
                {
                    var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
                    PlayersWardData = deserializer.Deserialize<Dictionary<string, int>>(data);
                }
            }
            else
            {
                string data = File.ReadAllText(_path);
                if (!string.IsNullOrEmpty(data))
                    PlayersWardData = JSON.ToObject<Dictionary<string, int>>(data);
            }

            WILLogger.LogInfo($"Ward data loaded. Total entries: {PlayersWardData.Count}");
        }

        public bool CanPlaceWard(string steamId)
        {
            string normalizedId = SafeGetNormalizedUserID(steamId);
            string userIdOnly = SafeExtractUserID(steamId);
            int currentCount = GetWardCount(normalizedId);

            return GetPlayerStatus(userIdOnly) switch
            {
                PlayerStatus.Admin => true,
                PlayerStatus.VIP => currentCount < _maxWardCountVipConfig.Value,
                PlayerStatus.User => currentCount < _maxWardCountConfig.Value,
                _ => false
            };
        }

        public void IncrementWardCount(string steamId)
        {
            string normalizedId = SafeGetNormalizedUserID(steamId);
            WILLogger.LogInfo($"Incrementing ward count for normalized ID: {normalizedId}");

            if (PlayersWardData.ContainsKey(normalizedId))
            {
                PlayersWardData[normalizedId]++;
            }
            else
            {
                PlayersWardData[normalizedId] = 1;
            }

            Save();
            WILLogger.LogInfo($"New ward count for ID {normalizedId}: {PlayersWardData[normalizedId]}");
        }

        public void DecrementWardCount(string steamId)
        {
            string normalizedId = SafeGetNormalizedUserID(steamId);

            if (PlayersWardData.ContainsKey(normalizedId))
            {
                PlayersWardData[normalizedId]--;
                if (PlayersWardData[normalizedId] < 0)
                    PlayersWardData[normalizedId] = 0;

                Save();
                WILLogger.LogInfo($"Decremented ward count for ID {normalizedId}: {PlayersWardData[normalizedId]}");
            }
        }

        public int GetWardCount(string steamId)
        {
            string normalizedId = SafeGetNormalizedUserID(steamId);
            return PlayersWardData.TryGetValue(normalizedId, out int value) ? value : 0;
        }

        public void Save()
        {
            WILLogger.LogDebug("Saving ward data to file.");
            var serializer = new SerializerBuilder().Build();

            var yaml = serializer.Serialize(PlayersWardData);

            using var writer = new StreamWriter(_path);
            writer.Write(yaml);
            WILLogger.LogDebug("Ward data saved successfully.");
        }

        public static void ConvertJsonToYamlAndDelete(string jsonFilePath)
        {
            // Read the JSON data from the file
            string jsonData = File.ReadAllText(jsonFilePath);

            if (string.IsNullOrEmpty(jsonData)) return;

            // Deserialize the JSON data into a dictionary
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            var data = deserializer.Deserialize<Dictionary<string, int>>(jsonData);

            // Serialize the dictionary to YAML
            var serializer = new SerializerBuilder().DisableAliases().Build();

            string yamlData = serializer.Serialize(data);

            // Write the YAML data to a new file
            string yamlFilePath = Path.Combine(Path.GetDirectoryName(jsonFilePath), "Azumatt.WardIsLoveData.yml");

            using (var writer = new StreamWriter(yamlFilePath))
            {
                writer.Write(yamlData);
            }

            // Delete the original JSON file
            File.Delete(jsonFilePath);
        }
    }

    // Client-side variables
    private static bool _canPlaceWard = true;

    private static void UpdatePermission(long server, bool canPlace)
    {
        _canPlaceWard = canPlace;
    }

    private static void WardPlaced(long sender)
    {
        WILLogger.LogInfo($"WardPlaced called by sender: {sender}");
        ZNetPeer peer = ZNet.instance.GetPeer(sender);
        if (peer == null) return;

        string hostName = peer.m_socket.GetHostName();
        string steam = SafeExtractUserID(hostName);
        _manager.IncrementWardCount(steam);
        WILLogger.LogInfo($"Player (Ward Creator) {peer.m_playerName} : {steam} placed ward. Ward amount: {_manager.GetWardCount(steam)}");

        bool canPlace = _manager.CanPlaceWard(steam);
        ZRoutedRpc.instance.InvokeRoutedRPC(sender, "WILLimitWard UpdatePermission", canPlace);
    }

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    static class ZNetScene_Patch
    {
        static void Postfix()
        {
            if (IsServer)
            {
                ZRoutedRpc.instance.Register("WILLimitWard WardPlaced", WardPlaced);
            }
            else
            {
                ZRoutedRpc.instance.Register("WILLimitWard UpdatePermission", new Action<long, bool>(UpdatePermission));
            }
        }
    }

    [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.HandleDestroyedZDO))]
    static class ZDOMan_Patch
    {
        static void Prefix(ZDOMan __instance, ZDOID uid)
        {
            if (!IsServer) return;
            ZDO zdo = __instance.GetZDO(uid);
            if (zdo == null) return;
            if (zdo.GetBool(ZdoInternalExtensions.WILLimitedWard) && zdo.m_prefab == "Thorward".GetStableHashCode())
            {
                string rawSteamId = zdo.GetString(ZdoInternalExtensions.steamID);
                string steam = SafeExtractUserID(rawSteamId);
                if (!string.IsNullOrEmpty(steam))
                {
                    _manager.DecrementWardCount(steam);
                    WILLogger.LogInfo($"Player's Ward {zdo.GetString(ZDOVars.s_creatorName)}({steam}) destroyed. Player wards count: {_manager.GetWardCount(steam)}");
                    foreach (ZNetPeer? player in ZNet.instance.m_peers)
                    {
                        if (player.m_socket.GetHostName().Contains(steam))
                        {
                            bool canPlace = _manager.CanPlaceWard(steam);
                            ZRoutedRpc.instance.InvokeRoutedRPC(player.m_uid, "WILLimitWard UpdatePermission", canPlace);
                            break;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
    static class PlacePiece_Patch
    {
        static void WriteDataInWard(GameObject go)
        {
            if (go == null) return;
            if (!go.name.Contains("Thorward")) return;
            
            Piece piece = go.GetComponent<Piece>();
            piece.m_nview.m_zdo.Set(ZdoInternalExtensions.WILLimitedWard, true);
            
            if (Admin)
            {
                piece.m_nview.m_zdo.Set(ZdoInternalExtensions.WILLimitedWardTime, -1);
            }
            else
            {
                ServerTimeRPCs.RequestServerTimeIfNeeded();
                piece.m_nview.m_zdo.Set(ZdoInternalExtensions.WILLimitedWardTime, (int)serverDateTimeOffset.ToUnixTimeSeconds());
            }

            go.GetComponent<WardMonoscript>().SetEnabled(true);
            
            try
            {
                string rawUserId = PlatformManager.DistributionPlatform.LocalUser.PlatformUserID.ToString();
                string normalizedId = SafeGetNormalizedUserID(rawUserId);
                piece.m_nview.m_zdo.Set(ZdoInternalExtensions.steamID, normalizedId);
                
                ZRoutedRpc.instance.InvokeRoutedRPC(ZNet.instance.GetServerPeer().m_uid, "WILLimitWard WardPlaced", Player.m_localPlayer.GetPlayerID());
            }
            catch
            {
                //IGNORE FOR SINGLE PLAYER: WardIsLovePlugin.WILLogger.LogError($"The issue was found in the GetServerPeer method:  {e}");
            }
        }

        static bool Prefix(Piece piece)
        {
            if (piece.gameObject.name == "Thorward" && !_canPlaceWard)
            {
                // This runs on client side, so PlatformManager is available
                string rawUserId = PlatformManager.DistributionPlatform.LocalUser.PlatformUserID.ToString();
                string userId = SafeExtractUserID(rawUserId);

                int maxWards = GetPlayerStatus(userId) switch
                {
                    PlayerStatus.Admin => 99999,
                    PlayerStatus.VIP => _maxWardCountVipConfig?.Value ?? 5,
                    PlayerStatus.User => _maxWardCountConfig?.Value ?? 3,
                    _ => 3
                };

                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, $"<color=#FF0000>Ward Limit Reached</color>\nMax: {maxWards}");
                return false;
            }

            return true;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> list = new(instructions);
            int insertionIndex = -1;

            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].opcode == OpCodes.Stloc_3)
                {
                    insertionIndex = i + 1;
                    break;
                }
            }

            if (insertionIndex >= 0)
            {
                // Prepare the instructions to insert
                CodeInstruction[] NewInstructions =
                [
                    new CodeInstruction(OpCodes.Ldloc_0), // load `gameObject2`
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlacePiece_Patch), nameof(WriteDataInWard)))
                ];

                list.InsertRange(insertionIndex, NewInstructions);
            }
            else
            {
                // Log an error if the injection point wasn't found
                WardIsLovePlugin.WILLogger.LogError("Failed to find injection point for WriteDataInWard");
            }

            return list;
        }
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_PeerInfo))]
    private static class ZnetSync1
    {
        private static void Postfix(ZRpc rpc)
        {
            if (!(ZNet.instance.IsServer() && ZNet.instance.IsDedicated())) return;

            ZNetPeer peer = ZNet.instance.GetPeer(rpc);
            if (peer == null) return;

            string hostName = peer.m_socket.GetHostName();
            string steam = SafeExtractUserID(hostName);
            bool canPlace = _manager.CanPlaceWard(steam);

            ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "WILLimitWard UpdatePermission", canPlace);

            WILLogger.LogInfo($"Sending ward permission to {peer.m_playerName}({steam}): Can place = {canPlace}, Current wards: {_manager.GetWardCount(steam)}");
        }
    }
}