using HarmonyLib;
using UnityEngine;

namespace WardIsLove.PatchClasses;

[HarmonyPatch(typeof(ZNetScene),nameof(ZNetScene.Awake))]
static class ZNetScene_Awake_Patch
{
    static void Postfix(ZNetScene __instance)
    {
        if(__instance.GetPrefab("Thorward") != null)
            __instance.GetPrefab("Thorward").GetComponent<Piece>().m_placeEffect = __instance.GetPrefab("guard_stone").GetComponent<Piece>().m_placeEffect;

        if (__instance.GetPrefab("wardlightningActivation") != null)
        {
            var destinationGO = Utils.FindChild(__instance.GetPrefab("wardlightningActivation").transform, "poff_ring");
            var sourceGO = Utils.FindChild(__instance.GetPrefab("lightningAOE").transform, "poff_ring");
            var sourceRenderMat = Object.Instantiate(sourceGO.GetComponent<ParticleSystemRenderer>().material);
            destinationGO.GetComponent<ParticleSystemRenderer>().material = sourceRenderMat;
            
        }
    }
}   