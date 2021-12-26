using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    /// Add health to player when inside ward
    /*[HarmonyPatch]
    public static class Health
    {
        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        [HarmonyPostfix]
        public static void Postfix(Player __instance, ref float hp)
        {
            // Terrible try catch...but it's here to prevent error for now
            try
            {
                if (!Player.m_localPlayer) return;
                foreach (WardMonoscript? ward in WardMonoscriptExt.WardMonoscriptsINSIDE)
                    if (ward.IsPermitted(__instance.GetPlayerID()) && _wardEnabled.Value)
                        if (ward && ward != null && ward.IsEnabled())
                            hp += ward.GetHealthBoost();
            }
            catch
            {
                // ignored
            }
        }
    }*/


    /// Passive Heal inside ward
    [HarmonyPatch]
    public static class Heal
    {
        private static int HealTick;

        [HarmonyPatch(typeof(Player), nameof(Player.Update))]
        public static void FixedUpdate(Player __instance)
        {
            if (!Player.m_localPlayer) return;
            Player p = Player.m_localPlayer;
            if (p.InPlaceMode() || !__instance)
                return;
            if (HealTick > 0) HealTick--;

            if (WardMonoscript.CheckInWardMonoscript(p.transform.position) &&
                CustomCheck.CheckAccess(p.GetPlayerID(), p.transform.position, 1f, false))
                if (HealTick <= 0)
                {
                    HealTick = 50;
                    p.Heal(1500f);
                }
        }
    }
}