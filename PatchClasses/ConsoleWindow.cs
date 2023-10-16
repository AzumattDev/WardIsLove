using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch(typeof(Terminal), nameof(Terminal.IsCheatsEnabled))]
    public static class Terminal_IsCheatsEnabled_Patch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return __result;
        }
    }

    [HarmonyPatch(typeof(Terminal), nameof(Terminal.InitTerminal))]
    static class TerminalInitTerminalPatch
    {
        private static void Postfix()
        {
            WILLogger.LogDebug("Patching Console Commands");

            /* Permit */
            Terminal.ConsoleCommand permitCommand = new("permit", "Permit on wards around you",
                args =>
                {
                    if (WardIsLovePlugin.Admin)
                    {
                        Player localPlayer = Player.m_localPlayer;
                        Vector3 pos = localPlayer.transform.position;
                        PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
                        List<GameObject> gameObjectList = new();
                        foreach (Collider component in Physics.OverlapSphere(localPlayer.transform.position,
                                     WardRange.Value * 1.5f))
                        {
                            WardMonoscript componentInParent = component.GetComponentInParent<WardMonoscript>();
                            if (!componentInParent || !(Vector3.Distance(localPlayer.transform.position,
                                                            componentInParent.transform.position) <=
                                                        (double)WardRange.Value)) continue;
                            componentInParent.AddPermitted(playerProfile.m_playerID, playerProfile.m_playerName);
                            if (gameObjectList.Contains(componentInParent.gameObject)) continue;
                            gameObjectList.Add(componentInParent.gameObject);
                            args.Context?.AddString(
                                $"<color=#00FF00>Ward in position: {componentInParent.transform.position} now permitted for {playerProfile.m_playerName}</color>");
                        }
                    }
                    else
                    {
                        args.Context?.AddString($"<color=#FF0000>You are not an admin</color>");
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
                                     WardRange.Value * 1.5f))
                        {
                            WardMonoscript componentInParent = component.GetComponentInParent<WardMonoscript>();
                            if (!componentInParent || !(Vector3.Distance(localPlayer.transform.position,
                                                            componentInParent.transform.position) <=
                                                        (double)WardRange.Value)) continue;
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
                                $"<color=#FFFF00>Ward in position: {componentInParent.transform.position} removed permitted for {playerProfile.m_playerName}</color>");
                        }
                    });

            /* Disable */
            Terminal.ConsoleCommand disableCommand =
                new("disable", "Disable wards around you",
                    args =>
                    {
                        if (Admin)
                        {
                            Player localPlayer = Player.m_localPlayer;
                            Vector3 pos = localPlayer.transform.position;
                            PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
                            List<GameObject> gameObjectList = new();
                            foreach (Collider component in Physics.OverlapSphere(localPlayer.transform.position,
                                         WardRange.Value * 1.5f))
                            {
                                WardMonoscript componentInParent = component.GetComponentInParent<WardMonoscript>();
                                bool flag = true;
                                if (!WardEnabled.Value && componentInParent)
                                    flag = componentInParent.m_piece.GetCreator() ==
                                           Game.instance.GetPlayerProfile().GetPlayerID() ||
                                           componentInParent.IsPermitted(Game.instance.GetPlayerProfile()
                                               .GetPlayerID());
                                if (((!(bool)componentInParent ? 0 :
                                        Vector3.Distance(localPlayer.transform.position,
                                            componentInParent.transform.position) <=
                                        (double)WardRange.Value ? 1 : 0) & (flag ? 1 : 0)) == 0) continue;
                                componentInParent.SetEnabled(false);
                                if (gameObjectList.Contains(componentInParent.gameObject)) continue;
                                gameObjectList.Add(componentInParent.gameObject);
                                args.Context?.AddString(
                                    $"<color=#FFA500>Ward in position: {componentInParent.transform.position} now disabled</color>");
                            }
                        }
                        else
                        {
                            args.Context?.AddString($"<color=#FF0000>You are not an admin</color>");
                        }
                    }, isCheat: true);
            /* Enable */
            Terminal.ConsoleCommand enableCommand =
                new("enable", "Enable wards around you",
                    args =>
                    {
                        if (Admin)
                        {
                            Player localPlayer = Player.m_localPlayer;
                            Vector3 pos = localPlayer.transform.position;
                            PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
                            List<GameObject> gameObjectList = new();
                            foreach (Collider component in Physics.OverlapSphere(localPlayer.transform.position,
                                         WardRange.Value * 1.5f))
                            {
                                WardMonoscript componentInParent = component.GetComponentInParent<WardMonoscript>();
                                bool flag = true;
                                if (!WardEnabled.Value && componentInParent)
                                    flag = componentInParent.m_piece.GetCreator() ==
                                           Game.instance.GetPlayerProfile().GetPlayerID() ||
                                           componentInParent.IsPermitted(Game.instance.GetPlayerProfile()
                                               .GetPlayerID());
                                if (((!(bool)componentInParent ? 0 :
                                        Vector3.Distance(localPlayer.transform.position,
                                            componentInParent.transform.position) <=
                                        (double)WardRange.Value ? 1 : 0) & (flag ? 1 : 0)) == 0) continue;
                                componentInParent.SetEnabled(true);
                                if (gameObjectList.Contains(componentInParent.gameObject)) continue;
                                gameObjectList.Add(componentInParent.gameObject);
                                args.Context?.AddString(
                                    $"<color=lightgreen>Ward in position: {componentInParent.transform.position} now enabled</color>");
                            }
                        }
                        else
                        {
                            args.Context?.AddString($"<color=#FF0000>You are not an admin</color>");
                        }
                    }, isCheat: true);
            /* Flash Wards */
            Terminal.ConsoleCommand flashCommand =
                new("flash", "Flash wards around you (Everyone can see this)",
                    args =>
                    {
                        if (Admin)
                        {
                            Player localPlayer = Player.m_localPlayer;
                            Vector3 pos = localPlayer.transform.position;
                            PlayerProfile playerProfile = Game.instance.GetPlayerProfile();
                            List<GameObject> gameObjectList = new();
                            foreach (Collider component in Physics.OverlapSphere(localPlayer.transform.position,
                                         WardRange.Value * 1.5f))
                            {
                                WardMonoscript componentInParent = component.GetComponentInParent<WardMonoscript>();
                                bool flag = true;
                                if (!WardEnabled.Value && componentInParent)
                                    flag = componentInParent.m_piece.GetCreator() ==
                                           Game.instance.GetPlayerProfile().GetPlayerID() ||
                                           componentInParent.IsPermitted(Game.instance.GetPlayerProfile()
                                               .GetPlayerID());
                                if (((!(bool)componentInParent ? 0 :
                                        Vector3.Distance(localPlayer.transform.position,
                                            componentInParent.transform.position) <=
                                        (double)WardRange.Value ? 1 : 0) & (flag ? 1 : 0)) == 0) continue;
                                componentInParent.FlashShield(true);
                                if (gameObjectList.Contains(componentInParent.gameObject)) continue;
                                gameObjectList.Add(componentInParent.gameObject);
                                args.Context?.AddString(
                                    $"<color=#FFFF00>Flashing Shield for ward in position: {componentInParent.transform.position}</color>");
                            }
                        }
                        else
                        {
                            args.Context?.AddString($"<color=#FF0000>You are not an admin</color>");
                        }
                    }, isCheat: true);
        }
    }
}