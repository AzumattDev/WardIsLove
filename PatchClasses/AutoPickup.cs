using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    public static class DisableAutoPickup
    {
        [HarmonyPatch(typeof(Player), nameof(Player.AutoPickup))]
        [HarmonyPrefix]
        private static bool Prefix(Player __instance)
        {
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position)) return true;
            WardMonoscript? pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!_wardEnabled.Value && !pa.GetAutoPickupOn())
                return true;
            return
                CustomCheck.CheckAccess(__instance.GetPlayerID(), __instance.transform.position, flash: false) ||
                pa.GetAutoPickupOn();

        }
    }
}