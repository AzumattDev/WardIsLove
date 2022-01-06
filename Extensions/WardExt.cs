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
                            p.m_nview.m_zdo.GetFloat("wardRadius", WardIsLovePlugin._wardRange.Value));

                    if (WardMonoscript.m_allAreas != null)
                        try
                        {
                            List<Character>? C = Character.GetAllCharacters();
                            foreach (Character? character in C)
                                WardCharacterINSIDE = WardMonoscript.m_allAreas.Where(p => p != null &&
                                    Vector3.Distance(character.transform.position, p.transform.position) <=
                                    p.m_nview.m_zdo.GetFloat("wardRadius", WardIsLovePlugin._wardRange.Value));
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
            HitData.DamageType damageType)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                WardMonoscript.m_nview.m_zdo.Set("damageType", (int)damageType);
        }


        public static float GetWardRadius(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetFloat("wardRadius", WardIsLovePlugin._wardRange.Value);
            return WardIsLovePlugin._wardRange.Value;
        }

        public static void SetWardRadius(this WardMonoscript WardMonoscript, float wardRadiusSliderVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("wardRadius", wardRadiusSliderVal);
        }

        public static float GetHealthBoost(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetFloat("healthBoost", WardIsLovePlugin._wardHealthBoostValue.Value);
            return WardIsLovePlugin._wardHealthBoostValue.Value;
        }

        public static void SetHealthBoost(this WardMonoscript WardMonoscript, float healthBoostSliderVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("healthBoost", healthBoostSliderVal);
        }

        public static float GetStaminaBoost(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetFloat("staminaBoost", WardIsLovePlugin._wardStaminaBoostValue.Value);
            return WardIsLovePlugin._wardStaminaBoostValue.Value;
        }

        public static void SetStaminaBoost(this WardMonoscript WardMonoscript, float staminaBoostSliderVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("staminaBoost", staminaBoostSliderVal);
        }

        public static bool GetBubbleOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("bubbleOn", WardIsLovePlugin._enableBubble.Value);
            return WardIsLovePlugin._enableBubble.Value;
        }

        public static void SetBubbleOn(this WardMonoscript WardMonoscript, bool bubbleOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("bubbleOn", bubbleOnVal);
        }

        public static bool GetWeatherDmgOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("weatherDmgOn", WardIsLovePlugin._noWeatherDmg.Value);
            return WardIsLovePlugin._noWeatherDmg.Value;
        }

        public static void SetWeatherDmgOn(this WardMonoscript WardMonoscript, bool weatherDmgOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("weatherDmgOn", weatherDmgOnVal);
        }

        public static bool GetAutoPickupOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("autoPickupOn", WardIsLovePlugin._disablePickup.Value);
            return WardIsLovePlugin._disablePickup.Value;
        }

        public static void SetAutoPickupOnOn(this WardMonoscript WardMonoscript, bool autoPickupOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("autoPickupOn", autoPickupOnVal);
        }

        public static bool GetAutoCloseDoorsOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("autoCloseDoorsOn", WardIsLovePlugin._autoClose.Value);
            return WardIsLovePlugin._autoClose.Value;
        }

        public static void SetAutoCloseDoorsOn(this WardMonoscript WardMonoscript, bool autoCloseDoorsOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("autoCloseDoorsOn", autoCloseDoorsOnVal);
        }

        public static bool GetFireplaceUnlimOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("fireplaceUnlimOn", WardIsLovePlugin._fireplaceUnlimited.Value);
            return WardIsLovePlugin._fireplaceUnlimited.Value;
        }

        public static void SetFireplaceUnlimOn(this WardMonoscript WardMonoscript, bool fireplaceUnlimOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("fireplaceUnlimOn", fireplaceUnlimOnVal);
        }

        public static bool GetBathingUnlimOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("bathingUnlimOn", WardIsLovePlugin._bathingUnlimited.Value);
            return WardIsLovePlugin._bathingUnlimited.Value;
        }

        public static void SetBathingUnlimOn(this WardMonoscript WardMonoscript, bool bathingUnlimOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("bathingUnlimOn", bathingUnlimOnVal);
        }

        public static bool GetCookingUnlimOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("cookingUnlimOn", WardIsLovePlugin._cookingUnlimited.Value);
            return WardIsLovePlugin._cookingUnlimited.Value;
        }

        public static void SetCookingUnlimOn(this WardMonoscript WardMonoscript, bool cookingUnlimOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("cookingUnlimOn", cookingUnlimOnVal);
        }

        public static string GetFireplaceList(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetString("wardFireplaceList", WardIsLovePlugin._fireSources.Value);
            return WardIsLovePlugin._fireSources.Value;
        }

        public static void SetFireplaceList(this WardMonoscript WardMonoscript, string wardFireplaceListVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("wardFireplaceList", wardFireplaceListVal);
        }

        public static bool GetNoDeathPenOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("noDeathPenOn", WardIsLovePlugin._wardNoDeathPen.Value);
            return WardIsLovePlugin._wardNoDeathPen.Value;
        }

        public static void SetNoDeathPenOn(this WardMonoscript WardMonoscript, bool noDeathPenOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("noDeathPenOn", noDeathPenOnVal);
        }

        public static bool GetNoFoodDrainOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("noFoodDrainOn", WardIsLovePlugin._noFoodDrain.Value);
            return WardIsLovePlugin._noFoodDrain.Value;
        }

        public static void SetNoFoodDrainOn(this WardMonoscript WardMonoscript, bool noFoodDrainOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("noFoodDrainOn", noFoodDrainOnVal);
        }

        public static bool GetPushoutPlayersOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("pushoutPlayersOn", WardIsLovePlugin._pushoutPlayers.Value);
            return WardIsLovePlugin._pushoutPlayers.Value;
        }

        public static void SetPushoutPlayersOn(this WardMonoscript WardMonoscript, bool pushoutPlayersOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("pushoutPlayersOn", pushoutPlayersOnVal);
        }

        public static bool GetPushoutCreaturesOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("pushoutCreaturesOn", WardIsLovePlugin._pushoutCreatures.Value);
            return WardIsLovePlugin._pushoutCreatures.Value;
        }

        public static void SetPushoutCreaturesOn(this WardMonoscript WardMonoscript, bool pushoutCreaturesOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("pushoutCreaturesOn", pushoutCreaturesOnVal);
        }

        public static bool GetPvpOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool("pvpOn", WardIsLovePlugin._wardPvP.Value);
            return WardIsLovePlugin._wardPvP.Value;
        }

        public static void SetPvpOn(this WardMonoscript WardMonoscript, bool pvpOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("pvpOn", pvpOnVal);
        }

        public static bool GetPveOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool("pveOn", WardIsLovePlugin._wardPve.Value);
            return WardIsLovePlugin._wardPve.Value;
        }

        public static void SetPveOn(this WardMonoscript WardMonoscript, bool PveOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("pveOn", PveOnVal);
        }

        public static bool GetNoTeleportOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("teleportOn", WardIsLovePlugin._noTeleport.Value);
            return WardIsLovePlugin._noTeleport.Value;
        }

        public static void SetNoTeleportOn(this WardMonoscript WardMonoscript, bool noteleportOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("teleportOn", noteleportOnVal);
        }

        public static bool GetShowFlashOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("showFlashOn", WardIsLovePlugin._showFlash.Value);
            return WardIsLovePlugin._showFlash.Value;
        }

        public static void SetShowFlashOn(this WardMonoscript WardMonoscript, bool showFlashOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("showFlashOn", showFlashOnVal);
        }

        public static bool GetShowMarkerOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("showMarkerOn", WardIsLovePlugin._showMarker.Value);
            return WardIsLovePlugin._showMarker.Value;
        }

        public static void SetShowMarkerOn(this WardMonoscript WardMonoscript, bool showMarkerOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("showMarkerOn", showMarkerOnVal);
        }

        public static bool GetWardNotificationsOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("wardNotificationsOn", WardIsLovePlugin._wardNotify.Value);
            return WardIsLovePlugin._wardNotify.Value;
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
                    WardIsLovePlugin._wardNotifyMessageEntry.Value);
            return WardIsLovePlugin._wardNotifyMessageEntry.Value;
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
                    WardIsLovePlugin._wardNotifyMessageExit.Value);
            return WardIsLovePlugin._wardNotifyMessageExit.Value;
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
                    .GetFloat("wardDamageAmount", WardIsLovePlugin._wardDamageAmount.Value);
            return WardIsLovePlugin._wardDamageAmount.Value;
        }

        public static void SetWardDamageAmount(this WardMonoscript WardMonoscript, float wardDamageAmountVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("wardDamageAmount", wardDamageAmountVal);
        }

        public static bool GetItemStandInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("itemstandInteractOn", WardIsLovePlugin._itemStandInteraction.Value);
            return WardIsLovePlugin._itemStandInteraction.Value;
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
                    .GetBool("indestructibleOn", WardIsLovePlugin._wardStructures.Value);
            return WardIsLovePlugin._wardStructures.Value;
        }

        public static void SetIndestructibleOn(this WardMonoscript WardMonoscript, bool indestructibleOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("indestructibleOn", indestructibleOnVal);
        }

        public static string GetIndestructList(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetString("indestructList", WardIsLovePlugin._itemStructureNames.Value);
            return WardIsLovePlugin._itemStructureNames.Value;
        }

        public static void SetIndestructList(this WardMonoscript WardMonoscript, string indestructListVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("indestructList", indestructListVal);
        }
        
        public static int GetCreatureDamageIncrease(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetInt("creatureDmgeIncreaseAmount",
                    WardIsLovePlugin._wardDamageIncrease.Value);
            return WardIsLovePlugin._wardDamageIncrease.Value;
        }

        public static void SetCreatureDamageIncrease(this WardMonoscript WardMonoscript,
            float creatureDamageIncreaseAmountVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("creatureDmgeIncreaseAmount", creatureDamageIncreaseAmountVal);
        }

        public static float GetStructDamageReduc(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetFloat("structDamageReducAmount",
                    WardIsLovePlugin._wardDamageReduction.Value);
            return WardIsLovePlugin._wardDamageReduction.Value;
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
                    .GetBool("itemInteractOn", WardIsLovePlugin._itemInteraction.Value);
            return WardIsLovePlugin._itemInteraction.Value;
        }

        public static void SetItemInteractOn(this WardMonoscript WardMonoscript, bool itemInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("itemInteractOn", itemInteractionOnVal);
        }

        public static bool GetDoorInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("doorInteractOn", WardIsLovePlugin._doorInteraction.Value);
            return WardIsLovePlugin._doorInteraction.Value;
        }

        public static void SetDoorInteractOn(this WardMonoscript WardMonoscript, bool doorInteractOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("doorInteractOn", doorInteractOnVal);
        }

        public static bool GetChestInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("chestInteractOn", WardIsLovePlugin._chestInteraction.Value);
            return WardIsLovePlugin._chestInteraction.Value;
        }

        public static void SetChestInteractOn(this WardMonoscript WardMonoscript, bool chestInteractOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("chestInteractOn", chestInteractOnVal);
        }

        public static bool GetPortalInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("portalInteractOn", WardIsLovePlugin._portalInteraction.Value);
            return WardIsLovePlugin._portalInteraction.Value;
        }

        public static void SetPortalInteractOn(this WardMonoscript WardMonoscript, bool portalInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("portalInteractOn", portalInteractionOnVal);
        }

        public static bool GetPickableInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("pickableInteractOn", WardIsLovePlugin._pickableInteraction.Value);
            return WardIsLovePlugin._pickableInteraction.Value;
        }

        public static void SetPickableInteractOn(this WardMonoscript WardMonoscript, bool pickableInteractOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("pickableInteractOn", pickableInteractOnVal);
        }

        public static bool GetShipInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("shipInteractOn", WardIsLovePlugin._shipInteraction.Value);
            return WardIsLovePlugin._shipInteraction.Value;
        }

        public static void SetShipInteractOn(this WardMonoscript WardMonoscript, bool shipInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("shipInteractOn", shipInteractionOnVal);
        }

        public static bool GetSignInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("signInteractOn", WardIsLovePlugin._signInteraction.Value);
            return WardIsLovePlugin._signInteraction.Value;
        }

        public static void SetSignInteractOn(this WardMonoscript WardMonoscript, bool signInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("signInteractOn", signInteractionOnVal);
        }

        public static bool GetCraftingStationInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("craftingStationInteractOn", WardIsLovePlugin._craftingStationInteraction.Value);
            return WardIsLovePlugin._craftingStationInteraction.Value;
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
                    .GetBool("smelterInteractOn", WardIsLovePlugin._smelterInteraction.Value);
            return WardIsLovePlugin._smelterInteraction.Value;
        }

        public static void SetSmelterInteractOn(this WardMonoscript WardMonoscript, bool smelterInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("smelterInteractOn", smelterInteractionOnVal);
        }

        public static bool GetBeehiveInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("beehiveInteractOn", WardIsLovePlugin._beehiveInteraction.Value);
            return WardIsLovePlugin._beehiveInteraction.Value;
        }

        public static void SetBeehiveInteractOn(this WardMonoscript WardMonoscript, bool beehiveInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("beehiveInteractOn", beehiveInteractionOnVal);
        }

        public static bool GetMapTableInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("maptableInteractOn", WardIsLovePlugin._maptableInteraction.Value);
            return WardIsLovePlugin._maptableInteraction.Value;
        }

        public static void SetMapTableInteractOn(this WardMonoscript WardMonoscript, bool maptableInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("maptableInteractOn", maptableInteractionOnVal);
        }

        public static bool GetOnlyPermOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("OnlyPermOn", WardIsLovePlugin._wardOnlyPerm.Value);
            return WardIsLovePlugin._wardOnlyPerm.Value;
        }

        public static void SetOnlyPermOn(this WardMonoscript WardMonoscript, bool onlypermOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("OnlyPermOn", onlypermOnVal);
        }

        public static bool GetNotPermOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("NotPermOn", WardIsLovePlugin._wardNotPerm.Value);
            return WardIsLovePlugin._wardNotPerm.Value;
        }

        public static void SetNotPermOn(this WardMonoscript WardMonoscript, bool NotpermOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("NotPermOn", NotpermOnVal);
        }

        public static string GetCtaMessage(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetString("ctaMessage", WardIsLovePlugin._ctaMessage.Value);
            return WardIsLovePlugin._ctaMessage.Value;
        }

        public static void SetCtaMessage(this WardMonoscript WardMonoscript, string ctaMessageVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("ctaMessage", ctaMessageVal);
        }

        public static bool GetAutoRepairOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("autoRepairOn", WardIsLovePlugin._autoRepair.Value);
            return WardIsLovePlugin._autoRepair.Value;
        }

        public static void SetAutoRepairOn(this WardMonoscript WardMonoscript, bool autorepairOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("autoRepairOn", autorepairOnVal);
        }

        public static float GetAutoRepairAmount(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetFloat("autoRepairAmount", WardIsLovePlugin._autoRepairAmount.Value);
            return WardIsLovePlugin._autoRepairAmount.Value;
        }

        public static void SetAutoRepairAmount(this WardMonoscript WardMonoscript, float autorepairOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("autoRepairAmount", autorepairOnVal);
        }

        public static float GetAutoRepairTextTime(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetFloat("autoRepairTime", WardIsLovePlugin._autoRepairTime.Value);
            return WardIsLovePlugin._autoRepairTime.Value;
        }

        public static void SetAutoRepairTextTime(this WardMonoscript WardMonoscript, float autoRepairTimeVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("autoRepairTime", autoRepairTimeVal);
        }

        public static bool GetRaidProtectionOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetBool("raidProtectionOn", WardIsLovePlugin._raidProtection.Value);
            return WardIsLovePlugin._raidProtection.Value;
        }

        public static void SetRaidProtectionOn(this WardMonoscript WardMonoscript, bool raidProtectionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set("raidProtectionOn", raidProtectionOnVal);
        }

        public static int GetRaidProtectionPlayerNeeded(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO()
                    .GetInt("raidablePlayersNeeded", WardIsLovePlugin._raidablePlayersNeeded.Value);
            return WardIsLovePlugin._raidablePlayersNeeded.Value;
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