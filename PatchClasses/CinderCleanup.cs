using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using WardIsLove.Util.Bubble;

namespace WardIsLove.PatchClasses;

[HarmonyPatch(typeof(Cinder), nameof(Cinder.FixedUpdate))]
static class CinderFixedUpdatePatch
{
    static void Postfix(Cinder __instance)
    {
        foreach (WardMonoscript wardMonoscript in WardMonoscript.m_allAreas)
        {
            if (wardMonoscript.IsInside(__instance.transform.position, wardMonoscript.GetWardRadius()))
            {
                ControlParticlesSpawner.OnProjectileHit(__instance.gameObject, wardMonoscript);
            }
        }
    }
}