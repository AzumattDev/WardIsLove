using HarmonyLib;

namespace WardIsLove.PatchClasses;

[HarmonyPatch(typeof(Hud), nameof(Hud.Awake))]
static class HudAwakePatch
{
    static void Prefix(Hud __instance)
    {
        __instance.m_hoverName.autoSizeTextContainer = true;
        __instance.m_hoverName.textWrappingMode = TMPro.TextWrappingModes.NoWrap;
    }
}