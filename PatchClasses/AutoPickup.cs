using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch(typeof(Player), nameof(Player.AutoPickup))]
    public static class DisableAutoPickup
    {
        private static bool Prefix(Player __instance)
        {
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!WardEnabled.Value && !pa.GetAutoPickupOn())
                return true;
            return
                CustomCheck.CheckAccess(__instance.GetPlayerID(), __instance.transform.position, flash: false) ||
                pa.GetAutoPickupOn();
        }
    }
}