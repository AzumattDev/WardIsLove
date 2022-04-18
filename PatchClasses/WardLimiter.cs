using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using BepInEx;
using fastJSON;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using WardIsLove.Util;

namespace WardIsLove
{
    public partial class WardIsLovePlugin
    {
        private static int MaxWardCount = 99999;

        private static int WardCount;
        public static int MaxDaysDifference = 99999;
        private static WardManager _manager;

        private static bool CanPlaceWard => WardCount < MaxWardCount;
        private static bool IsServer;

        public enum PlayerStatus
        {
            Admin,
            VIP,
            User
        }


        private void WardLimitServerCheck()
        {
            IsServer = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
            if (IsServer)
            {
                _manager = new WardManager(Path.Combine(Paths.ConfigPath, "wardIsLoveData"));
                MaxWardCountConfig = Config.Bind("General", "Max Wards Per Player", 3);
                MaxWardCountVIPConfig = Config.Bind("General", "Max Wards Per Player (VIP)", 5);
                MaxDaysDifferenceConfig = Config.Bind("General", "Days For Deactivate", 300);
                VIPplayersListConfig = Config.Bind("General", "VIP players list", "steam ids");
            }
        }

        static PlayerStatus GetPlayerStatus(string steam)
        {
            if (ZNet.instance.m_adminList.Contains(steam)) return PlayerStatus.Admin;
            if (VIPplayersListConfig.Value.Contains(steam)) return PlayerStatus.VIP;
            return PlayerStatus.User;
        }

        private class WardManager
        {
            private string _path;
            public Dictionary<string, int> PlayersWardData = new();

            public WardManager(string path)
            {
                _path = path;
                if (!File.Exists(_path))
                {
                    File.Create(_path).Dispose();
                }
                else
                {
                    string data = File.ReadAllText(_path);
                    if (!string.IsNullOrEmpty(data))
                        PlayersWardData = JSON.ToObject<Dictionary<string, int>>(data);
                }
            }

            public void Save()
            {
                File.WriteAllText(_path, JSON.ToJSON(PlayersWardData));
            }
        }

        private static void GetServerInitialData(long sender, int maxwards, int days)
        {
            MaxWardCount = maxwards;
            WardCount = 9999;
            MaxDaysDifference = days;
        }

        private static void GetServerInfo(long server, int currentWards)
        {
            WardCount = currentWards;
        }

