using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    /* All of this code below is...shit, but it works. */

    [HarmonyPatch]
    public static class OfflineRaid
    {
        [HarmonyPatch(typeof(Player), nameof(Player.Update))]
        [HarmonyPostfix]
        private static void Postfix(Player __instance)
        {
            if (!ZNetScene.instance) return;
            if (!_wardEnabled.Value || !(bool)(Object)Player.m_localPlayer ||
                !_raidProtection.Value || messageHappened)
                return;
            if (WardMonoscript.CheckInWardMonoscript(Player.m_localPlayer.transform.position) &&
                !CustomCheck.CheckAccess(Player.m_localPlayer.GetPlayerID(), Player.m_localPlayer.transform.position,
                    flash: false) && _raidProtection.Value)
            {
                int j = 0;
                List<GameObject> gameObjectList = new();
                foreach (Collider component in Physics.OverlapSphere(Player.m_localPlayer.transform.position, 100))
                {
                    WardMonoscript componentInParent = component.GetComponentInParent<WardMonoscript>();
                    bool flag = true;
                    if (!_wardEnabled.Value && componentInParent)
                        flag =
                            componentInParent.m_piece.GetCreator() == Game.instance.GetPlayerProfile().GetPlayerID() ||
                            componentInParent.IsPermitted(Game.instance.GetPlayerProfile().GetPlayerID());

                    if (((!(bool)componentInParent ? 0 :
                            Vector3.Distance(Player.m_localPlayer.transform.position,
                                componentInParent.transform.position) <= (double)100 ? 1 : 0) & (flag ? 1 : 0)) ==
                        0) continue;
                    if (gameObjectList.Contains(componentInParent.gameObject)) continue;
                    gameObjectList.Add(componentInParent.gameObject);
                    List<KeyValuePair<long, string>> permittedList = componentInParent.GetPermittedPlayers();
                    List<string> stringList = ZNet.instance.GetPlayerList().Select(player => player.m_name).ToList();
                    if (permittedList.Count == 0 && stringList.Contains(componentInParent.GetCreatorName()) &&
                        !componentInParent.IsPermitted(Game.instance.GetPlayerProfile().GetPlayerID()))
                    {
                        j++;
                        if (j < _raidablePlayersNeeded.Value)
                        {
                            Raidable = false;
                            //Player.m_localPlayer.Message(MessageHud.MessageType.Center, "<color=\"red\">Not-Raidable\nOwner: " + componentInParent.GetCreatorName() + "\nPermitted Players: " + string.Join("\n", name.Value) + "</color>", 0, (Sprite)null);
                            Chat.m_instance.AddString("[WardIsLove]", "<color=\"red\">Not-Raidable</color>",
                                Talker.Type.Normal);
                            Chat.m_instance.AddString("[WardIsLove]",
                                "<color=\"red\">Only the Owner: " + componentInParent.GetCreatorName() +
                                " is online</color>", Talker.Type.Normal);
                            messageHappened = true;
                            return;
                        }

                        Raidable = true;
                        // Player.m_localPlayer.Message(MessageHud.MessageType.Center, "<color=\"green\">Raidable</color>", 0, null);
                        Chat.m_instance.AddString("[WardIsLove]", "<color=\"green\">Raidable</color>",
                            Talker.Type.Normal);
                        messageHappened = true;
                        return;
                    }

                    if (WardMonoscript.CheckInWardMonoscript(Player.m_localPlayer.transform.position) &&
                        !CustomCheck.CheckAccess(Player.m_localPlayer.GetPlayerID(),
                            Player.m_localPlayer.transform.position, flash: false))
                    {
                        Raidable = false;

                        // Player.m_localPlayer.Message(MessageHud.MessageType.Center, "<color=\"red\">Not enough players on this ward are online\nAll structures inside the ward are indestructible</color>", 0, null);
                        Chat.m_instance.AddString("[WardIsLove]",
                            "<color=\"red\">Not enough players on this ward are online</color>",
                            Talker.Type.Normal);
                        Chat.m_instance.AddString("[WardIsLove]",
                            "<color=\"red\">All structures inside the ward are indestructible</color>",
                            Talker.Type.Normal);
                        messageHappened = true;
                        return;
                    }

                    foreach (KeyValuePair<long, string> name in permittedList)
                    {
                        if (stringList.Contains(name.Value) && !messageHappened)
                        {
                            j++;
                            if (j < _raidablePlayersNeeded.Value)
                            {
                                Raidable = false;
                                //Player.m_localPlayer.Message(MessageHud.MessageType.Center, "<color=\"red\">Not-Raidable\nOwner: " + componentInParent.GetCreatorName() + "\nPermitted Players: " + string.Join("\n", name.Value) + "</color>", 0, (Sprite)null);
                                Chat.m_instance.AddString("[WardIsLove]",
                                    "<color=\"red\">Not-Raidable</color>", Talker.Type.Normal);
                                Chat.m_instance.AddString("[WardIsLove]",
                                    "<color=\"red\">Owner: " + componentInParent.GetCreatorName() + "</color>",
                                    Talker.Type.Normal);
                                Chat.m_instance.AddString("[WardIsLove]",
                                    "<color=\"red\">Permitted Players: " + string.Join("\n", name.Value) +
                                    "</color>", Talker.Type.Normal);
                                messageHappened = true;
                                return;
                            }

                            Raidable = true;
                            // Player.m_localPlayer.Message(MessageHud.MessageType.Center, "<color=\"green\">Raidable</color>", 0, null);
                            Chat.m_instance.AddString("[WardIsLove]", "<color=\"green\">Raidable</color>",
                                Talker.Type.Normal);
                            messageHappened = true;
                            return;
                        }

                        if (WardMonoscript.CheckInWardMonoscript(Player.m_localPlayer.transform.position) &&
                            !CustomCheck.CheckAccess(Player.m_localPlayer.GetPlayerID(),
                                Player.m_localPlayer.transform.position, flash: false))
                        {
                            Raidable = false;

                            // Player.m_localPlayer.Message(MessageHud.MessageType.Center, "<color=\"red\">Not enough players on this ward are online\nAll structures inside the ward are indestructible</color>", 0, null);
                            Chat.m_instance.AddString("[WardIsLove]",
                                "<color=\"red\">Not enough players on this ward are online</color>",
                                Talker.Type.Normal);
                            Chat.m_instance.AddString("[WardIsLove]",
                                "<color=\"red\">All structures inside the ward are indestructible</color>",
                                Talker.Type.Normal);
                            messageHappened = true;
                            return;
                        }

                        if (messageHappened)
                            break;
                    }
                }
            }
            else
            {
                if (!WardMonoscript.CheckInWardMonoscript(Player.m_localPlayer.transform.position))
                    messageHappened = false;
                if (WardMonoscript.CheckInWardMonoscript(Player.m_localPlayer.transform.position) &&
                    CustomCheck.CheckAccess(Player.m_localPlayer.GetPlayerID(), Player.m_localPlayer.transform.position,
                        flash: false)) Raidable = true;
            }
        }
    }
}