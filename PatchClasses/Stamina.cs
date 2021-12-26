using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    /// Add stamina to player when inside ward
    [HarmonyPatch]
    public static class Stamina
    {
        [HarmonyPatch(typeof(Player), nameof(Player.GetTotalFoodValue))]
        [HarmonyPostfix]
        public static void Postfix(Player __instance, ref float stamina)
        {
            // Terrible try catch...but it's here to prevent error for now
            try
            {
                foreach (WardMonoscript? ward in WardMonoscriptExt.WardMonoscriptsINSIDE)
                    if (ward.IsPermitted(__instance.GetPlayerID()) && _wardEnabled.Value)
                        if (ward && ward != null && ward.IsEnabled())
                            stamina += ward.GetStaminaBoost();
            }
            catch
            {
                // ignored
            }
        }
    }
}