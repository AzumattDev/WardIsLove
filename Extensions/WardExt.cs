using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WardIsLove.Util;
using WardIsLove.Util.RPCShit;

namespace WardIsLove.Extensions
{
    public static class WardMonoscriptExt
    {
        public static IEnumerable<WardMonoscript> WardMonoscriptsINSIDE = null!;
        public static IEnumerable<WardMonoscript> WardCharacterINSIDE = null!;

        public static IEnumerator UpdateAreas()
        {
            while (true)
            {
                try
                {
                    if (Player.m_localPlayer && WardMonoscript.m_allAreas != null)
                        WardMonoscriptsINSIDE = WardMonoscript.m_allAreas.Where(p => p != null &&
                                                                                     Vector3.Distance(Player.m_localPlayer.transform.position, p.transform.position) <=
                                                                                     p.m_nview.m_zdo.GetFloat(ZdoInternalExtensions.wardRadius, WardIsLovePlugin.WardRange.Value));

                    if (WardMonoscript.m_allAreas != null)
                        try
                        {
                            List<Character>? C = Character.GetAllCharacters();
                            foreach (Character? character in C)
                                WardCharacterINSIDE = WardMonoscript.m_allAreas.Where(p => p != null &&
                                                                                           Vector3.Distance(character.transform.position, p.transform.position) <=
                                                                                           p.m_nview.m_zdo.GetFloat(ZdoInternalExtensions.wardRadius, WardIsLovePlugin.WardRange.Value));
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

            return null!;
        }

        public static WardIsLovePlugin.WardInteractBehaviorEnums GetAccessMode(
            this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return (WardIsLovePlugin.WardInteractBehaviorEnums)WardMonoscript.m_nview.m_zdo.GetInt(ZdoInternalExtensions.accessMode);

            return WardIsLovePlugin.WardInteractBehaviorEnums.OwnerOnly;
        }

        public static void SetAccessMode(this WardMonoscript WardMonoscript,
            WardIsLovePlugin.WardInteractBehaviorEnums accessMode)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                WardMonoscript.m_nview.m_zdo.Set(ZdoInternalExtensions.accessMode, (int)accessMode);
            //MonoBehaviour.print((int)accessMode);
        }

        public static WardIsLovePlugin.WardBehaviorEnums GetBubbleMode(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return (WardIsLovePlugin.WardBehaviorEnums)WardMonoscript.m_nview.m_zdo.GetInt(ZdoInternalExtensions.bubbleMode);

            return WardIsLovePlugin.WardBehaviorEnums.Default;
        }

        public static void SetBubbleMode(this WardMonoscript WardMonoscript,
            WardIsLovePlugin.WardBehaviorEnums bubbleMode)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                WardMonoscript.m_nview.m_zdo.Set(ZdoInternalExtensions.bubbleMode, (int)bubbleMode);
        }

        public static HitData.DamageType GetDamageType(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return (HitData.DamageType)WardMonoscript.m_nview.m_zdo.GetInt(ZdoInternalExtensions.damageType);

            return HitData.DamageType.Lightning;
        }


        public static void SetDamageType(this WardMonoscript WardMonoscript,
            WardIsLovePlugin.WardDamageTypes damageType)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                WardMonoscript.m_nview.m_zdo.Set(ZdoInternalExtensions.damageType, (int)damageType);
        }


        public static float GetWardRadius(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetFloat(ZdoInternalExtensions.wardRadius, WardIsLovePlugin.WardRange.Value);
            return WardIsLovePlugin.WardRange.Value;
        }

        public static void SetWardRadius(this WardMonoscript WardMonoscript, float wardRadiusSliderVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.wardRadius, wardRadiusSliderVal);
        }

        public static float GetHealthBoost(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetFloat(ZdoInternalExtensions.healthRegen, WardIsLovePlugin.WardPassiveHealthRegen.Value);
            return WardIsLovePlugin.WardPassiveHealthRegen.Value;
        }

