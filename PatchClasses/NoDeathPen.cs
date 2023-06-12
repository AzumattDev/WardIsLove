using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{

    [HarmonyPatch(typeof(Skills), nameof(Skills.OnDeath))]
    static class Skills_OnDeath_Patch
    {
        static bool Prefix(Skills __instance)
        {
            Vector3 position = Player.m_localPlayer.transform.position;
            if (!WardMonoscript.CheckInWardMonoscript(position) || !CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), position, 1f,
                    false) || !WardNoDeathPen.Value || !WardEnabled.Value) return true;
            WardMonoscript ward = WardMonoscriptExt.GetWardMonoscript(position);
            if (!ward.GetNoDeathPenOn()) return true;
            return false;
        }
    }
}