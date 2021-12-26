using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    public static class NoDeathPen
    {
        [HarmonyPatch(typeof(Skills), nameof(Skills.Awake))]
        [HarmonyPostfix]
        private static void Postfix(ref Skills __instance)
        {
            if (!Player.m_localPlayer || !_wardEnabled.Value)
                return;
            Vector3 pos = Player.m_localPlayer.transform.position;
            if (!WardMonoscript.CheckInWardMonoscript(pos)) return;
            WardMonoscript? ward = WardMonoscriptExt.GetWardMonoscript(pos);
            if (ward.GetNoDeathPenOn())
                __instance.m_DeathLowerFactor = 0f;
        }
    }

    [HarmonyPatch]
    public static class OnDeath
    {
        [HarmonyPatch(typeof(Skills), nameof(Skills.OnDeath))]
        [HarmonyPrefix]
        private static bool Prefix(Skills __instance)
        {
            Vector3 position = Player.m_localPlayer.transform.position;
            return !WardMonoscript.CheckInWardMonoscript(position) || !CustomCheck.CheckAccess(
                Player.m_localPlayer.GetPlayerID(), position, 1f,
                false) || !_wardNoDeathPen.Value || !_wardEnabled.Value;
        }
    }
}