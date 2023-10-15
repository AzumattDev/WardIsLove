using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch(typeof(Door), nameof(Door.Interact))]
    static class AutomaticDoorClose
    {
        private static readonly Dictionary<int, Coroutine> coroutineClose = new();

        private static void Postfix(ref Door __instance, ZNetView ___m_nview)
        {
            if (!WardEnabled.Value) return;
            if (WardMonoscriptExt.WardMonoscriptsINSIDE == null) return;
            foreach (WardMonoscript? ward in WardMonoscriptExt.WardMonoscriptsINSIDE)
            {
                if (ward.GetAutoCloseDoorsOn())
                {
                    if (coroutineClose.ContainsKey(___m_nview.GetHashCode()))
                        ___m_nview.StopCoroutine(coroutineClose[___m_nview.GetHashCode()]);
                    Coroutine coroutine = ___m_nview.StartCoroutine(AutoClose(__instance, ___m_nview));
                    coroutineClose[___m_nview.GetHashCode()] = coroutine;
                }

                if (WardIsLovePlugin.AutoClose.Value)
                {
                    if (coroutineClose.ContainsKey(___m_nview.GetHashCode()))
                        ___m_nview.StopCoroutine(coroutineClose[___m_nview.GetHashCode()]);
                    Coroutine coroutine = ___m_nview.StartCoroutine(AutoClose(__instance, ___m_nview));
                    coroutineClose[___m_nview.GetHashCode()] = coroutine;
                }
            }
        }


        private static IEnumerator AutoClose(Door __instance, ZNetView ___m_nview)
        {
            _ = coroutineClose.Remove(___m_nview.GetHashCode());
            yield return new WaitForSeconds(5);
            ___m_nview.GetZDO().Set("state", 0);
            _ = coroutineClose.Remove(___m_nview.GetHashCode());
        }
    }
}