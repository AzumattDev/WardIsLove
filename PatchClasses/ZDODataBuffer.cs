using System.Collections.Generic;
using HarmonyLib;

namespace WardIsLove.PatchClasses
{
    public static class ZdoDataBuffer
    {
        private static readonly List<ZPackage> PackageBuffer = new();

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.OnNewConnection))]
        private class StartBufferingOnNewConnection
        {
            private static void Postfix(ZNet __instance, ZNetPeer peer)
            {
                if (!__instance.IsServer())
                {
                    peer.m_rpc.Register<ZPackage>("ZDOData", (_, package) => PackageBuffer.Add(package));
                }
            }
        }

        [HarmonyPatch(typeof(ZNet), nameof(ZNet.Shutdown))]
        private class ClearPackageBufferOnShutdown
        {
            private static void Postfix() => PackageBuffer.Clear();
        }

        [HarmonyPatch(typeof(ZDOMan), nameof(ZDOMan.AddPeer))]
        private class EvaluateBufferedPackages
        {
            private static void Postfix(ZDOMan __instance, ZNetPeer netPeer)
            {
                foreach (ZPackage package in PackageBuffer)
                {
                    __instance.RPC_ZDOData(netPeer.m_rpc, package);
                }

                PackageBuffer.Clear();
            }
        }
    }
}