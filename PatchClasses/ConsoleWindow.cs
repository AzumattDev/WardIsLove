using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    public class ConsoleWindow
    {
        [HarmonyPatch(typeof(Terminal), nameof(Terminal.IsCheatsEnabled))]
        public static class Terminal_IsCheatsEnabled_Patch
        {
            public static bool Prefix(ref bool __result)
            {
                __result = true;
                return false;
            }
        }

        [HarmonyPatch(typeof(Terminal), nameof(Terminal.InitTerminal))]
        public static class Console_Patch
        {
            private static void Postfix()
            {
#if DEBUG
                WILLogger.LogDebug("Patching Console Commands");
#endif

                /*if (!ValidServer || !Admin)
                    return;*/


                /* Permit */
                Terminal.ConsoleCommand permitCommand = new("permit", "Permit on wards around you",
                    args =>
                    {
                        Player localPlayer = Player.m_localPlayer;
                        Vector3 pos = localPlayer.transform.position;
                        PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
                        List<GameObject> gameObjectList = new();
                        foreach (Collider component in Physics.OverlapSphere(localPlayer.transform.position,
                                     _wardRange.Value * 1.5f))
                        {
                            WardMonoscript componentInParent = component.GetComponentInParent<WardMonoscript>();
                            if (!componentInParent || !(Vector3.Distance(localPlayer.transform.position,
                                                            componentInParent.transform.position) <=
                                                        (double)_wardRange.Value)) continue;
                            componentInParent.AddPermitted(playerProfile.m_playerID, playerProfile.m_playerName);
                            if (gameObjectList.Contains(componentInParent.gameObject)) continue;
                            gameObjectList.Add(componentInParent.gameObject);
                            args.Context?.AddString(
                                $"<color=green>Ward in position: {componentInParent.transform.position} now permitted for {playerProfile.m_playerName}</color>");
                        }
                    }, true);

                /* UnPermit */
                Terminal.ConsoleCommand unpermitCommand =
                    new("unpermit", "Unpermit on wards around you",
                        args =>
                        {
                            Player localPlayer = Player.m_localPlayer;
                            Vector3 pos = localPlayer.transform.position;
                            PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
                            List<GameObject> gameObjectList = new();
                            foreach (Collider component in Physics.OverlapSphere(localPlayer.transform.position,
                                         _wardRange.Value * 1.5f))
                            {
                                WardMonoscript componentInParent = component.GetComponentInParent<WardMonoscript>();
                                if (!componentInParent || !(Vector3.Distance(localPlayer.transform.position,
                                                                componentInParent.transform.position) <=
                                                            (double)_wardRange.Value)) continue;
                                List<KeyValuePair<long, string>> permittedPlayers =
                                    componentInParent.GetPermittedPlayers();
                                if (permittedPlayers.RemoveAll(x => x.Key == Player.m_localPlayer.GetPlayerID()) <= 0)
                                    return;
                                componentInParent.SetPermittedPlayers(permittedPlayers);
                                Transform transform = componentInParent.transform;
                                _ = componentInParent.m_removedPermittedEffect.Create(transform.position,
                                    transform.rotation);
                                //componentInParent.m_activateEffect.Create(((Component)componentInParent).transform.position, ((Component)componentInParent).transform.rotation, null, 4);
                                if (gameObjectList.Contains(componentInParent.gameObject)) continue;
                                gameObjectList.Add(componentInParent.gameObject);
                                args.Context?.AddString(
                                    $"<color=yellow>Ward in position: {componentInParent.transform.position} removed permitted for {playerProfile.m_playerName}</color>");
                            }
                        });

                /* Disable */
                Terminal.ConsoleCommand disableCommand =
                    new("disable", "Disable wards around you",
                        args =>
                        {
                            Player localPlayer = Player.m_localPlayer;
                            Vector3 pos = localPlayer.transform.position;
                            PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
                            List<GameObject> gameObjectList = new();
                            foreach (Collider component in Physics.OverlapSphere(localPlayer.transform.position,
                                         _wardRange.Value * 1.5f))
                            {
                                WardMonoscript componentInParent = component.GetComponentInParent<WardMonoscript>();
                                bool flag = true;
                                if (!_wardEnabled.Value && componentInParent)
                                    flag = componentInParent.m_piece.GetCreator() ==
                                           Game.instance.GetPlayerProfile().GetPlayerID() ||
                                           componentInParent.IsPermitted(Game.instance.GetPlayerProfile()
                                               .GetPlayerID());
                                if (((!(bool)componentInParent ? 0 :
                                        Vector3.Distance(localPlayer.transform.position,
                                            componentInParent.transform.position) <=
                                        (double)_wardRange.Value ? 1 : 0) & (flag ? 1 : 0)) == 0) continue;
                                componentInParent.SetEnabled(false);
                                if (gameObjectList.Contains(componentInParent.gameObject)) continue;
                                gameObjectList.Add(componentInParent.gameObject);
                                args.Context?.AddString(
                                    $"<color=orange>Ward in position: {componentInParent.transform.position} now disabled</color>");
                            }
                        });
                /* Enable */
                Terminal.ConsoleCommand enableCommand =
                    new("enable", "Enable wards around you",
                        args =>
                        {
                            Player localPlayer = Player.m_localPlayer;
                            Vector3 pos = localPlayer.transform.position;
                            PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
                            List<GameObject> gameObjectList = new();
                            foreach (Collider component in Physics.OverlapSphere(localPlayer.transform.position,
                                         _wardRange.Value * 1.5f))
                            {
                                WardMonoscript componentInParent = component.GetComponentInParent<WardMonoscript>();
                                bool flag = true;
                                if (!_wardEnabled.Value && componentInParent)
                                    flag = componentInParent.m_piece.GetCreator() ==
                                           Game.instance.GetPlayerProfile().GetPlayerID() ||
                                           componentInParent.IsPermitted(Game.instance.GetPlayerProfile()
                                               .GetPlayerID());
                                if (((!(bool)componentInParent ? 0 :
                                        Vector3.Distance(localPlayer.transform.position,
                                            componentInParent.transform.position) <=
                                        (double)_wardRange.Value ? 1 : 0) & (flag ? 1 : 0)) == 0) continue;
                                componentInParent.SetEnabled(true);
                                if (gameObjectList.Contains(componentInParent.gameObject)) continue;
                                gameObjectList.Add(componentInParent.gameObject);
                                args.Context?.AddString(
                                    $"<color=lightgreen>Ward in position: {componentInParent.transform.position} now enabled</color>");
                            }
                        });
                /* Flash Wards */
                Terminal.ConsoleCommand flashCommand =
                    new("flash", "Flash wards around you (Everyone can see this)",
                        args =>
                        {
                            Player localPlayer = Player.m_localPlayer;
                            Vector3 pos = localPlayer.transform.position;
                            PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
                            List<GameObject> gameObjectList = new();
                            foreach (Collider component in Physics.OverlapSphere(localPlayer.transform.position,
                                         _wardRange.Value * 1.5f))
                            {
                                WardMonoscript componentInParent = component.GetComponentInParent<WardMonoscript>();
                                bool flag = true;
                                if (!_wardEnabled.Value && componentInParent)
                                    flag = componentInParent.m_piece.GetCreator() ==
                                           Game.instance.GetPlayerProfile().GetPlayerID() ||
                                           componentInParent.IsPermitted(Game.instance.GetPlayerProfile()
                                               .GetPlayerID());
                                if (((!(bool)componentInParent ? 0 :
                                        Vector3.Distance(localPlayer.transform.position,
                                            componentInParent.transform.position) <=
                                        (double)_wardRange.Value ? 1 : 0) & (flag ? 1 : 0)) == 0) continue;
                                componentInParent.FlashShield(true);
                                if (gameObjectList.Contains(componentInParent.gameObject)) continue;
                                gameObjectList.Add(componentInParent.gameObject);
                                args.Context?.AddString(
                                    $"<color=yellow>Flashing Shield for ward in position: {componentInParent.transform.position}</color>");
                            }
                        });
            }
        }
    }
}