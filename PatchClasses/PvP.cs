using System;
using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    /// make sure PvP is on
    [HarmonyPatch]
    public static class PvP
    {
        [HarmonyPatch(typeof(Player), nameof(Player.Update))]
        [HarmonyPostfix]
        private static void Postfix(Player __instance)
        {
            if (!ZNetScene.instance) return;
            if (Game.m_instance && !Player.m_localPlayer) return;

            if (!Game.m_instance || !_wardEnabled.Value) return;
            try
            {
                if (!InventoryGui.instance) return;
                PvPChecker(InventoryGui.instance);
            }
            catch (Exception exception)
            {
                //WardIsLovePlugin.WILLogger.LogError($"There was an error in setting the PvP {exception}");
            }
        }


        [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.UpdateCharacterStats))]
        [HarmonyPostfix]
        private static void Postfix(InventoryGui __instance)
        {
            try
            {
                if (!__instance) return;
                PvPChecker(__instance);
            }
            catch (Exception exception)
            {
                //WardIsLovePlugin.WILLogger.LogError($"There was an error in setting the PvP {exception}");
            }
        }

        private static void PvPChecker(InventoryGui invGUI)
        {
            if (!Player.m_localPlayer) return;
            foreach (WardMonoscript? pa in WardMonoscriptExt.WardMonoscriptsINSIDE)
            {
                if (pa.GetPvpOn())
                {
                    if (pa.GetOnlyPermOn() && pa.IsEnabled() &&
                        (pa.IsPermitted(Player.m_localPlayer.GetPlayerID()) || pa.m_piece.IsCreator()))
                    {
                        Player.m_localPlayer.m_pvp = pa.GetPvpOn();
                        Player.m_localPlayer.SetPVP(pa.GetPvpOn());
                        InventoryGui.instance.m_pvp.isOn = pa.GetPvpOn();
                        invGUI.m_pvp.interactable = false;
                    }
                    else if (pa.GetNotPermOn() && pa.IsEnabled() &&
                             (!pa.IsPermitted(Player.m_localPlayer.GetPlayerID()) || !pa.m_piece.IsCreator()))
                    {
                        Player.m_localPlayer.m_pvp = pa.GetPvpOn();
                        Player.m_localPlayer.SetPVP(pa.GetPvpOn());
                        InventoryGui.instance.m_pvp.isOn = pa.GetPvpOn();
                        invGUI.m_pvp.interactable = false;
                    }
                    else if (!pa.GetNotPermOn() && !pa.GetOnlyPermOn())
                    {
                        //If ward has pvp forced, apply to all players
                        Player.m_localPlayer.m_pvp = pa.GetPvpOn();
                        Player.m_localPlayer.SetPVP(pa.GetPvpOn());
                        InventoryGui.instance.m_pvp.isOn = pa.GetPvpOn();
                        invGUI.m_pvp.interactable = false;
                    }
                }

                if (pa.GetPveOn())
                {
                    if (pa.GetOnlyPermOn() && pa.IsEnabled() &&
                        (pa.IsPermitted(Player.m_localPlayer.GetPlayerID()) || pa.m_piece.IsCreator()))
                    {
                        Player.m_localPlayer.m_pvp = !pa.GetPveOn();
                        Player.m_localPlayer.SetPVP(!pa.GetPveOn());
                        InventoryGui.instance.m_pvp.isOn = !pa.GetPveOn();
                        invGUI.m_pvp.interactable = false;
                    }
                    else if (pa.GetNotPermOn() && pa.IsEnabled() &&
                             (!pa.IsPermitted(Player.m_localPlayer.GetPlayerID()) || !pa.m_piece.IsCreator()))
                    {
                        Player.m_localPlayer.m_pvp = !pa.GetPveOn();
                        Player.m_localPlayer.SetPVP(!pa.GetPveOn());
                        InventoryGui.instance.m_pvp.isOn = !pa.GetPveOn();
                        invGUI.m_pvp.interactable = false;
                    }
                    else if (!pa.GetNotPermOn() && !pa.GetOnlyPermOn())
                    {
                        //If ward has pve forced, apply to all players
                        Player.m_localPlayer.m_pvp = !pa.GetPveOn();
                        Player.m_localPlayer.SetPVP(!pa.GetPveOn());
                        InventoryGui.instance.m_pvp.isOn = !pa.GetPveOn();
                        invGUI.m_pvp.interactable = false;
                    }
                }
            }
        }
    }
}