using HarmonyLib;
using JetBrains.Annotations;
using WardIsLove.Extensions;
using WardIsLove.Util;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch(typeof(Player), nameof(Player.UpdateFood))]
    static class Player_NoFoodDrain_Patch
    {
        [UsedImplicitly]
        static void Prefix(Player __instance, float dt, bool forceUpdate)
        {
            if (WardMonoscriptExt.WardMonoscriptsINSIDE == null) return;
            if (Player.m_localPlayer == null) return;

            foreach (WardMonoscript? ward in WardMonoscriptExt.WardMonoscriptsINSIDE)
            {
                if (!ward.GetNoFoodDrainOn() ||
                    !WardMonoscript.CheckInWardMonoscript(__instance.transform.position) ||
                    !CustomCheck.CheckAccess(Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                        flash: false)) continue;
                if (!(__instance.m_foodUpdateTimer + dt >= 1) && !forceUpdate) continue;
                foreach (Player.Food food in __instance.m_foods) ++food.m_time;
                break;  // Added break to prevent food drain timer constant increase for each ward you are inside of.
            }
        }
    }
}