        private static void GetClientInfo(long sender)
        {
            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            string steam = peer.m_socket.GetHostName();
            if (_manager.PlayersWardData.ContainsKey(steam))
            {
                _manager.PlayersWardData[steam]++;
            }
            else
            {
                _manager.PlayersWardData[steam] = 1;
            }

            WILLogger.LogInfo(
                $"Player (Ward Creator) {peer.m_playerName} : {steam} placed ward. Ward amount: {_manager.PlayersWardData[steam]}");
            _manager.Save();
            ZRoutedRpc.instance.InvokeRoutedRPC(sender, "WILLimitWard GetServerInfo",
                _manager.PlayersWardData[steam]);
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
                    ZRoutedRpc.instance.Register("WILLimitWard GetServerInitialData",
                        new Action<long, int, int>(GetServerInitialData));
                }
            }
        }

        [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.HandleDestroyedZDO))]
        static class ZDOMan_Patch
        {
            private static readonly int CreatorHash = "steamID".GetStableHashCode();
            private static readonly int PlayerIDHash = "playerID".GetStableHashCode();

            static void Prefix(ZDOMan __instance, ZDOID uid)
            {
                if (!IsServer) return;
                ZDO zdo = __instance.GetZDO(uid);
                if (zdo == null) return;
                if (zdo.GetBool("WILLimitedWard"))
                {
                    string steam = zdo.GetString(CreatorHash);
                    if (_manager.PlayersWardData.ContainsKey(steam))
                    {
                        _manager.PlayersWardData[steam]--;
                        if (_manager.PlayersWardData[steam] < 0) _manager.PlayersWardData[steam] = 0;
                        WILLogger.LogDebug(
                            $"Player's Ward {steam} destroyed. Player wards count: {_manager.PlayersWardData[steam]}");
                        foreach (var player in ZNet.instance.m_peers)
                        {
                            if (player.m_socket.GetHostName() == steam)
                            {
                                ZRoutedRpc.instance.InvokeRoutedRPC(player.m_uid, "WILLimitWard GetServerInfo",
                                    _manager.PlayersWardData[steam]);
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
                WardCount = 999;
                piece.m_nview.m_zdo.Set("WILLimitedWard", true);
                if (Admin)
                {
                    piece.m_nview.m_zdo.Set("WILLimitedWardTime", EnvMan.instance.GetCurrentDay() + 50000);
                }
                else
                {
                    piece.m_nview.m_zdo.Set("WILLimitedWardTime", EnvMan.instance.GetCurrentDay());
                }

                go.GetComponent<WardMonoscript>().SetEnabled(true);
                try
                {
                    ZRoutedRpc.instance.InvokeRoutedRPC(ZNet.instance.GetServerPeer().m_uid,
                        "WILLimitWard GetClientInfo",
                        Player.m_localPlayer.GetPlayerID());
                }
                catch (Exception e)
                {
                    //IGNORE FOR SINGLE PLAYER: WardIsLovePlugin.WILLogger.LogError($"The issue was found in the GetServerPeer method:  {e}");
                }
            }

            static bool Prefix(Piece piece)
            {
                if (piece.gameObject.name == "Thorward" && !CanPlaceWard)
                {
                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center,
                        "<color=red>Ward Limit</color>");
                    return false;
                }

                return true;
            }

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> list = new(instructions);
                CodeInstruction[] NewInstructions = {
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

        /*[HarmonyPatch(typeof(ZNet), "RPC_CharacterID")]
        private static class ZnetSync2
        {
            private static void Postfix(ZRpc rpc, ZDOID characterID)
            {
                if (!(ZNet.instance.IsServer() && ZNet.instance.IsDedicated())) return;
                print($"Player {rpc.m_socket.GetHostName()} Connected");
                ZNetPeer peer = ZNet.instance.GetPeer(rpc);
                Task.Run(async () =>
                {
                    await Task.Delay(6000);
                    if (peer == null || !peer.IsReady()) return;
                    ZDO zdo = ZDOMan.instance.GetZDO(peer.m_characterID);
                    if (zdo != null)
                    {
                        print($"ZDO is fine");
                        long data = zdo.GetLong("playerID");
                        if (_manager.PlayersWardData.ContainsKey(data))
                        {
                            print(
                                $"Sending info to player about his wards (exist in database) Wards count: {_manager.PlayersWardData[data]}");
                            ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "WILLimitWard GetServerInfo",
                                _manager.PlayersWardData[data]);
                        }
                        else
                        {
                            print("Sending info to player about his wards (not existing in database)");
                            ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "WILLimitWard GetServerInfo", 0);
                        }
                    }
                });
            }
        }*/

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
                    PlayerStatus.VIP => MaxWardCountVIPConfig.Value,
                    PlayerStatus.User => MaxWardCountConfig.Value,
                    _ => MaxWardCountConfig.Value
                };
                ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "WILLimitWard GetServerInitialData", wardCount,
                    MaxDaysDifferenceConfig.Value);
                if (_manager.PlayersWardData.ContainsKey(steam))
                {
                    WILLogger.LogDebug(
                        $"Sending info to player about his wards (exist in database) Wards count: {_manager.PlayersWardData[steam]}");
                    ZRoutedRpc.instance.InvokeRoutedRPC(peer.m_uid, "WILLimitWard GetServerInfo",
                        _manager.PlayersWardData[steam]);
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