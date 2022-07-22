using System.Linq;
using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    public static class FireplaceUnlimited
    {
        [HarmonyPatch(typeof(Fireplace), nameof(Fireplace.UpdateFireplace))]
        [HarmonyPostfix]
        private static void Postfix(Fireplace __instance, ZNetView ___m_nview)
        {
            if (WardMonoscript.CheckInWardMonoscript(__instance.transform.position))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (!WardEnabled.Value || !pa.GetFireplaceUnlimOn() || ___m_nview.GetZDO() == null ||
                    !___m_nview.IsOwner()) return;
                string[] array = pa.GetFireplaceList().ToLower().Trim().Split(',').ToArray();
                foreach (string item in array)
                    if (__instance.gameObject.name.Contains(item))
                        ___m_nview.GetZDO().Set("fuel", __instance.m_maxFuel);
            }
        }

        [HarmonyPatch(typeof(Smelter), nameof(Smelter.UpdateSmelter))]
        [HarmonyPostfix]
        private static void Postfix(Smelter __instance, ZNetView ___m_nview)
        {
            if (WardMonoscript.CheckInWardMonoscript(__instance.transform.position))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (!WardEnabled.Value || !pa.GetBathingUnlimOn() ||
                    ___m_nview.GetZDO() == null ||
                    !___m_nview.IsOwner()) return;
                if (__instance.m_name.ToLower().Contains("tub"))
                    __instance.SetFuel(__instance.m_maxFuel);
            }
        }

        [HarmonyPatch(typeof(CookingStation), nameof(CookingStation.UpdateCooking))]
        [HarmonyPostfix]
        private static void Postfix(CookingStation __instance, ZNetView ___m_nview)
        {
            if (WardMonoscript.CheckInWardMonoscript(__instance.transform.position))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (!WardEnabled.Value || !pa.GetCookingUnlimOn() ||
                    ___m_nview.GetZDO() == null ||
                    !___m_nview.IsOwner()) return;
                if (__instance.m_name.ToLower().Contains("oven"))
                    __instance.SetFuel(__instance.m_maxFuel);
            }
        }
    }
}