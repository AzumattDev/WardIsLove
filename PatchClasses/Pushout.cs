using Guilds;
using HarmonyLib;

namespace WardIsLove.PatchClasses
{
    /// Push away player when inside ward
    [HarmonyPatch]
    public class Pushout
    {

        /*[HarmonyPatch(typeof(Player), nameof(Player.Update))]
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            try
            {
                if (WardMonoscriptExt.WardMonoscriptsINSIDE == null) return;
                foreach (WardMonoscript? ward in WardMonoscriptExt.WardMonoscriptsINSIDE)
                {
                    if (!ward.IsEnabled()) continue;
                    if (!ward.GetPushoutPlayersOn() ||
                        ward.m_piece.GetCreator() == Game.instance.GetPlayerProfile().GetPlayerID() ||
                        ward.IsPermitted(Game.instance.GetPlayerProfile().GetPlayerID())) continue;
                    Vector3 position = __instance.transform.position;
                    Vector3 dir = (position - ward.transform.position).normalized;
                    position += dir * 0.15f;
                    __instance.transform.position = position;
                }
            }
            catch
            {
                // ignored
            }
        }*/

        /* Use character here instead of Humanoid to also pushout deer */
        /*[HarmonyPatch(typeof(Character), nameof(Character.FixedUpdate))]
        [HarmonyPostfix]
        public static void Postfix(Character __instance)
        {
            try
            {
                if (!__instance || __instance.IsDead()) return;
                if (!Player.m_localPlayer) return;
                if (WardMonoscriptExt.WardCharacterINSIDE == null) return;

                foreach (WardMonoscript? ward in WardMonoscriptExt.WardCharacterINSIDE)
                {
                    if (!ward.IsEnabled()) continue;
                    if (__instance.IsPlayer() || __instance.IsTamed()) continue;
                    if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position)) continue;
                    if (!ward.GetPushoutCreaturesOn()) continue;
                    Vector3 position = __instance.transform.position;
                    Vector3 dir = (position - ward.transform.position).normalized;
                    position += dir * 0.15f;
                    __instance.transform.position = position;
                }
            }
            catch
            {
                // ignored
            }
        }*/
    }
}