using System.Linq;
using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    internal class StructureWearNTear
    {
        // Alter damage to structure inside of ward
        [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.ApplyDamage))]
        public static class StructureDamage
        {
            // Makes only certain items indestructible.
            private static bool Prefix(ref WearNTear __instance, ref float damage)
            {
                bool shouldDamage = true;
                if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || !WardEnabled.Value)
                    return shouldDamage;
                WardMonoscript paa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (paa.GetRaidProtectionOn())
                {
                    // Can't check for player here, so the default is just false to not show the message, but should still calculate the damage.
                    if (!OfflineStatus.CheckOfflineStatus(paa))
                    {
                        shouldDamage = false;
                    }

                    //return shouldDamage;
                }

                if (paa.GetStructDamageReduc() > 0)
                {
                    damage *= (float)(1.0 - paa.GetStructDamageReduc() / 100.0);
                    //return shouldDamage;
                }

                if (!paa.GetIndestructibleOn()) return shouldDamage;
                string[] array = paa.GetIndestructList().ToLower().Trim().Split(',').ToArray();
                if (!array.Any()) return shouldDamage;
                foreach (string item in array)
                    if (__instance.m_nview.GetPrefabName().ToLower().Contains(item))
                    {
                        shouldDamage = false;
                        return shouldDamage;
                    }

                return shouldDamage;
            }
        }

        [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.RPC_Damage))]
        [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.Damage))]
        public static class WearNTear_Reduction
        {
            // reduces damage to all things. Stuctures, ships, beds etc.
            private static bool Prefix(WearNTear __instance, ref HitData hit, ZNetView ___m_nview)
            {
                if (WardMonoscript.CheckInWardMonoscript(__instance.transform.position) && ___m_nview != null &&
                    WardEnabled.Value)
                {
                    WardMonoscript paa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                    // I guess checking for the attacker being a player or it being null might have been a good idea.
                    if (paa.GetRaidProtectionOn() && hit.GetAttacker() != null)
                    {
                        if (!OfflineStatus.CheckOfflineStatus(paa, hit.GetAttacker().IsPlayer()))
                        {
                            hit.ApplyModifier(0);
                            return false;
                        }
                    }
                }

                if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || ___m_nview == null ||
                    !WardEnabled.Value) return true;
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                hit.ApplyModifier((float)(1.0 - pa.GetStructDamageReduc() / 100.0));
                return true;
            }
        }


        [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.HaveRoof))]
        public static class WILDisableWeatherDamagePatch
        {
            private static void Postfix(WearNTear __instance, ref bool __result)
            {
                if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position)) return;
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (!WardEnabled.Value || !pa.GetWeatherDmgOn() || __instance.m_nview.GetZDO() == null ||
                    !__instance.m_nview.IsOwner()) return;
                __result = true;
            }
        }
    }
}