        public static void SetHealthBoost(this WardMonoscript WardMonoscript, float healthRegenSliderVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.healthRegen, healthRegenSliderVal);
        }

        public static float GetStaminaBoost(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetFloat(ZdoInternalExtensions.staminaBoost, WardIsLovePlugin.WardPassiveStaminaRegen.Value);
            return WardIsLovePlugin.WardPassiveStaminaRegen.Value;
        }

        public static void SetStaminaBoost(this WardMonoscript WardMonoscript, float staminaBoostSliderVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.staminaBoost, staminaBoostSliderVal);
        }

        public static bool GetBubbleOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.bubbleOn, WardIsLovePlugin.EnableBubble.Value);
            return WardIsLovePlugin.EnableBubble.Value;
        }

        public static void SetBubbleOn(this WardMonoscript WardMonoscript, bool bubbleOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.bubbleOn, bubbleOnVal);
        }

        public static bool GetWeatherDmgOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.weatherDmgOn, WardIsLovePlugin.NoWeatherDmg.Value);
            return WardIsLovePlugin.NoWeatherDmg.Value;
        }

        public static void SetWeatherDmgOn(this WardMonoscript WardMonoscript, bool weatherDmgOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.weatherDmgOn, weatherDmgOnVal);
        }

        public static bool GetAutoPickupOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.autoPickupOn, WardIsLovePlugin.DisablePickup.Value);
            return WardIsLovePlugin.DisablePickup.Value;
        }

        public static void SetAutoPickupOnOn(this WardMonoscript WardMonoscript, bool autoPickupOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.autoPickupOn, autoPickupOnVal);
        }

        public static bool GetAutoCloseDoorsOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.autoCloseDoorsOn, WardIsLovePlugin.AutoClose.Value);
            return WardIsLovePlugin.AutoClose.Value;
        }

        public static void SetAutoCloseDoorsOn(this WardMonoscript WardMonoscript, bool autoCloseDoorsOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.autoCloseDoorsOn, autoCloseDoorsOnVal);
        }

        public static bool GetFireplaceUnlimOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.fireplaceUnlimOn, WardIsLovePlugin.FireplaceUnlimited.Value);
            return WardIsLovePlugin.FireplaceUnlimited.Value;
        }

        public static void SetFireplaceUnlimOn(this WardMonoscript WardMonoscript, bool fireplaceUnlimOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.fireplaceUnlimOn, fireplaceUnlimOnVal);
        }

        public static bool GetBathingUnlimOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.bathingUnlimOn, WardIsLovePlugin.BathingUnlimited.Value);
            return WardIsLovePlugin.BathingUnlimited.Value;
        }

        public static void SetBathingUnlimOn(this WardMonoscript WardMonoscript, bool bathingUnlimOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.bathingUnlimOn, bathingUnlimOnVal);
        }

        public static bool GetCookingUnlimOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.cookingUnlimOn, WardIsLovePlugin.CookingUnlimited.Value);
            return WardIsLovePlugin.CookingUnlimited.Value;
        }

        public static void SetCookingUnlimOn(this WardMonoscript WardMonoscript, bool cookingUnlimOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.cookingUnlimOn, cookingUnlimOnVal);
        }

        public static string GetFireplaceList(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetString(ZdoInternalExtensions.wardFireplaceList, WardIsLovePlugin.FireSources.Value);
            return WardIsLovePlugin.FireSources.Value;
        }

        public static void SetFireplaceList(this WardMonoscript WardMonoscript, string wardFireplaceListVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.wardFireplaceList, wardFireplaceListVal);
        }

        public static bool GetNoDeathPenOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.noDeathPenOn, WardIsLovePlugin.WardNoDeathPen.Value);
            return WardIsLovePlugin.WardNoDeathPen.Value;
        }

        public static void SetNoDeathPenOn(this WardMonoscript WardMonoscript, bool noDeathPenOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.noDeathPenOn, noDeathPenOnVal);
        }

        public static bool GetNoFoodDrainOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.noFoodDrainOn, WardIsLovePlugin.NoFoodDrain.Value);
            return WardIsLovePlugin.NoFoodDrain.Value;
        }

        public static void SetNoFoodDrainOn(this WardMonoscript WardMonoscript, bool noFoodDrainOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.noFoodDrainOn, noFoodDrainOnVal);
        }

        public static bool GetPushoutPlayersOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.pushoutPlayersOn, WardIsLovePlugin.PushoutPlayers.Value);
            return WardIsLovePlugin.PushoutPlayers.Value;
        }

        public static void SetPushoutPlayersOn(this WardMonoscript WardMonoscript, bool pushoutPlayersOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.pushoutPlayersOn, pushoutPlayersOnVal);
        }

        public static bool GetPushoutCreaturesOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.pushoutCreaturesOn, WardIsLovePlugin.PushoutCreatures.Value);
            return WardIsLovePlugin.PushoutCreatures.Value;
        }

        public static void SetPushoutCreaturesOn(this WardMonoscript WardMonoscript, bool pushoutCreaturesOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.pushoutCreaturesOn, pushoutCreaturesOnVal);
        }

        public static bool GetPvpOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.pvpOn, WardIsLovePlugin.WardPvP.Value);
            return WardIsLovePlugin.WardPvP.Value;
        }

        public static void SetPvpOn(this WardMonoscript WardMonoscript, bool pvpOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.pvpOn, pvpOnVal);
        }

        public static bool GetPveOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.pveOn, WardIsLovePlugin.WardPve.Value);
            return WardIsLovePlugin.WardPve.Value;
        }

        public static void SetPveOn(this WardMonoscript WardMonoscript, bool PveOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.pveOn, PveOnVal);
        }

        public static bool GetNoTeleportOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.teleportOn, WardIsLovePlugin.NoTeleport.Value);
            return WardIsLovePlugin.NoTeleport.Value;
        }

        public static void SetNoTeleportOn(this WardMonoscript WardMonoscript, bool noteleportOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.teleportOn, noteleportOnVal);
        }

        public static bool GetShowFlashOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.showFlashOn, WardIsLovePlugin.ShowFlash.Value);
            return WardIsLovePlugin.ShowFlash.Value;
        }

        public static void SetShowFlashOn(this WardMonoscript WardMonoscript, bool showFlashOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.showFlashOn, showFlashOnVal);
        }

        public static bool GetShowMarkerOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.showMarkerOn, WardIsLovePlugin.ShowMarker.Value);
            return WardIsLovePlugin.ShowMarker.Value;
        }

        public static void SetShowMarkerOn(this WardMonoscript WardMonoscript, bool showMarkerOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.showMarkerOn, showMarkerOnVal);
        }

        public static bool GetWardNotificationsOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.wardNotificationsOn, WardIsLovePlugin.WardNotify.Value);
            return WardIsLovePlugin.WardNotify.Value;
        }

        public static void SetWardNotificationsOn(this WardMonoscript WardMonoscript,
            bool wardNotificationsOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.wardNotificationsOn, wardNotificationsOnVal);
        }

        public static string GetWardEnterNotifyMessage(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetString(ZdoInternalExtensions.wardNotifyMessageEntry, WardIsLovePlugin.WardNotifyMessageEntry.Value);
            return WardIsLovePlugin.WardNotifyMessageEntry.Value;
        }

        public static void SetWardEnterNotifyMessage(this WardMonoscript WardMonoscript,
            string wardNotifyMessageEntryVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.wardNotifyMessageEntry, wardNotifyMessageEntryVal);
        }

        public static string GetWardExitNotifyMessage(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetString(ZdoInternalExtensions.wardNotifyMessageExit, WardIsLovePlugin.WardNotifyMessageExit.Value);
            return WardIsLovePlugin.WardNotifyMessageExit.Value;
        }

        public static void SetWardExitNotifyMessage(this WardMonoscript WardMonoscript,
            string wardNotifyMessageExitVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.wardNotifyMessageExit, wardNotifyMessageExitVal);
        }

        public static float GetWardDamageAmount(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetFloat(ZdoInternalExtensions.wardDamageAmount, WardIsLovePlugin.WardDamageAmount.Value);
            return WardIsLovePlugin.WardDamageAmount.Value;
        }

        public static void SetWardDamageAmount(this WardMonoscript WardMonoscript, float wardDamageAmountVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.wardDamageAmount, wardDamageAmountVal);
        }

        public static bool GetItemStandInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.itemstandInteractOn, WardIsLovePlugin.ItemStandInteraction.Value);
            return WardIsLovePlugin.ItemStandInteraction.Value;
        }

        public static void SetItemStandInteractOn(this WardMonoscript WardMonoscript,
            bool itemStandInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.itemstandInteractOn, itemStandInteractionOnVal);
        }

        public static bool GetIndestructibleOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.indestructibleOn, WardIsLovePlugin.WardStructures.Value);
            return WardIsLovePlugin.WardStructures.Value;
        }

        public static void SetIndestructibleOn(this WardMonoscript WardMonoscript, bool indestructibleOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.indestructibleOn, indestructibleOnVal);
        }

        public static string GetIndestructList(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetString(ZdoInternalExtensions.indestructList, WardIsLovePlugin.ItemStructureNames.Value);
            return WardIsLovePlugin.ItemStructureNames.Value;
        }

        public static void SetIndestructList(this WardMonoscript WardMonoscript, string indestructListVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.indestructList, indestructListVal);
        }

        public static float GetCreatureDamageIncrease(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetFloat(ZdoInternalExtensions.CreatDamageIncreaseAmount, WardIsLovePlugin.WardDamageIncrease.Value);
            return WardIsLovePlugin.WardDamageIncrease.Value;
        }

        public static void SetCreatureDamageIncrease(this WardMonoscript WardMonoscript,
            float creatureDamageIncreaseAmountVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.CreatDamageIncreaseAmount, creatureDamageIncreaseAmountVal);
        }

        public static float GetStructDamageReduc(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetFloat(ZdoInternalExtensions.structDamageReducAmount, WardIsLovePlugin.WardDamageReduction.Value);
            return WardIsLovePlugin.WardDamageReduction.Value;
        }

        public static void SetStructDamageReduc(this WardMonoscript WardMonoscript,
            float structDamageReducAmountVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.structDamageReducAmount, structDamageReducAmountVal);
        }

        public static bool GetItemInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.itemInteractOn, WardIsLovePlugin.ItemInteraction.Value);
            return WardIsLovePlugin.ItemInteraction.Value;
        }

        public static void SetItemInteractOn(this WardMonoscript WardMonoscript, bool itemInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.itemInteractOn, itemInteractionOnVal);
        }

        public static bool GetDoorInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.doorInteractOn, WardIsLovePlugin.DoorInteraction.Value);
            return WardIsLovePlugin.DoorInteraction.Value;
        }

        public static void SetDoorInteractOn(this WardMonoscript WardMonoscript, bool doorInteractOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.doorInteractOn, doorInteractOnVal);
        }

        public static bool GetChestInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.chestInteractOn, WardIsLovePlugin.ChestInteraction.Value);
            return WardIsLovePlugin.ChestInteraction.Value;
        }

        public static void SetChestInteractOn(this WardMonoscript WardMonoscript, bool chestInteractOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.chestInteractOn, chestInteractOnVal);
        }

        public static bool GetPortalInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.portalInteractOn, WardIsLovePlugin.PortalInteraction.Value);
            return WardIsLovePlugin.PortalInteraction.Value;
        }

        public static void SetPortalInteractOn(this WardMonoscript WardMonoscript, bool portalInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.portalInteractOn, portalInteractionOnVal);
        }

        public static bool GetPickableInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.pickableInteractOn, WardIsLovePlugin.PickableInteraction.Value);
            return WardIsLovePlugin.PickableInteraction.Value;
        }

        public static void SetPickableInteractOn(this WardMonoscript WardMonoscript, bool pickableInteractOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.pickableInteractOn, pickableInteractOnVal);
        }

        public static bool GetShipInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.shipInteractOn, WardIsLovePlugin.ShipInteraction.Value);
            return WardIsLovePlugin.ShipInteraction.Value;
        }

        public static void SetShipInteractOn(this WardMonoscript WardMonoscript, bool shipInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.shipInteractOn, shipInteractionOnVal);
        }

        public static bool GetSignInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.signInteractOn, WardIsLovePlugin.SignInteraction.Value);
            return WardIsLovePlugin.SignInteraction.Value;
        }

        public static void SetSignInteractOn(this WardMonoscript WardMonoscript, bool signInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.signInteractOn, signInteractionOnVal);
        }

        public static bool GetCraftingStationInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.craftingStationInteractOn, WardIsLovePlugin.CraftingStationInteraction.Value);
            return WardIsLovePlugin.CraftingStationInteraction.Value;
        }

        public static void SetCraftingStationInteractOn(this WardMonoscript WardMonoscript,
            bool craftingStationInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.craftingStationInteractOn, craftingStationInteractionOnVal);
        }

        public static bool GetSmelterInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.smelterInteractOn, WardIsLovePlugin.SmelterInteraction.Value);
            return WardIsLovePlugin.SmelterInteraction.Value;
        }

        public static void SetSmelterInteractOn(this WardMonoscript WardMonoscript, bool smelterInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.smelterInteractOn, smelterInteractionOnVal);
        }

        public static bool GetBeehiveInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.beehiveInteractOn, WardIsLovePlugin.BeehiveInteraction.Value);
            return WardIsLovePlugin.BeehiveInteraction.Value;
        }

        public static void SetBeehiveInteractOn(this WardMonoscript WardMonoscript, bool beehiveInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.beehiveInteractOn, beehiveInteractionOnVal);
        }

        public static bool GetMapTableInteractOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.maptableInteractOn, WardIsLovePlugin.MaptableInteraction.Value);
            return WardIsLovePlugin.MaptableInteraction.Value;
        }

        public static void SetMapTableInteractOn(this WardMonoscript WardMonoscript, bool maptableInteractionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.maptableInteractOn, maptableInteractionOnVal);
        }

        public static bool GetOnlyPermOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.OnlyPermOn, WardIsLovePlugin.WardOnlyPerm.Value);
            return WardIsLovePlugin.WardOnlyPerm.Value;
        }

        public static void SetOnlyPermOn(this WardMonoscript WardMonoscript, bool onlypermOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.OnlyPermOn, onlypermOnVal);
        }

        public static bool GetNotPermOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.NotPermOn, WardIsLovePlugin.WardNotPerm.Value);
            return WardIsLovePlugin.WardNotPerm.Value;
        }

        public static void SetNotPermOn(this WardMonoscript WardMonoscript, bool NotpermOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.NotPermOn, NotpermOnVal);
        }

        public static string GetCtaMessage(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetString(ZdoInternalExtensions.ctaMessage, WardIsLovePlugin.CtaMessage.Value);
            return WardIsLovePlugin.CtaMessage.Value;
        }

        public static void SetCtaMessage(this WardMonoscript WardMonoscript, string ctaMessageVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.ctaMessage, ctaMessageVal);
        }

        public static bool GetAutoRepairOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.autoRepairOn, WardIsLovePlugin.AutoRepair.Value);
            return WardIsLovePlugin.AutoRepair.Value;
        }

        public static void SetAutoRepairOn(this WardMonoscript WardMonoscript, bool autorepairOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.autoRepairOn, autorepairOnVal);
        }

        public static float GetAutoRepairAmount(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetFloat(ZdoInternalExtensions.autoRepairAmount, WardIsLovePlugin.AutoRepairAmount.Value);
            return WardIsLovePlugin.AutoRepairAmount.Value;
        }

        public static void SetAutoRepairAmount(this WardMonoscript WardMonoscript, float autorepairOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.autoRepairAmount, autorepairOnVal);
        }

        public static float GetAutoRepairTextTime(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetFloat(ZdoInternalExtensions.autoRepairTime, WardIsLovePlugin.AutoRepairTime.Value);
            return WardIsLovePlugin.AutoRepairTime.Value;
        }

        public static void SetAutoRepairTextTime(this WardMonoscript WardMonoscript, float autoRepairTimeVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.autoRepairTime, autoRepairTimeVal);
        }

        public static bool GetRaidProtectionOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.raidProtectionOn, WardIsLovePlugin.RaidProtection.Value);
            return WardIsLovePlugin.RaidProtection.Value;
        }

        public static void SetRaidProtectionOn(this WardMonoscript WardMonoscript, bool raidProtectionOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.raidProtectionOn, raidProtectionOnVal);
        }

        public static int GetRaidProtectionPlayerNeeded(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetInt(ZdoInternalExtensions.raidablePlayersNeeded, WardIsLovePlugin.RaidablePlayersNeeded.Value);
            return WardIsLovePlugin.RaidablePlayersNeeded.Value;
        }

        public static void SetRaidProtectionPlayerNeeded(this WardMonoscript WardMonoscript,
            int raidablePlayersNeededVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.raidablePlayersNeeded, raidablePlayersNeededVal);
        }

        public static bool GetWardIsLoveOn(this WardMonoscript WardMonoscript)
        {
            if (WardMonoscript.m_nview && WardMonoscript.m_nview.m_zdo != null)
                return WardMonoscript.m_nview.GetZDO().GetBool(ZdoInternalExtensions.wardIsLoveOn);
            return false;
        }

        public static void SetWardIsLoveOn(this WardMonoscript WardMonoscript, bool wardIsLoveOnVal)
        {
            WardMonoscript.m_nview.GetZDO().Set(ZdoInternalExtensions.wardIsLoveOn, wardIsLoveOnVal);
        }

        public static bool WILWardLimitCheck(this WardMonoscript area)
        {
            if (area.m_nview.IsValid() && area.m_nview.m_zdo.GetBool(ZdoInternalExtensions.WILLimitedWard))
            {
                ServerTimeRPCs.RequestServerTimeIfNeeded();
                int currentUnixTimeDefault = (int)WardIsLovePlugin.serverDateTimeOffset.ToUnixTimeSeconds();
                int storedUnixTime = area.m_nview.m_zdo.GetInt(ZdoInternalExtensions.WILLimitedWardTime, currentUnixTimeDefault);

                // Check for admin ward that should never expire
                if (storedUnixTime == -1)
                {
                    return true;
                }

                DateTime storedDateTime = DateTimeOffset.FromUnixTimeSeconds(storedUnixTime).DateTime;

                // Calculate the difference in days
                TimeSpan timeDifference = WardIsLovePlugin.serverTime - storedDateTime;
                int daysDifference = (int)Math.Floor(timeDifference.TotalDays);

                return daysDifference < WardIsLovePlugin.MaxDaysDifference;
            }

            return false;
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