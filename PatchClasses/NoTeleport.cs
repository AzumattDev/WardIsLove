using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.Teleport))]
    static class TeleportWorldTeleportPatch
    {
        private static bool Prefix(TeleportWorld __instance, ref Player player)
        {
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position)) return true;
            if (!player) return true;
            bool canTeleportThisMofo = false;
            foreach (WardMonoscript? ward in WardMonoscriptExt.WardMonoscriptsINSIDE)
                try
                {
                    if (!ward.GetNoTeleportOn() || !WardEnabled.Value) return true;
                    if (!CustomCheck.CheckAccess(Player.m_localPlayer.GetPlayerID(),
                            Player.m_localPlayer.transform.position, flash: false))
                    {
                        canTeleportThisMofo = false;
                        player.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                        return false;
                    }

                    if (CustomCheck.CheckAccess(Player.m_localPlayer.GetPlayerID(),
                            Player.m_localPlayer.transform.position, flash: false))
                    {
                        canTeleportThisMofo = true;
                    }
                }
                catch
                {
                    // ignored
                }

            return canTeleportThisMofo;
        }
    }
}