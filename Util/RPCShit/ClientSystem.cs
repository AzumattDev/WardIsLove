using System;
using HarmonyLib;

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

    [HarmonyPatch(typeof(Game), nameof(Game.Logout))]
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Disconnect))]
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.RPC_Error))]
    internal static class ClientResetPatches
    {
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

    [HarmonyPatch]
    public class ClientRPC_Registrations
    {
        [HarmonyPatch(typeof(Game), nameof(Game.Start))]
        [HarmonyPrefix]
        public static void Start_Prefix()
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
            //ZRoutedRpc.instance.Register<ZPackage>("EventCostConfigSync", new Action<long, ZPackage>(ClientSystem.RPC_EventCostConfigSync));
            //ZRoutedRpc.instance.Register<ZPackage>("RequestCostConfigSync", new Action<long, ZPackage>(ClientSystem.RPC_RequestCostConfigSync));
        }


        [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
        [HarmonyPrefix]
        public static void Awake_Postfix()
        {
            if (ZRoutedRpc.instance == null || !ZNetScene.instance)
                return;
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "RequestSync", new ZPackage());
            if (!WardIsLovePlugin.Admin)
                ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "RequestAdminSync",
                    new ZPackage());
            //ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.instance.GetServerPeerID(), "RequestCostConfigSync", (object)new ZPackage());
        }
        
    }
}