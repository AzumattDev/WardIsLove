using System;
using HarmonyLib;
using WardIsLove.Util.UI;

namespace WardIsLove.Util.RPCShit
{
    [HarmonyPatch]
    public class ClientSystem
    {
        /// <summary>
        ///     All requests, put in order to the corresponding events. These are "sent" to the server
        /// </summary>
        public static void RPC_RequestAdminSync(long sender, ZPackage pkg)
        {
        }

        public static void RPC_RequestTestConnection(long sender, ZPackage pkg)
        {
        }

        public static void RPC_RequestGuild(long sender, ZPackage pkg)
        {
        }

        public static void RPC_BadRequestMsg(long sender, ZPackage pkg)
        {
            if (sender != ZRoutedRpc.instance.GetServerPeerID() || pkg == null || pkg.Size() <= 0)
                return;
            string str = pkg.ReadString();
            if (str == "")
                return;
            Chat.m_instance.AddString("Server", "<color=\"red\">" + str + "</color>", Talker.Type.Normal);
        }
        
        public static void RPC_RequestDropdownPlayers(long sender, ZPackage pkg)
        {
        }

        /// <summary>
        ///     All events, put in order to the corresponding requests. These are "received" from the server
        ///     put logic here that you want to happen on the client AFTER getting the information from the server.
        /// </summary>
        public static void RPC_EventTestConnection(long sender, ZPackage pkg)
        {
            WardIsLovePlugin.WILLogger.LogDebug("Server has WardIsLove installed");
            WardIsLovePlugin.ValidServer = true;
        }


        public static void RPC_EventAdminSync(long sender, ZPackage pkg)
        {
            WardIsLovePlugin.WILLogger.LogInfo("This account is an admin.");
            Chat.m_instance.AddString("[WardIsLove]", "<color=\"green\">" + "Admin permissions synced" + "</color>",
                Talker.Type.Normal);
            WardIsLovePlugin.Admin = true;
        }

        public static void RPC_EventRequestGuild(long sender, ZPackage pkg)
        {
        }
        
        public static void RPC_EventDropdownPlayers(long sender, ZPackage pkg)
        {
            /* Populate External list, then populate dropdown in DropdownPopulate.cs "PopulatePlayerList()" */
            DropdownPopulate.External_list.Clear();
            int num = pkg.ReadInt();
            long playerID;
            for (int index = 0; index < num; ++index)
            {
                ZNet.PlayerInfo playerInfo = new()
                {
                    m_name = pkg.ReadString(),
                    m_host = pkg.ReadString(),
                    m_characterID = pkg.ReadZDOID()
                };
                playerID = pkg.ReadLong();
                playerInfo.m_publicPosition = pkg.ReadBool();
                if (playerInfo.m_publicPosition)
                    playerInfo.m_position = pkg.ReadVector3();
                
                if (playerInfo.m_name != "Human")
                {
                    DropdownPopulate.External_list.Add(index,
                        new DropdownData
                        {
                            //id = ZDOMan.instance.GetZDO(playerInfo.m_characterID).GetLong("playerID"),
                            id = playerID,
                            name = playerInfo.m_name
                        });
                }
                else
                {
                    if (!ZNet.instance.IsServer() || ZNet.instance.IsDedicated()) continue;
                    DropdownPopulate.External_list.Add(index,
                        new DropdownData
                        {
                            //id = ZDOMan.instance.GetZDO(playerInfo.m_characterID).GetLong("playerID"),
                            id = playerID,
                            name = Player.m_localPlayer.GetPlayerName()
                        });
                }
                
                WardIsLovePlugin.WILLogger.LogDebug($"Dropdown data from server:\nName:{playerInfo.m_name}\nCharacterID:{playerInfo.m_characterID}\nHost:{playerInfo.m_host}\nPosition:{playerInfo.m_position}\nPlayerID:{playerID}");
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
        public static class PlayerOnSpawnPatch
        {
            private static void Prefix()
            {
                if (!ZNet.instance.IsDedicated() && ZNet.instance.IsServer())
                {
                    WardIsLovePlugin.Admin = true;

                    WardIsLovePlugin.WILLogger.LogInfo($"Local Play Detected setting Admin: {WardIsLovePlugin.Admin}");
                }

                if (ZRoutedRpc.instance == null || !ZNetScene.instance)
                    return;
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "RequestSync",
                    new ZPackage());
            }
        }
    }

    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Start))]
    internal static class FejdStartup_Start_Patch
    {
        private static void Postfix()
        {
            if (!WardIsLovePlugin._wardEnabled.Value)
                return;
            Console.SetConsoleEnabled(true);
        }
    }

    [HarmonyPatch]
    internal static class ClientResetPatches
    {
        [HarmonyPatch(typeof(Game), nameof(Game.Logout))]
        [HarmonyPatch(typeof(ZNet), nameof(ZNet.Disconnect))]
        [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_Error))]
        private static void Prefix()
        {
            if (!WardIsLovePlugin._wardEnabled.Value)
                return;
            if (WardIsLovePlugin.Admin)
            {
                WardIsLovePlugin.Admin = false;
                WardIsLovePlugin.WILLogger.LogDebug($"Admin Status changed to: {WardIsLovePlugin.Admin}");
            }
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    static class ClientRPC_Registrations
    {
        static void Prefix(Game __instance)
        {
            if (ZNet.m_isServer) return;
            ZRoutedRpc.instance.Register("RequestTestConnection",
                new Action<long, ZPackage>(ClientSystem.RPC_RequestTestConnection));
            ZRoutedRpc.instance.Register("EventTestConnection",
                new Action<long, ZPackage>(ClientSystem.RPC_EventTestConnection));
            ZRoutedRpc.instance.Register("RequestAdminSync",
                new Action<long, ZPackage>(ClientSystem.RPC_RequestAdminSync));
            ZRoutedRpc.instance.Register("EventAdminSync",
                new Action<long, ZPackage>(ClientSystem.RPC_EventAdminSync));
            ZRoutedRpc.instance.Register("BadRequestMsg",
                new Action<long, ZPackage>(ClientSystem.RPC_BadRequestMsg));
            
            /* Dropdown list fix */
            ZRoutedRpc.instance.Register("DropdownListRequest",
                new Action<long, ZPackage>(ClientSystem.RPC_RequestDropdownPlayers));
            ZRoutedRpc.instance.Register("DropdownListEvent",
                new Action<long, ZPackage>(ClientSystem.RPC_EventDropdownPlayers));
        }
    }
}