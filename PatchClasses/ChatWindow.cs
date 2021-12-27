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
                    ? $"<color=\"yellow\">You are running on an older version of WardIsLove ({version}). " +
                      $"A newer version has been released, Please visit the Thunderstore page to download the latest. https://valheim.thunderstore.io/package/Azumatt/WardIsLove/ </color>"
                    : $"{version} is loaded and up to date.",
                Talker.Type.Normal);
        }
    }
}