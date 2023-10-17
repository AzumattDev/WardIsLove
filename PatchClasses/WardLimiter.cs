using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using BepInEx;
using fastJSON;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using WardIsLove.Extensions;
using WardIsLove.Util;
using WardIsLove.Util.RPCShit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WardIsLove
{
    public partial class WardIsLovePlugin
    {
        private static int _maxWardCount = 99999;

        private static int _wardCount;
        public static int MaxDaysDifference = 99999;
        private static WardManager _manager = null!;

        private static bool CanPlaceWard => _wardCount < _maxWardCount;
        internal static bool IsServer;
        internal static bool IsSinglePlayer;

        public enum PlayerStatus
        {
            Admin,
            VIP,
            User
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
            }

            public void IncrementWardCount(string steamId)
            {
                if (PlayersWardData.ContainsKey(steamId))
                {
                    PlayersWardData[steamId]++;
                }
                else
                {
                    PlayersWardData[steamId] = 1;
                }

                Save();
            }

            public int GetWardCount(string steamId)
            {
                return PlayersWardData.TryGetValue(steamId, out int value) ? value : 0;
            }

            public void Save()
            {
                var serializer = new SerializerBuilder().Build();

                var yaml = serializer.Serialize(PlayersWardData);

                using var writer = new StreamWriter(_path);
                writer.Write(yaml);
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

        private static void GetServerInitialData(long sender, int maxwards, int days)
        {
            _maxWardCount = maxwards;
            _wardCount = 9999;
            MaxDaysDifference = days;
        }

        private static void GetServerInfo(long server, int currentWards)
        {
            _wardCount = currentWards;
        }

        private static void GetClientInfo(long sender)
        {
            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            string steam = peer.m_socket.GetHostName();
            _manager.IncrementWardCount(steam);
            WILLogger.LogInfo($"Player (Ward Creator) {peer.m_playerName} : {steam} placed ward. Ward amount: {_manager.GetWardCount(steam)}");
            ZRoutedRpc.instance.InvokeRoutedRPC(sender, "WILLimitWard GetServerInfo", _manager.GetWardCount(steam));
        }

        [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class ZNetScene_Patch
        {
            static void Postfix()
            {
                if (IsServer)
                {
                    ZRoutedRpc.instance.Register("WILLimitWard GetClientInfo", GetClientInfo);
                }
                else
                {
                    ZRoutedRpc.instance.Register("WILLimitWard GetServerInfo", new Action<long, int>(GetServerInfo));
                    ZRoutedRpc.instance.Register("WILLimitWard GetServerInitialData", new Action<long, int, int>(GetServerInitialData));
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
                if (zdo.GetBool(ZdoInternalExtensions.WILLimitedWard))
                {
                    string steam = zdo.GetString(ZdoInternalExtensions.steamID);
                    if (_manager.PlayersWardData.ContainsKey(steam))
                    {
                        _manager.PlayersWardData[steam]--;
                        if (_manager.GetWardCount(steam) < 0) _manager.PlayersWardData[steam] = 0;
                        WILLogger.LogInfo($"Player's Ward {zdo.GetString(ZDOVars.s_creatorName)}({steam}) destroyed. Player wards count: {_manager.GetWardCount(steam)}");
                        foreach (ZNetPeer? player in ZNet.instance.m_peers)
                        {
                            if (player.m_socket.GetHostName() == steam)
                            {
                                ZRoutedRpc.instance.InvokeRoutedRPC(player.m_uid, "WILLimitWard GetServerInfo", _manager.GetWardCount(steam));
                            }
                        }
                    }

                    _manager.Save();
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece), typeof(Piece))]
        static class PlacePiece_Patch
        {
            static void WriteDataInWard(GameObject go)
            {
                if (!go.name.Contains("Thorward")) return;
                Piece piece = go.GetComponent<Piece>();
                _wardCount = 999;
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
                    ZRoutedRpc.instance.InvokeRoutedRPC(ZNet.instance.GetServerPeer().m_uid, "WILLimitWard GetClientInfo", Player.m_localPlayer.GetPlayerID());
                }
                catch
                {
                    //IGNORE FOR SINGLE PLAYER: WardIsLovePlugin.WILLogger.LogError($"The issue was found in the GetServerPeer method:  {e}");
                }
            }

            static bool Prefix(Piece piece)
            {
                if (piece.gameObject.name == "Thorward" && !CanPlaceWard)
                {
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "<color=#FF0000>Ward Limit</color>");
                    return false;
                }

                return true;
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> list = new(instructions);
                CodeInstruction[] NewInstructions =
                {
                    new(OpCodes.Ldloc_3),
                    new(OpCodes.Call,
                        AccessTools.Method(typeof(PlacePiece_Patch), nameof(WriteDataInWard),
                            new[] { typeof(GameObject) }))
                };
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].opcode == OpCodes.Stloc_3)
                    {
                        list.InsertRange(i + 1, NewInstructions);
                    }
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
                string steam = peer.m_socket.GetHostName();
                int wardCount = GetPlayerStatus(steam) switch
                {
                    PlayerStatus.Admin => 99999,
                    PlayerStatus.VIP => _maxWardCountVipConfig.Value,
                    PlayerStatus.User => _maxWardCountConfig.Value,
                    _ => _maxWardCountConfig.Value
                };
                ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "WILLimitWard GetServerInitialData", wardCount, MaxDaysDifferenceConfig.Value);
                if (_manager.PlayersWardData.ContainsKey(steam))
                {
                    WILLogger.LogInfo($"Sending info to {peer.m_playerName}({steam}) about his wards Wards count: {_manager.GetWardCount(steam)}");
                    ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "WILLimitWard GetServerInfo", _manager.GetWardCount(steam));
                }
                else
                {
                    WILLogger.LogDebug("Sending info to player about his wards (not existing in database)");
                    ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "WILLimitWard GetServerInfo", 0);
                }
            }
        }
    }
}