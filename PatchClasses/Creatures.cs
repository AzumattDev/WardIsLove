using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
    static class Character_WILRPC_Damage_Patch
    {
        static bool Prefix(Character __instance, ref HitData hit, ZNetView ___m_nview)
        {
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || ___m_nview == null ||
                !_wardEnabled.Value) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            hit.ApplyModifier((float)(1.0 + pa.GetCreatureDamageIncrease() / 100.0));
            return true;
        }
    }
}