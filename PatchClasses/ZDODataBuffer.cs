using System.Collections.Generic;
using HarmonyLib;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.OnNewConnection))]
    internal class StartBufferingOnNewConnection
    {
        internal static readonly List<ZPackage> PackageBuffer = new();

        private static void Postfix(ZNet __instance, ZNetPeer peer)
        {
            if (!__instance.IsServer())
            {
                peer.m_rpc.Register<ZPackage>("ZDOData", (_, package) => PackageBuffer.Add(package));
            }
        }
    }

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Shutdown))]
    internal class ClearPackageBufferOnShutdown
    {
        private static void Postfix() => StartBufferingOnNewConnection.PackageBuffer.Clear();
    }

    [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.AddPeer))]
    internal class EvaluateBufferedPackages
    {
        private static void Postfix(ZDOMan __instance, ZNetPeer netPeer)
        {
            foreach (ZPackage package in StartBufferingOnNewConnection.PackageBuffer)
            {
                __instance.RPC_ZDOData(netPeer.m_rpc, package);
            }

            StartBufferingOnNewConnection.PackageBuffer.Clear();
        }
    }
}