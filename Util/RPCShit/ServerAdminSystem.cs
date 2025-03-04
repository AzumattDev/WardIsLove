using System;
using HarmonyLib;

namespace WardIsLove.Util.RPCShit
{
    [HarmonyPatch]
    public class ServerAdminSystem
    {
        /// <summary>
        ///     All requests, put in order to the corresponding events. These are "sent" to the server
        /// </summary>
        public static void RPC_RequestAdminSync(long sender, ZPackage pkg)
        {
            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            if (peer != null)
            {
                string str = peer.m_rpc.GetSocket().GetHostName();
                if (!ZNet.instance.IsDedicated() && ZNet.instance.IsServer())
                {
                    WardIsLovePlugin.Admin = true;
                    WardIsLovePlugin.WILLogger.LogDebug($"Local Play Detected setting Admin:{WardIsLovePlugin.Admin}");
                }

                //string str = ((ZSteamSocket)peer.m_socket).GetPeerID().m_SteamID.ToString();
                if (ZNet.instance.m_adminList == null || !ZNet.instance.ListContainsId(ZNet.instance.m_adminList, str))
                    return;
                WardIsLovePlugin.WILLogger.LogInfo($"Admin Detected: {str}");
                ZRoutedRpc.instance.InvokeRoutedRPC(sender, "WILEventAdminSync", pkg);
            }
            else
            {
                ZPackage zpackage = new();
                zpackage.Write("You aren't an Admin!");
                ZRoutedRpc.instance.InvokeRoutedRPC(sender, "WILBadRequestMsg", zpackage);
            }
        }

        public static void RPC_RequestSync(long sender, ZPackage pkg)
        {
            if (ZNet.instance.GetPeer(sender) != null)
            {
                ZPackage zpackage1 = new();
                string data = EnvMan.instance.m_debugTime.ToString();
                zpackage1.Write(data);
                ZPackage zpackage2 = new();
                string debugEnv = EnvMan.instance.m_debugEnv;
                zpackage2.Write(debugEnv);
                WardIsLovePlugin.WILLogger.LogDebug("Syncing with clients...");
                ZRoutedRpc.instance.InvokeRoutedRPC(sender, "WILEventTestConnection", new ZPackage());
            }
            else
            {
                ZPackage zpackage = new();
                zpackage.Write("Peer doesn't exist");
                ZRoutedRpc.instance.InvokeRoutedRPC(sender, "WILBadRequestMsg", zpackage);
            }
        }

        public static void RPC_RequestGuild(long sender, ZPackage pkg)
        {
            if (ZNet.instance.GetPeer(sender) != null)
            {
                ZPackage zpackage1 = new();
                string data = EnvMan.instance.m_debugTime.ToString();
                zpackage1.Write(data);
                ZPackage zpackage2 = new();
                string debugEnv = EnvMan.instance.m_debugEnv;
                zpackage2.Write(debugEnv);
                WardIsLovePlugin.WILLogger.LogInfo("Syncing with clients...");
                ZRoutedRpc.instance.InvokeRoutedRPC(sender, "WILEventTestConnection", new ZPackage());
            }
            else
            {
                ZPackage zpackage = new();
                zpackage.Write("Peer doesn't exist");
                ZRoutedRpc.instance.InvokeRoutedRPC(sender, "WILBadRequestMsg", zpackage);
            }
        }

        public static void RPC_RequestWardCount(long sender, ZPackage pkg)
        {
            
        }

        public static void RPC_RequestDropdownPlayers(long sender, ZPackage pkg)
        {
            if (ZNet.instance.m_peers.Count <= 0)
                return;
            ZPackage zpackage = new();
            zpackage.Write(ZNet.instance.m_players.Count);
            foreach (ZNet.PlayerInfo player in ZNet.instance.m_players)
            {
                zpackage.Write(player.m_name);
                zpackage.Write(player.m_userInfo.m_id.ToString());
                zpackage.Write(player.m_characterID);
                zpackage.Write(ZDOMan.instance.GetZDO(player.m_characterID).GetLong(ZDOVars.s_playerID));
                zpackage.Write(player.m_publicPosition);
                if (player.m_publicPosition)
                    zpackage.Write(player.m_position);

                WardIsLovePlugin.WILLogger.LogDebug(
                    $"Server Data being sent to Dropdown list:\nName:{player.m_name}\nCharacterID:{player.m_characterID}\nHost:{player.m_userInfo.m_id.ToString()}\nPosition:{player.m_position}\nPlayerID:{ZDOMan.instance.GetZDO(player.m_characterID).GetLong(ZDOVars.s_playerID)}");
            }

            ZRoutedRpc.instance.InvokeRoutedRPC(sender, "WILDropdownListEvent", zpackage);
        }

        /// <summary>
        ///     All events, put in order to the corresponding requests. These are "received" from the server
        ///     put logic here that you want to happen on the client AFTER getting the information from the server.
        /// </summary>
        public static void RPC_EventAdminSync(long sender, ZPackage pkg)
        {
        }

        public static void RPC_EventSync(long sender, ZPackage pkg)
        {
        }

        public static void RPC_EventRequestGuild(long sender, ZPackage pkg)
        {
        }

        public static void RPC_EventDropdownPlayers(long sender, ZPackage pkg)
        {
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    static class GameStartPatch
    {
        static void Prefix(Game __instance)
        {
            if (!ZNet.m_isServer) return;
            ZRoutedRpc.instance.Register("RequestSync", new Action<long, ZPackage>(ServerAdminSystem.RPC_RequestSync));
            ZRoutedRpc.instance.Register("EventSync", new Action<long, ZPackage>(ServerAdminSystem.RPC_EventSync));
            ZRoutedRpc.instance.Register("WILRequestAdminSync", new Action<long, ZPackage>(ServerAdminSystem.RPC_RequestAdminSync));
            ZRoutedRpc.instance.Register("WILEventAdminSync", new Action<long, ZPackage>(ServerAdminSystem.RPC_EventAdminSync));

            /* Dropdown list fix */
            ZRoutedRpc.instance.Register("WILDropdownListRequest", new Action<long, ZPackage>(ServerAdminSystem.RPC_RequestDropdownPlayers));
            ZRoutedRpc.instance.Register("WILDropdownListEvent", new Action<long, ZPackage>(ServerAdminSystem.RPC_EventDropdownPlayers));
        }
    }
}