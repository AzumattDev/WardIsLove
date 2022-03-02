using HarmonyLib;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    /// Add WardIsLove intro to chat window.
    [HarmonyPatch(typeof(Chat), nameof(Chat.Awake))]
    public static class ChatPatchAwake
    {
        [HarmonyPostfix]
        private static void Postfix_ChatAwake(ref Chat __instance)
        {
            __instance.AddString("[WardIsLove]",
                !IsUpToDate
                    ? $"<color=\"yellow\">You are running on an older version of WardIsLove ({version}). " +
                      "A newer version has been released, Please visit the Thunderstore page to download the latest. https://valheim.thunderstore.io/package/Azumatt/WardIsLove/ </color>"
                    : $"{version} is loaded and up to date.",
                Talker.Type.Normal);
        }
    }
}