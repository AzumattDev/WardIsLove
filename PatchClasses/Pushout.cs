using System.Collections;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.Util;

namespace WardIsLove.PatchClasses
{

    /// Pushout Players
    [HarmonyPatch]
    public static class PushoutNonPermitted
    {
        public static IEnumerator PushoutPlayer(Player p, WardMonoscript wardEntered)
        {
            while (true)
            {
                if (WardIsLovePlugin._wardEnabled != null && ZNetScene.instance && WardIsLovePlugin._wardEnabled.Value)
                {
                    try
                    {
                        if (WardMonoscriptExt.WardMonoscriptsINSIDE == null) yield break;
                        foreach (WardMonoscript? ward in WardMonoscriptExt.WardMonoscriptsINSIDE)
                        {
                            if (!ward.IsEnabled()) continue;
                            if (!ward.GetPushoutPlayersOn() ||
                                ward.m_piece.GetCreator() == Game.instance.GetPlayerProfile().GetPlayerID() ||
                                ward.IsPermitted(Game.instance.GetPlayerProfile().GetPlayerID()) ||
                                !ward.m_bubble.activeSelf) continue;
                            Transform transform = p.transform;
                            Vector3 position = transform.position;
                            Vector3 dir = (position - ward.transform.position).normalized;
                            position += dir * 3.15f;
                            transform.position = position;
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

                yield return new WaitForSecondsRealtime(0.5f);
            }
        }


        // Pushout Creatures
        public static IEnumerator PushoutCreature(Character c, WardMonoscript wardEntered)
        {
            while (true)
            {
                if (WardIsLovePlugin._wardEnabled != null && ZNetScene.instance && WardIsLovePlugin._wardEnabled.Value)
                {
                    try
                    {
                        if (!c || c.IsDead()) yield break;
                        if (!Player.m_localPlayer) yield break;
                        if (WardMonoscriptExt.WardCharacterINSIDE == null) yield break;

                        foreach (WardMonoscript? ward in WardMonoscriptExt.WardCharacterINSIDE)
                        {
                            if (!ward.IsEnabled()) continue;
                            if (c.IsPlayer() || c.IsTamed()) continue;
                            if (!WardMonoscript.CheckInWardMonoscript(c.transform.position)) continue;
                            if (!ward.GetPushoutCreaturesOn() || !ward.m_bubble.activeSelf) continue;
                            Vector3 position = c.transform.position;
                            Vector3 dir = (position - ward.transform.position).normalized;
                            position += dir * 3.15f;
                            c.transform.position = position;
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
    }
}