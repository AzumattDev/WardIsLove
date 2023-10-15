using System;
using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    static class PlayerUpdatePatch
    {
        static void Postfix(Player __instance)
        {
            if (!ZNetScene.instance) return;
            if (Game.instance && !Player.m_localPlayer) return;

            if (!Game.instance || !WardEnabled.Value) return;
            try
            {
                if (!InventoryGui.instance) return;
                PvP.PvPChecker(InventoryGui.instance);
            }
            catch (Exception exception)
            {
                //WardIsLovePlugin.WILLogger.LogError($"There was an error in setting the PvP {exception}");
            }
        }
    }

    [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.UpdateCharacterStats))]
    static class InventoryGuiUpdateCharacterStatsPatch
    {
        static void Postfix(InventoryGui __instance)
        {
            try
            {
                if (!__instance) return;
                PvP.PvPChecker(__instance);
            }
            catch (Exception exception)
            {
                //WardIsLovePlugin.WILLogger.LogError($"There was an error in setting the PvP {exception}");
            }
        }
    }

    public static class PvP
    {
        internal static void PvPChecker(InventoryGui invGUI)
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