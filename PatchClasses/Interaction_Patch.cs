using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    public static class Interaction_Patch
    {
        [HarmonyPatch(typeof(Sign), nameof(Sign.Interact))]
        [HarmonyPrefix]
        private static bool SignInteraction(Sign __instance, Humanoid character)
        {
            if (!_wardEnabled.Value)
                return true;
            bool flag = false;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return !flag;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!pa.GetSignInteractOn())
            {
                character.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(ItemStand), nameof(ItemStand.Interact))]
        [HarmonyPrefix]
        private static bool ItemStandInteraction(ItemStand __instance, Humanoid user)
        {
            if (!_wardEnabled.Value)
                return true;
            bool flag = false;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return !flag;
            if (__instance.m_name.Contains("guardianston") ||
                __instance.GetHoverName().ToLower().Contains("bossstone")) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!pa.GetItemStandInteractOn())
            {
                user.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.Interact))]
        [HarmonyPrefix]
        private static bool Portal(TeleportWorld __instance, Humanoid human, bool hold)
        {
            if (hold)
                return false;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, 0.0f, false, true))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (!pa.GetPortalInteractOn())
                {
                    human.Message(MessageHud.MessageType.Center, "$piece_noaccess");
                    return false;
                }

                TextInput.instance.RequestText(__instance, "$piece_portal_tag", 10);
                return true;
            }

            TextInput.instance.RequestText(__instance, "$piece_portal_tag", 10);
            return true;
        }

        [HarmonyPatch(typeof(Door), nameof(Door.Interact))]
        [HarmonyPrefix]
        private static bool DoorInteraction(Door __instance, Humanoid character, bool hold)
        {
            if (!_wardEnabled.Value)
                return true;
            if (hold)
                return false;
            bool flag = false;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return !flag;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!pa.GetDoorInteractOn())
            {
                character.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(Container), nameof(Container.Interact))]
        [HarmonyPrefix]
        private static bool ContainerInteraction(Container __instance, Humanoid character, bool hold)
        {
            if (!_wardEnabled.Value)
                return true;
            if (hold)
                return false;
            bool flag = false;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return !flag;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!pa.GetChestInteractOn())
            {
                character.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }


        [HarmonyPatch(typeof(Pickable), nameof(Pickable.Interact))]
        [HarmonyPrefix]
        private static bool RPC_Pick(Pickable __instance, ref bool repeat)
        {
            if (!_wardEnabled.Value)
                return true;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!pa.GetPickableInteractOn())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }

        /* Meant for pickable protection by request */
        [HarmonyPatch(typeof(Destructible), nameof(Destructible.Damage))]
        public class PickableProtection_WardIsLove
        {
            public static void Prefix(Destructible __instance, HitData hit)
            {
                if (!_wardEnabled.Value)
                    return;
                if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position)) return;
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetPickableInteractOn()) return;
                if (!__instance.gameObject.GetComponent<Pickable>()) return;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                hit.ApplyModifier(0);
            }
        }

        [HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.Interact))]
        [HarmonyPrefix]
        private static bool Pickup(ItemDrop __instance)
        {
            if (!_wardEnabled.Value)
                return true;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!pa.GetItemInteractOn())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(ShipControlls), nameof(ShipControlls.Interact))]
        [HarmonyPrefix]
        private static bool Ship(ShipControlls __instance)
        {
            if (!_wardEnabled.Value)
                return true;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.GetControlledComponent().transform.position) ||
                CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.GetControlledComponent().transform.position,
                    flash: false)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!pa.GetShipInteractOn())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return false;
        }

        [HarmonyPatch(typeof(CraftingStation), nameof(CraftingStation.Interact))]
        [HarmonyPrefix]
        private static bool CraftingStation_InteractionCheck(CraftingStation __instance)
        {
            if (!_wardEnabled.Value)
                return true;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);


            if (!pa.GetCraftingStationInteractOn())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(Smelter), nameof(Smelter.OnAddOre))]
        [HarmonyPrefix]
        private static bool SmeltingStation_InteractionCheck(Smelter __instance)
        {
            if (!_wardEnabled.Value)
                return true;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);

            if (!pa.GetSmelterInteractOn())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(Smelter), nameof(Smelter.OnAddFuel))]
        [HarmonyPrefix]
        private static bool SmeltingStation_InteractionCheck2(Smelter __instance)
        {
            if (!_wardEnabled.Value)
                return true;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!pa.GetSmelterInteractOn())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(Beehive), nameof(Beehive.Interact))]
        [HarmonyPrefix]
        private static bool Beehive_InteractionCheck(Beehive __instance)
        {
            if (!_wardEnabled.Value)
                return true;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!pa.GetBeehiveInteractOn())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(MapTable), nameof(MapTable.OnWrite))]
        [HarmonyPrefix]
        private static bool MapTable_InteractionCheck(MapTable __instance)
        {
            if (!_wardEnabled.Value)
                return true;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!pa.GetMapTableInteractOn())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(MapTable), nameof(MapTable.OnRead))]
        [HarmonyPrefix]
        private static bool MapTable_InteractionCheckOnRead(MapTable __instance)
        {
            if (!_wardEnabled.Value)
                return true;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (!pa.GetMapTableInteractOn())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }
    }
}