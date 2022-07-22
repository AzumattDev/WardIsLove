using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WardIsLove.Util;

namespace WardIsLove.Extensions
{
    public static class WardMonoscriptExt
    {
        public static IEnumerable<WardMonoscript> WardMonoscriptsINSIDE;
        public static IEnumerable<WardMonoscript> WardCharacterINSIDE;

        public static IEnumerator UpdateAreas()
        {
            while (true)
            {
                try
                {
                    if (Player.m_localPlayer && WardMonoscript.m_allAreas != null)
                        WardMonoscriptsINSIDE = WardMonoscript.m_allAreas.Where(p => p != null &&
                            Vector3.Distance(Player.m_localPlayer.transform.position, p.transform.position) <=
                            p.m_nview.m_zdo.GetFloat("wardRadius", WardIsLovePlugin.WardRange.Value));

                    if (WardMonoscript.m_allAreas != null)
                        try
                        {
                            List<Character>? C = Character.GetAllCharacters();
                            foreach (Character? character in C)
                                WardCharacterINSIDE = WardMonoscript.m_allAreas.Where(p => p != null &&
                                    Vector3.Distance(character.transform.position, p.transform.position) <=
                                    p.m_nview.m_zdo.GetFloat("wardRadius", WardIsLovePlugin.WardRange.Value));
                        }
                        catch
                        {
                        }
                }
                catch (Exception ex)
                {
                    WardIsLovePlugin.WILLogger.LogError($"Error in UpdateAreas {ex}");
                }

                yield return new WaitForSeconds(2f);
            }
        }

        public static WardMonoscript GetWardMonoscript(Vector3 transformPos)
        {
            foreach (WardMonoscript allArea in WardMonoscript.m_allAreas)
                if (allArea.IsEnabled() && allArea.IsInside(transformPos, 0.0f))
                    return allArea;

            return null;
        }

        public static WardIsLovePlugin.WardInteractBehaviorEnums GetAccessMode(
            this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return (WardIsLovePlugin.WardInteractBehaviorEnums)WardMonoscript.m_nview.m_zdo.GetInt(
                    "accessMode");

            return WardIsLovePlugin.WardInteractBehaviorEnums.OwnerOnly;
        }

        public static void SetAccessMode(this WardMonoscript WardMonoscript,
            WardIsLovePlugin.WardInteractBehaviorEnums accessMode)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                WardMonoscript.m_nview.m_zdo.Set("accessMode", (int)accessMode);
            //MonoBehaviour.print((int)accessMode);
        }

        public static WardIsLovePlugin.WardBehaviorEnums GetBubbleMode(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return (WardIsLovePlugin.WardBehaviorEnums)WardMonoscript.m_nview.m_zdo.GetInt("bubbleMode");

            return WardIsLovePlugin.WardBehaviorEnums.Default;
        }

