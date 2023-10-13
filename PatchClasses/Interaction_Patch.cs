using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch(typeof(Sign), nameof(Sign.Interact))]
    static class SignInteractPatch
    {
        static bool Prefix(Sign __instance, Humanoid character)
        {
            if (!WardEnabled.Value)
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
    }

    [HarmonyPatch(typeof(ItemStand), nameof(ItemStand.Interact))]
    static class ItemStandInteractPatch
    {
        static bool Prefix(ItemStand __instance, Humanoid user)
        {
            if (!WardEnabled.Value)
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
    }

    [HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.Interact))]
    static class TeleportWorldInteractPatch
    {
        static bool Prefix(TeleportWorld __instance, Humanoid human, bool hold)
        {
            if (hold)
                return false;
            if (WardMonoscript.CheckAccess(__instance.transform.position, 0.0f, false, true)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (pa.GetPortalInteractOn()) return true;
            human.Message(MessageHud.MessageType.Center, "$piece_noaccess");
            return false;
        }
    }

    [HarmonyPatch(typeof(Door), nameof(Door.Interact))]
    static class DoorInteractPatch
    {
        static bool Prefix(Door __instance, Humanoid character, bool hold)
        {
            if (!WardEnabled.Value)
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
    }

    [HarmonyPatch(typeof(Container), nameof(Container.Interact))]
    static class ContainerInteractPatch
    {
        static bool Prefix(Container __instance, Humanoid character, bool hold)
        {
            if (!WardEnabled.Value)
                return true;
            if (hold)
                return false;
            bool flag = false;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return !flag;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
            if (pa.GetChestInteractOn()) return true;
            if (__instance.m_piece.m_name.Contains("yuleklapp"))
            {
                return true;
            }

            character.Message(MessageHud.MessageType.Center, "$msg_privatezone");
            return false;
        }
    }

    [HarmonyPatch(typeof(Pickable), nameof(Pickable.Interact))]
    static class PickableInteractPatch
    {
        static bool Prefix(Pickable __instance, ref bool repeat)
        {
            if (!WardEnabled.Value)
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
    }

    /* Meant for pickable protection by request */
    [HarmonyPatch(typeof(Destructible), nameof(Destructible.Damage))]
    static class DestructibleDamagePatch
    {
        static void Prefix(Destructible __instance, HitData hit)
        {
            if (!WardEnabled.Value)
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
    static class ItemDropInteractPatch
    {
        static bool Prefix(ItemDrop __instance)
        {
            if (!WardEnabled.Value)
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
    }

    [HarmonyPatch(typeof(ShipControlls), nameof(ShipControlls.Interact))]
    static class ShipControllsInteractPatch
    {
        static bool Prefix(ShipControlls __instance)
        {
            if (!WardEnabled.Value)
                return true;


            if (!WardMonoscript.CheckInWardMonoscript(__instance.m_attachPoint.position) ||
                CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.m_attachPoint.position,
                    flash: false)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.m_attachPoint.position);
            if (!pa.GetShipInteractOn())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(ArmorStand), nameof(ArmorStand.UseItem))]
    static class ArmorStandUseItemPatch
    {
        static bool Prefix(ArmorStand __instance, Switch caller, Humanoid user, ItemDrop.ItemData item)
        {
            if (!WardEnabled.Value)
                return true;
            if (!WardMonoscript.CheckInWardMonoscript(__instance.transform.position) || CustomCheck.CheckAccess(
                    Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                    flash: false)) return true;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);


            if (!pa.GetItemStandInteractOn())
            {
                MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CraftingStation), nameof(CraftingStation.Interact))]
    static class CraftingStationInteractPatch
    {
        static bool Prefix(CraftingStation __instance)
        {
            if (!WardEnabled.Value)
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
    }

    [HarmonyPatch(typeof(Smelter), nameof(Smelter.OnAddOre))]
    static class SmelterOnAddOrePatch
    {
        static bool Prefix(Smelter __instance)
        {
            if (!WardEnabled.Value)
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
    }

    [HarmonyPatch(typeof(Smelter), nameof(Smelter.OnAddFuel))]
    static class SmelterOnAddFuelPatch
    {
        static bool Prefix(Smelter __instance)
        {
            if (!WardEnabled.Value)
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
    }

    [HarmonyPatch(typeof(Beehive), nameof(Beehive.Interact))]
    static class BeehiveInteractPatch
    {
        static bool Prefix(Beehive __instance)
        {
            if (!WardEnabled.Value)
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
    }

    [HarmonyPatch(typeof(MapTable), nameof(MapTable.OnWrite))]
    static class MapTableOnWritePatch
    {
        static bool Prefix(MapTable __instance)
        {
            if (!WardEnabled.Value)
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

    [HarmonyPatch(typeof(MapTable), nameof(MapTable.OnRead))]
    static class MapTableOnReadPatch
    {
        static bool Prefix(MapTable __instance)
        {
            if (!WardEnabled.Value)
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