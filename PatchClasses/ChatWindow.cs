using HarmonyLib;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    /// Add WardIsLove intro to chat window.
    [HarmonyPatch]
    public static class ChatPatchAwake
    {
        [HarmonyPatch(typeof(Chat), nameof(Chat.Awake))]
        [HarmonyPostfix]
        private static void Postfix(ref Chat __instance)
        {
            Chat.m_instance.AddString("[WardIsLove]",
                !IsUpToDate
                    ? $"<color=\"yellow\">You are running on an older version of WardIsLoves ({version}). A newer version has been released, see https://www.nexusmods.com/valheim/mods/402</color>"
                    : $"{version} is loaded and up to date.",
                Talker.Type.Normal);
        }
    }
}