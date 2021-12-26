using HarmonyLib;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    internal class HoverTextPatches
    {
        [HarmonyPatch(typeof(Sign), nameof(Sign.GetHoverText))]
        [HarmonyPrefix]
        private static void SignGetHoverTextion(Sign __instance, ref string __result)
        {
            if (!_wardEnabled.Value)
                return;
            string localize = !WardMonoscript.CheckAccess(__instance.transform.position, flash: false)
                ? "\"" + __instance.GetText() + "\""
                : "\"" + __instance.GetText() + "\"\n" +
                  Localization.instance.Localize(__instance.m_name +
                                                 "\n[<color=yellow><b>$KEY_Use</b></color>] $piece_use");
            __result = localize;
        }

        [HarmonyPatch(typeof(ItemStand), nameof(ItemStand.GetHoverText))]
        [HarmonyPrefix]
        private static void ItemStandGetHoverText(ItemStand __instance, ref string __result)
        {
            if (!_wardEnabled.Value)
                return;

            if (!Player.m_localPlayer)
                __result = "";
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
                __result = Localization.instance.Localize(__instance.m_name + "\n$piece_noaccess");
            if (__instance.HaveAttachment())
            {
                if (__instance.m_canBeRemoved)
                    __result = Localization.instance.Localize(__instance.m_name + " ( " + __instance.m_currentItemName +
                                                              " )\n[<color=yellow><b>$KEY_Use</b></color>] $piece_itemstand_take");
                if (!(__instance.m_guardianPower != null) ||
                    __instance.IsInvoking("DelayedPowerActivation") ||
                    __instance.IsGuardianPowerActive(Player.m_localPlayer))
                    __result = "";
                __result = Localization.instance.Localize("<color=orange>" + __instance.m_guardianPower.m_name +
                                                          "</color>\n" + __instance.m_guardianPower.GetTooltipString() +
                                                          "\n\n[<color=yellow><b>$KEY_Use</b></color>] $guardianstone_hook_activate");
            }

            __result = __instance.m_autoAttach && __instance.m_supportedItems.Count == 1
                ? Localization.instance.Localize(__instance.m_name +
                                                 "\n[<color=yellow><b>$KEY_Use</b></color>] $piece_itemstand_attach")
                : Localization.instance.Localize(__instance.m_name +
                                                 "\n[<color=yellow><b>1-8</b></color>] $piece_itemstand_attach");
        }

        [HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.GetHoverText))]
        [HarmonyPrefix]
        private static void Portal(TeleportWorld __instance, ref string __result)
        {
            if (!_wardEnabled.Value)
                return;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
                __result = Localization.instance.Localize(__instance.gameObject.name + "\n$piece_noaccess");
            __result = Localization.instance.Localize("$piece_portal $piece_portal_tag:\"" + __instance.GetText() +
                                                      "\"  [" +
                                                      (__instance.HaveTarget()
                                                          ? "$piece_portal_connected"
                                                          : "$piece_portal_unconnected") +
                                                      "]\n[<color=yellow><b>$KEY_Use</b></color>] $piece_portal_settag");
        }

        [HarmonyPatch(typeof(Door), nameof(Door.GetHoverText))]
        [HarmonyPostfix]
        private static void DoorGetHoverTextion(Door __instance, Humanoid character, ref string __result)
        {
            if (!_wardEnabled.Value)
                return;
            if (!__instance.m_nview.IsValid())
                __result = "";
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
                __result = Localization.instance.Localize(__instance.m_name + "\n$piece_noaccess");
            if (!__instance.CanInteract())
                __result = Localization.instance.Localize(__instance.m_name);
            __result = __instance.m_nview.GetZDO().GetInt("state") != 0
                ? Localization.instance.Localize(__instance.m_name +
                                                 "\n[<color=yellow><b>$KEY_Use</b></color>] $piece_door_close")
                : Localization.instance.Localize(__instance.m_name +
                                                 "\n[<color=yellow><b>$KEY_Use</b></color>] $piece_door_open");
        }

        [HarmonyPatch(typeof(Container), nameof(Container.GetHoverText))]
        [HarmonyPrefix]
        private static void ContainerGetHoverTextion(Container __instance, ref string __result)
        {
            if (!_wardEnabled.Value)
                return;
            __result =
                __instance.m_checkGuardStone && !WardMonoscript.CheckAccess(__instance.transform.position, flash: false)
                    ? Localization.instance.Localize(__instance.m_name + "\n$piece_noaccess")
                    : Localization.instance.Localize(
                        (__instance.m_inventory.NrOfItems() != 0
                            ? __instance.m_name
                            : __instance.m_name + " ( $piece_container_empty )") +
                        "\n[<color=yellow><b>$KEY_Use</b></color>] $piece_container_open");
        }


        [HarmonyPatch(typeof(Pickable), nameof(Pickable.GetHoverText))]
        [HarmonyPrefix]
        private static void RPC_Pick(Pickable __instance, ref string __result)
        {
            if (!_wardEnabled.Value)
                return;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
                __result = Localization.instance.Localize(__instance.gameObject.name + "\n$piece_noaccess");
            __result = __instance.m_picked
                ? ""
                : Localization.instance.Localize(__instance.GetHoverName() +
                                                 "\n[<color=yellow><b>$KEY_Use</b></color>] $inventory_pickup");
        }

        [HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.GetHoverText))]
        [HarmonyPrefix]
        private static void Pickup(ItemDrop __instance, ref string __result)
        {
            if (!_wardEnabled.Value)
                return;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
                __result = Localization.instance.Localize(__instance.gameObject.name + "\n$msg_privatezone");
            string str = __instance.m_itemData.m_shared.m_name;
            if (__instance.m_itemData.m_quality > 1)
                str = str + "[" + __instance.m_itemData.m_quality + "] ";
            if (__instance.m_itemData.m_stack > 1)
                str = str + " x" + __instance.m_itemData.m_stack;
            __result = Localization.instance.Localize(str +
                                                      "\n[<color=yellow><b>$KEY_Use</b></color>] $inventory_pickup");
        }

        [HarmonyPatch(typeof(ShipControlls), nameof(ShipControlls.GetHoverText))]
        [HarmonyPrefix]
        private static void Ship(ShipControlls __instance, ref string __result)
        {
            if (!_wardEnabled.Value)
                return;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
                __result = Localization.instance.Localize(__instance.gameObject.name + "\n$msg_privatezone");
            __result = !__instance.InUseDistance(Player.m_localPlayer)
                ? Localization.instance.Localize("<color=grey>$piece_toofar</color>")
                : Localization.instance.Localize("[<color=yellow><b>$KEY_Use</b></color>] " + __instance.m_hoverText);
        }
    }
}