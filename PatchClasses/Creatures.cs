using HarmonyLib;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    public static class CreatureDmg_Increase
    {
        [HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
        [HarmonyPrefix]
        private static bool Prefix(Character __instance, ref HitData hit, ZNetView ___m_nview)
        {
            /* TODO
             Set this to work with the GUI
             */
            if (___m_nview == null || !WardMonoscript.CheckInWardMonoscript(__instance.transform.position) ||
                __instance.IsPlayer()) return true;
            hit.m_damage.m_blunt *= (float)(1.0 + _wardDamageIncrease.Value / 100.0);
            hit.m_damage.m_slash *= (float)(1.0 + _wardDamageIncrease.Value / 100.0);
            hit.m_damage.m_pierce *= (float)(1.0 + _wardDamageIncrease.Value / 100.0);
            hit.m_damage.m_chop *= (float)(1.0 + _wardDamageIncrease.Value / 100.0);
            hit.m_damage.m_pickaxe *= (float)(1.0 + _wardDamageIncrease.Value / 100.0);
            hit.m_damage.m_fire *= (float)(1.0 + _wardDamageIncrease.Value / 100.0);
            hit.m_damage.m_frost *= (float)(1.0 + _wardDamageIncrease.Value / 100.0);
            hit.m_damage.m_lightning *= (float)(1.0 + _wardDamageIncrease.Value / 100.0);
            hit.m_damage.m_poison *= (float)(1.0 + _wardDamageIncrease.Value / 100.0);
            hit.m_damage.m_spirit *= (float)(1.0 + _wardDamageIncrease.Value / 100.0);
            

            return true;
        }
    }
}