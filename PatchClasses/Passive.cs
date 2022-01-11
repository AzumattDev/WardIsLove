using System.Collections;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.Util;

namespace WardIsLove.PatchClasses
{
    /// Passive Heal inside ward
    public static class PlayerHealthUpdatePatch
    {
        public static IEnumerator DelayedHeal(Player p, WardMonoscript ward)
        {
            while (true)
            {
                if (WardIsLovePlugin._wardEnabled != null && ZNetScene.instance && WardIsLovePlugin._wardEnabled.Value)
                {
                    if (p == null) yield break;
                    if (!Player.m_localPlayer) yield break;
                    if (ward.IsPermitted(p.GetPlayerID()))
                        p.Heal(ward.GetHealthBoost());
                }

                yield return new WaitForSecondsRealtime(2);
            }
        }
    }

    /// Passive Stamina inside ward
    public static class PlayerStaminaUpdatePatch
    {
        public static IEnumerator DelayedStaminaRegen(Player p, WardMonoscript ward)
        {
            while (true)
            {
                if (WardIsLovePlugin._wardEnabled != null && ZNetScene.instance && WardIsLovePlugin._wardEnabled.Value)
                {
                    if (p == null) yield break;
                    if (!Player.m_localPlayer) yield break;
                    if (ward.IsPermitted(p.GetPlayerID()))
                        p.AddStamina(ward.GetStaminaBoost());
                }

                yield return new WaitForSecondsRealtime(2);
            }
        }
    }
}