        public static void SetBubbleMode(this WardMonoscript WardMonoscript,
            WardIsLovePlugin.WardBehaviorEnums bubbleMode)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                WardMonoscript.m_nview.m_zdo.Set("bubbleMode", (int)bubbleMode);
        }

        public static HitData.DamageType GetDamageType(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return (HitData.DamageType)WardMonoscript.m_nview.m_zdo.GetInt("damageType");

            return HitData.DamageType.Lightning;
        }


        public static void SetDamageType(this WardMonoscript WardMonoscript,
            WardIsLovePlugin.WardDamageTypes damageType)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                WardMonoscript.m_nview.m_zdo.Set("damageType", (int)damageType);
        }


        public static float GetWardRadius(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetFloat("wardRadius", WardIsLovePlugin.WardRange.Value);
            return WardIsLovePlugin.WardRange.Value;
        }

        public static void SetWardRadius(this WardMonoscript WardMonoscript, float wardRadiusSliderVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("wardRadius", wardRadiusSliderVal);
        }

        public static float GetHealthBoost(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetFloat("healthRegen", WardIsLovePlugin.WardPassiveHealthRegen.Value);
            return WardIsLovePlugin.WardPassiveHealthRegen.Value;
        }

        public static void SetHealthBoost(this WardMonoscript WardMonoscript, float healthRegenSliderVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("healthRegen", healthRegenSliderVal);
        }

        public static float GetStaminaBoost(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetFloat("staminaBoost", WardIsLovePlugin.WardPassiveStaminaRegen.Value);
            return WardIsLovePlugin.WardPassiveStaminaRegen.Value;
        }

        public static void SetStaminaBoost(this WardMonoscript WardMonoscript, float staminaBoostSliderVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("staminaBoost", staminaBoostSliderVal);
        }

        public static bool GetBubbleOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("bubbleOn", WardIsLovePlugin.EnableBubble.Value);
            return WardIsLovePlugin.EnableBubble.Value;
        }

        public static void SetBubbleOn(this WardMonoscript WardMonoscript, bool bubbleOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("bubbleOn", bubbleOnVal);
        }

        public static bool GetWeatherDmgOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("weatherDmgOn", WardIsLovePlugin.NoWeatherDmg.Value);
            return WardIsLovePlugin.NoWeatherDmg.Value;
        }

        public static void SetWeatherDmgOn(this WardMonoscript WardMonoscript, bool weatherDmgOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("weatherDmgOn", weatherDmgOnVal);
        }

        public static bool GetAutoPickupOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("autoPickupOn", WardIsLovePlugin.DisablePickup.Value);
            return WardIsLovePlugin.DisablePickup.Value;
        }

        public static void SetAutoPickupOnOn(this WardMonoscript WardMonoscript, bool autoPickupOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("autoPickupOn", autoPickupOnVal);
        }

        public static bool GetAutoCloseDoorsOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("autoCloseDoorsOn", WardIsLovePlugin.AutoClose.Value);
            return WardIsLovePlugin.AutoClose.Value;
        }

        public static void SetAutoCloseDoorsOn(this WardMonoscript WardMonoscript, bool autoCloseDoorsOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("autoCloseDoorsOn", autoCloseDoorsOnVal);
        }

        public static bool GetFireplaceUnlimOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("fireplaceUnlimOn", WardIsLovePlugin.FireplaceUnlimited.Value);
            return WardIsLovePlugin.FireplaceUnlimited.Value;
        }

        public static void SetFireplaceUnlimOn(this WardMonoscript WardMonoscript, bool fireplaceUnlimOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("fireplaceUnlimOn", fireplaceUnlimOnVal);
        }

        public static bool GetBathingUnlimOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("bathingUnlimOn", WardIsLovePlugin.BathingUnlimited.Value);
            return WardIsLovePlugin.BathingUnlimited.Value;
        }

        public static void SetBathingUnlimOn(this WardMonoscript WardMonoscript, bool bathingUnlimOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("bathingUnlimOn", bathingUnlimOnVal);
        }

        public static bool GetCookingUnlimOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("cookingUnlimOn", WardIsLovePlugin.CookingUnlimited.Value);
            return WardIsLovePlugin.CookingUnlimited.Value;
        }

        public static void SetCookingUnlimOn(this WardMonoscript WardMonoscript, bool cookingUnlimOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("cookingUnlimOn", cookingUnlimOnVal);
        }

        public static string GetFireplaceList(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetString("wardFireplaceList", WardIsLovePlugin.FireSources.Value);
            return WardIsLovePlugin.FireSources.Value;
        }

        public static void SetFireplaceList(this WardMonoscript WardMonoscript, string wardFireplaceListVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("wardFireplaceList", wardFireplaceListVal);
        }

        public static bool GetNoDeathPenOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("noDeathPenOn", WardIsLovePlugin.WardNoDeathPen.Value);
            return WardIsLovePlugin.WardNoDeathPen.Value;
        }

        public static void SetNoDeathPenOn(this WardMonoscript WardMonoscript, bool noDeathPenOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("noDeathPenOn", noDeathPenOnVal);
        }

        public static bool GetNoFoodDrainOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("noFoodDrainOn", WardIsLovePlugin.NoFoodDrain.Value);
            return WardIsLovePlugin.NoFoodDrain.Value;
        }

        public static void SetNoFoodDrainOn(this WardMonoscript WardMonoscript, bool noFoodDrainOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("noFoodDrainOn", noFoodDrainOnVal);
        }

        public static bool GetPushoutPlayersOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("pushoutPlayersOn", WardIsLovePlugin.PushoutPlayers.Value);
            return WardIsLovePlugin.PushoutPlayers.Value;
        }

        public static void SetPushoutPlayersOn(this WardMonoscript WardMonoscript, bool pushoutPlayersOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("pushoutPlayersOn", pushoutPlayersOnVal);
        }

        public static bool GetPushoutCreaturesOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("pushoutCreaturesOn", WardIsLovePlugin.PushoutCreatures.Value);
            return WardIsLovePlugin.PushoutCreatures.Value;
        }

        public static void SetPushoutCreaturesOn(this WardMonoscript WardMonoscript, bool pushoutCreaturesOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("pushoutCreaturesOn", pushoutCreaturesOnVal);
        }

        public static bool GetPvpOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool("pvpOn", WardIsLovePlugin.WardPvP.Value);
            return WardIsLovePlugin.WardPvP.Value;
        }

        public static void SetPvpOn(this WardMonoscript WardMonoscript, bool pvpOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("pvpOn", pvpOnVal);
        }

        public static bool GetPveOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool("pveOn", WardIsLovePlugin.WardPve.Value);
            return WardIsLovePlugin.WardPve.Value;
        }

        public static void SetPveOn(this WardMonoscript WardMonoscript, bool PveOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("pveOn", PveOnVal);
        }

        public static bool GetNoTeleportOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("teleportOn", WardIsLovePlugin.NoTeleport.Value);
            return WardIsLovePlugin.NoTeleport.Value;
        }

        public static void SetNoTeleportOn(this WardMonoscript WardMonoscript, bool noteleportOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("teleportOn", noteleportOnVal);
        }

        public static bool GetShowFlashOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("showFlashOn", WardIsLovePlugin.ShowFlash.Value);
            return WardIsLovePlugin.ShowFlash.Value;
        }

        public static void SetShowFlashOn(this WardMonoscript WardMonoscript, bool showFlashOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("showFlashOn", showFlashOnVal);
        }

        public static bool GetShowMarkerOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("showMarkerOn", WardIsLovePlugin.ShowMarker.Value);
            return WardIsLovePlugin.ShowMarker.Value;
        }

        public static void SetShowMarkerOn(this WardMonoscript WardMonoscript, bool showMarkerOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("showMarkerOn", showMarkerOnVal);
        }

        public static bool GetWardNotificationsOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("wardNotificationsOn", WardIsLovePlugin.WardNotify.Value);
            return WardIsLovePlugin.WardNotify.Value;
        }

        public static void SetWardNotificationsOn(this WardMonoscript WardMonoscript,
            bool wardNotificationsOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("wardNotificationsOn", wardNotificationsOnVal);
        }

        public static string GetWardEnterNotifyMessage(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetString("wardNotifyMessageEntry",
                    WardIsLovePlugin.WardNotifyMessageEntry.Value);
            return WardIsLovePlugin.WardNotifyMessageEntry.Value;
        }

        public static void SetWardEnterNotifyMessage(this WardMonoscript WardMonoscript,
            string wardNotifyMessageEntryVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("wardNotifyMessageEntry", wardNotifyMessageEntryVal);
        }

        public static string GetWardExitNotifyMessage(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetString("wardNotifyMessageExit",
                    WardIsLovePlugin.WardNotifyMessageExit.Value);
            return WardIsLovePlugin.WardNotifyMessageExit.Value;
        }

        public static void SetWardExitNotifyMessage(this WardMonoscript WardMonoscript,
            string wardNotifyMessageExitVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("wardNotifyMessageExit", wardNotifyMessageExitVal);
        }

        public static float GetWardDamageAmount(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetFloat("wardDamageAmount", WardIsLovePlugin.WardDamageAmount.Value);
            return WardIsLovePlugin.WardDamageAmount.Value;
        }

        public static void SetWardDamageAmount(this WardMonoscript WardMonoscript, float wardDamageAmountVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("wardDamageAmount", wardDamageAmountVal);
        }

        public static bool GetItemStandInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("itemstandInteractOn", WardIsLovePlugin.ItemStandInteraction.Value);
            return WardIsLovePlugin.ItemStandInteraction.Value;
        }

        public static void SetItemStandInteractOn(this WardMonoscript WardMonoscript,
            bool itemStandInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("itemstandInteractOn", itemStandInteractionOnVal);
        }

        public static bool GetIndestructibleOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("indestructibleOn", WardIsLovePlugin.WardStructures.Value);
            return WardIsLovePlugin.WardStructures.Value;
        }

        public static void SetIndestructibleOn(this WardMonoscript WardMonoscript, bool indestructibleOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("indestructibleOn", indestructibleOnVal);
        }

        public static string GetIndestructList(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetString("indestructList", WardIsLovePlugin.ItemStructureNames.Value);
            return WardIsLovePlugin.ItemStructureNames.Value;
        }

        public static void SetIndestructList(this WardMonoscript WardMonoscript, string indestructListVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("indestructList", indestructListVal);
        }

        public static float GetCreatureDamageIncrease(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetFloat("CreatDamageIncreaseAmount",
                    WardIsLovePlugin.WardDamageIncrease.Value);
            return WardIsLovePlugin.WardDamageIncrease.Value;
        }

        public static void SetCreatureDamageIncrease(this WardMonoscript WardMonoscript,
            float creatureDamageIncreaseAmountVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("CreatDamageIncreaseAmount", creatureDamageIncreaseAmountVal);
        }

        public static float GetStructDamageReduc(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetFloat("structDamageReducAmount",
                    WardIsLovePlugin.WardDamageReduction.Value);
            return WardIsLovePlugin.WardDamageReduction.Value;
        }

        public static void SetStructDamageReduc(this WardMonoscript WardMonoscript,
            float structDamageReducAmountVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("structDamageReducAmount", structDamageReducAmountVal);
        }

        public static bool GetItemInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("itemInteractOn", WardIsLovePlugin.ItemInteraction.Value);
            return WardIsLovePlugin.ItemInteraction.Value;
        }

        public static void SetItemInteractOn(this WardMonoscript WardMonoscript, bool itemInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("itemInteractOn", itemInteractionOnVal);
        }

        public static bool GetDoorInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("doorInteractOn", WardIsLovePlugin.DoorInteraction.Value);
            return WardIsLovePlugin.DoorInteraction.Value;
        }

        public static void SetDoorInteractOn(this WardMonoscript WardMonoscript, bool doorInteractOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("doorInteractOn", doorInteractOnVal);
        }

        public static bool GetChestInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("chestInteractOn", WardIsLovePlugin.ChestInteraction.Value);
            return WardIsLovePlugin.ChestInteraction.Value;
        }

        public static void SetChestInteractOn(this WardMonoscript WardMonoscript, bool chestInteractOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("chestInteractOn", chestInteractOnVal);
        }

        public static bool GetPortalInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("portalInteractOn", WardIsLovePlugin.PortalInteraction.Value);
            return WardIsLovePlugin.PortalInteraction.Value;
        }

        public static void SetPortalInteractOn(this WardMonoscript WardMonoscript, bool portalInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("portalInteractOn", portalInteractionOnVal);
        }

        public static bool GetPickableInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("pickableInteractOn", WardIsLovePlugin.PickableInteraction.Value);
            return WardIsLovePlugin.PickableInteraction.Value;
        }

        public static void SetPickableInteractOn(this WardMonoscript WardMonoscript, bool pickableInteractOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("pickableInteractOn", pickableInteractOnVal);
        }

        public static bool GetShipInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("shipInteractOn", WardIsLovePlugin.ShipInteraction.Value);
            return WardIsLovePlugin.ShipInteraction.Value;
        }

        public static void SetShipInteractOn(this WardMonoscript WardMonoscript, bool shipInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("shipInteractOn", shipInteractionOnVal);
        }

        public static bool GetSignInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("signInteractOn", WardIsLovePlugin.SignInteraction.Value);
            return WardIsLovePlugin.SignInteraction.Value;
        }

        public static void SetSignInteractOn(this WardMonoscript WardMonoscript, bool signInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("signInteractOn", signInteractionOnVal);
        }

        public static bool GetCraftingStationInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("craftingStationInteractOn", WardIsLovePlugin.CraftingStationInteraction.Value);
            return WardIsLovePlugin.CraftingStationInteraction.Value;
        }

        public static void SetCraftingStationInteractOn(this WardMonoscript WardMonoscript,
            bool craftingStationInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("craftingStationInteractOn", craftingStationInteractionOnVal);
        }

        public static bool GetSmelterInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("smelterInteractOn", WardIsLovePlugin.SmelterInteraction.Value);
            return WardIsLovePlugin.SmelterInteraction.Value;
        }

        public static void SetSmelterInteractOn(this WardMonoscript WardMonoscript, bool smelterInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("smelterInteractOn", smelterInteractionOnVal);
        }

        public static bool GetBeehiveInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("beehiveInteractOn", WardIsLovePlugin.BeehiveInteraction.Value);
            return WardIsLovePlugin.BeehiveInteraction.Value;
        }

        public static void SetBeehiveInteractOn(this WardMonoscript WardMonoscript, bool beehiveInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("beehiveInteractOn", beehiveInteractionOnVal);
        }

        public static bool GetMapTableInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("maptableInteractOn", WardIsLovePlugin.MaptableInteraction.Value);
            return WardIsLovePlugin.MaptableInteraction.Value;
        }

        public static void SetMapTableInteractOn(this WardMonoscript WardMonoscript, bool maptableInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("maptableInteractOn", maptableInteractionOnVal);
        }

        public static bool GetOnlyPermOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("OnlyPermOn", WardIsLovePlugin.WardOnlyPerm.Value);
            return WardIsLovePlugin.WardOnlyPerm.Value;
        }

        public static void SetOnlyPermOn(this WardMonoscript WardMonoscript, bool onlypermOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("OnlyPermOn", onlypermOnVal);
        }

        public static bool GetNotPermOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("NotPermOn", WardIsLovePlugin.WardNotPerm.Value);
            return WardIsLovePlugin.WardNotPerm.Value;
        }

        public static void SetNotPermOn(this WardMonoscript WardMonoscript, bool NotpermOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("NotPermOn", NotpermOnVal);
        }

        public static string GetCtaMessage(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetString("ctaMessage", WardIsLovePlugin.CtaMessage.Value);
            return WardIsLovePlugin.CtaMessage.Value;
        }

        public static void SetCtaMessage(this WardMonoscript WardMonoscript, string ctaMessageVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("ctaMessage", ctaMessageVal);
        }

        public static bool GetAutoRepairOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("autoRepairOn", WardIsLovePlugin.AutoRepair.Value);
            return WardIsLovePlugin.AutoRepair.Value;
        }

        public static void SetAutoRepairOn(this WardMonoscript WardMonoscript, bool autorepairOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("autoRepairOn", autorepairOnVal);
        }

        public static float GetAutoRepairAmount(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetFloat("autoRepairAmount", WardIsLovePlugin.AutoRepairAmount.Value);
            return WardIsLovePlugin.AutoRepairAmount.Value;
        }

        public static void SetAutoRepairAmount(this WardMonoscript WardMonoscript, float autorepairOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("autoRepairAmount", autorepairOnVal);
        }

        public static float GetAutoRepairTextTime(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetFloat("autoRepairTime", WardIsLovePlugin.AutoRepairTime.Value);
            return WardIsLovePlugin.AutoRepairTime.Value;
        }

        public static void SetAutoRepairTextTime(this WardMonoscript WardMonoscript, float autoRepairTimeVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("autoRepairTime", autoRepairTimeVal);
        }

        public static bool GetRaidProtectionOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("raidProtectionOn", WardIsLovePlugin.RaidProtection.Value);
            return WardIsLovePlugin.RaidProtection.Value;
        }

        public static void SetRaidProtectionOn(this WardMonoscript WardMonoscript, bool raidProtectionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("raidProtectionOn", raidProtectionOnVal);
        }

        public static int GetRaidProtectionPlayerNeeded(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetInt("raidablePlayersNeeded", WardIsLovePlugin.RaidablePlayersNeeded.Value);
            return WardIsLovePlugin.RaidablePlayersNeeded.Value;
        }
        public static void SetRaidProtectionPlayerNeeded(this WardMonoscript WardMonoscript,
            int raidablePlayersNeededVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("raidablePlayersNeeded", raidablePlayersNeededVal);
        }

        public static bool GetWardIsLoveOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("wardIsLoveOn");
            return false;
        }

        public static void SetWardIsLoveOn(this WardMonoscript WardMonoscript, bool wardIsLoveOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("wardIsLoveOn", wardIsLoveOnVal);
        }

        public static bool WILWardLimitCheck(this WardMonoscript area)
        {
            return area.m_nview.IsValid() && area.m_nview.m_zdo.GetBool("WILLimitedWard") &&
                   EnvMan.instance.GetCurrentDay() - area.m_nview.m_zdo.GetInt("WILLimitedWardTime") <
                   WardIsLovePlugin.MaxDaysDifference;
        }

        internal static void EmissionSetter(this WardMonoscript area)
        {
            bool flag = area.IsEnabled();
            foreach (Material material in area.m_model.materials)
                if (flag)
                    material.EnableKeyword("_EMISSION");
                else
                    material.DisableKeyword("_EMISSION");
            foreach (Material material in area.m_modelLoki.materials)
                if (flag)
                    material.EnableKeyword("_EMISSION");
                else
                    material.DisableKeyword("_EMISSION");
            foreach (Material material in area.m_modelHel.materials)
                if (flag)
                    material.EnableKeyword("_EMISSION");
                else
                    material.DisableKeyword("_EMISSION");
            foreach (Material material in area.m_modelBetterWard.materials)
                if (flag)
                    material.EnableKeyword("_EMISSION");
                else
                    material.DisableKeyword("_EMISSION");
            foreach (Material material in area.m_modelBetterWard_Type2.materials)
                if (flag)
                    material.EnableKeyword("_EMISSION");
                else
                    material.DisableKeyword("_EMISSION");
            foreach (Material material in area.m_modelBetterWard_Type3.materials)
                if (flag)
                    material.EnableKeyword("_EMISSION");
                else
                    material.DisableKeyword("_EMISSION");
            foreach (Material material in area.m_modelBetterWard_Type4.materials)
                if (flag)
                    material.EnableKeyword("_EMISSION");
                else
                    material.DisableKeyword("_EMISSION");
        }
    }
}
