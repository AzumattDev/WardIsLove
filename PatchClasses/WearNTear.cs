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
        // Since I am using a custom monoscript for wards...vanilla doesn't pick up on the creator for some reason. Patch it.
        [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.OnPlaced))]
        [HarmonyPostfix]
        private static void WardCreatorNameUpdate(WearNTear __instance)
        {
            Piece? pieceComp = __instance.GetComponent<Piece>();
            long creatorLong = pieceComp.GetCreator();
            WardMonoscript ward;
            try
            {
                ward = pieceComp.gameObject.GetComponent<WardMonoscript>();
                if (ward) ward.Setup(Player.GetPlayer(creatorLong).GetPlayerName());
            }
            catch
            {
                // ignored
            }
        }

        // Alter damage to structure inside of ward
        [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.ApplyDamage))]
        public static class StructureDamage
        {
            // Makes only certain items indestructible.
            private static bool Prefix(ref WearNTear __instance)
            {
                bool shouldDamage = true;
                if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || !_wardEnabled.Value)
                    return shouldDamage;
                WardMonoscript? paa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (!OfflineStatus.CheckOfflineStatus(paa) && paa.GetRaidProtectionOn())
                {
                    shouldDamage = false;
                    return shouldDamage;
                }

                if (!paa.GetIndestructibleOn()) return shouldDamage;
                string[] array = paa.GetIndestructList().ToLower().Trim().Split(',').ToArray();
                if (!array.Any()) return shouldDamage;
                foreach (string item in array)
                    if (__instance.m_piece.m_name.Contains(item))
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
                    _wardEnabled.Value)
                {
                    WardMonoscript? paa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                    if (!OfflineStatus.CheckOfflineStatus(paa) && paa.GetRaidProtectionOn())
                    {
                        hit.ApplyModifier(0);
                        return false;
                    }
                }

                if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || ___m_nview == null ||
                    !_wardEnabled.Value) return true;
                WardMonoscript? pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                hit.ApplyModifier((float)(1.0 - pa.GetStructDamageReduc() / 100.0));
                return true;
            }
        }


        [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.HaveRoof))]
        public static class DisableWeatherDamagePatch
        {
            private static void Postfix(WearNTear __instance, ref bool __result)
            {
                /*if (!_wardEnabled.Value || !_noWeatherDmg.Value)
                    return;
                __result = false;*/
                if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position)) return;
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (!_wardEnabled.Value || !pa.GetWeatherDmgOn() || __instance.m_nview.GetZDO() == null ||
                    !__instance.m_nview.IsOwner()) return;
                __result = false;
            }
        }
    }
}