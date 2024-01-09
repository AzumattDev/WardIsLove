using HarmonyLib;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch(typeof(Sign), nameof(Sign.GetHoverText))]
    static class SignGetHoverTextPatch
    {
        public static string Postfix(string __result, Sign __instance)
        {
            if (!WardEnabled.Value)
                return __result;
            string hoverText = __result;
            string localize = !WardMonoscript.CheckAccess(__instance.transform.position, flash: false)
                ? $"\"{__instance.GetText()}\""
                : $"\"{__instance.GetText()}\"\n{Localization.instance.Localize($"{__instance.m_name}\n[<color=#FFFF00><b>$KEY_Use</b></color>] $piece_use")}";
            return localize;
        }
    }

    [HarmonyPatch(typeof(ItemStand), nameof(ItemStand.GetHoverText))]
    static class ItemStandGetHoverTextPatch
    {
        static string Postfix(string __result, ItemStand __instance)
        {
            if (!WardEnabled.Value)
                return __result;
            string hoverText = __result;


            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetItemStandInteractOn()) return __result;
                hoverText = Localization.instance.Localize($"{__instance.m_name}\n<color=#FF0000>$piece_noaccess</color>");
                return hoverText;
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.GetHoverText))]
    static class TeleportWorldGetHoverTextPatch
    {
        private static string Postfix(string __result, TeleportWorld __instance)
        {
            if (!WardEnabled.Value)
                return __result;
            string hoverText = __result;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (!pa.GetNoTeleportOn()) return __result;
                hoverText = Localization.instance.Localize($"{__instance.GetHoverName()}\n<color=#FF0000>$piece_noaccess</color>");
                return hoverText;
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(Door), nameof(Door.GetHoverText))]
    static class DoorGetHoverTextPatch
    {
        private static string Postfix(string __result, Door __instance)
        {
            if (!WardEnabled.Value)
                return __result;
            string hoverText = __result;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetDoorInteractOn()) return __result;
                hoverText = Localization.instance.Localize($"{__instance.m_name}\n<color=#FF0000>$piece_noaccess</color>");
                return hoverText;
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(Container), nameof(Container.GetHoverText))]
    static class ContainerGetHoverTextPatch
    {
        private static string Postfix(string __result, Container __instance)
        {
            if (!WardEnabled.Value)
                return __result;
            string hoverText = __result;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetChestInteractOn()) return __result;
                hoverText = Localization.instance.Localize($"{__instance.GetHoverName()}\n<color=#FF0000>$piece_noaccess</color>");
                return hoverText;
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(Pickable), nameof(Pickable.GetHoverText))]
    static class PickableGetHoverTextPatch
    {
        private static string Postfix(string __result, Pickable __instance)
        {
            if (!WardEnabled.Value)
                return __result;
            string hoverText = __result;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (!pa.GetPickableInteractOn()) return __result;
                hoverText = Localization.instance.Localize($"{__instance.GetHoverName()}\n<color=#FF0000>$piece_noaccess</color>");
                return hoverText;
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(ItemDrop), nameof(ItemDrop.GetHoverText))]
    static class ItemDropGetHoverTextPatch
    {
        private static string Postfix(string __result, ItemDrop __instance)
        {
            if (!WardEnabled.Value)
                return __result;
            string hoverText = __result;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetItemInteractOn()) return __result;
                hoverText = Localization.instance.Localize($"{__instance.GetHoverName()}\n<color=#FF0000>$piece_noaccess</color>");
                return hoverText;
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(ShipControlls), nameof(ShipControlls.GetHoverText))]
    static class ShipControllsGetHoverTextPatch
    {
        private static string Postfix(string __result, ShipControlls __instance)
        {
            if (!WardEnabled.Value)
                return __result;
            string hoverText = __result;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetShipInteractOn()) return __result;
                hoverText = Localization.instance.Localize($"{__instance.GetHoverName()}\n<color=#FF0000>$piece_noaccess</color>");
                return hoverText;
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(CraftingStation), nameof(CraftingStation.GetHoverText))]
    static class CraftingStationGetHoverTextPatch
    {
        private static string Postfix(string __result, CraftingStation __instance)
        {
            if (!WardEnabled.Value)
                return __result;
            string hoverText = __result;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetCraftingStationInteractOn()) return __result;
                hoverText = Localization.instance.Localize($"{__instance.GetHoverName()}\n<color=#FF0000>$piece_noaccess</color>");
                return hoverText;
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(Smelter), nameof(Smelter.OnHoverAddOre))]
    static class SmelterOnHoverAddOrePatch
    {
        static void Postfix(Smelter __instance, ref string __result)
        {
            if (!WardEnabled.Value)
                return;
            if (!Player.m_localPlayer) return;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetSmelterInteractOn()) return;
                __result = Localization.instance.Localize($"{__instance.m_name}\n<color=#FF0000>$piece_noaccess</color>");
            }
        }
    }

    [HarmonyPatch(typeof(Smelter), nameof(Smelter.OnHoverAddFuel))]
    static class SmelterOnHoverAddFuelPatch
    {
        static void Postfix(Smelter __instance, ref string __result)
        {
            if (!WardEnabled.Value)
                return;
            if (!Player.m_localPlayer) return;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetSmelterInteractOn()) return;
                __result = Localization.instance.Localize($"{__instance.m_name}\n<color=#FF0000>$piece_noaccess</color>");
            }
        }
    }

    [HarmonyPatch(typeof(Smelter), nameof(Smelter.OnHoverEmptyOre))]
    static class SmelterOnHoverEmptyOrePatch
    {
        static void Postfix(Smelter __instance, ref string __result)
        {
            if (!WardEnabled.Value)
                return;
            if (!Player.m_localPlayer) return;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetSmelterInteractOn()) return;
                if (__instance.m_emptyOreSwitch && __instance.m_spawnStack)
                    __result = Localization.instance.Localize($"{__instance.m_name}\n<color=#FF0000>$piece_noaccess</color>");
            }
        }
    }

    [HarmonyPatch(typeof(Beehive), nameof(Beehive.GetHoverText))]
    static class BeehiveGetHoverTextPatch
    {
        private static string Postfix(string __result, Beehive __instance)
        {
            if (!WardEnabled.Value)
                return __result;
            string hoverText = __result;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetBeehiveInteractOn()) return __result;
                hoverText = Localization.instance.Localize($"{__instance.GetHoverName()}\n<color=#FF0000>$piece_noaccess</color>");
                return hoverText;
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(MapTable), nameof(MapTable.GetWriteHoverText))]
    static class MapTableGetWriteHoverTextPatch
    {
        private static string Postfix(string __result, MapTable __instance)
        {
            if (!WardEnabled.Value)
                return __result;
            string hoverText = __result;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetMapTableInteractOn()) return __result;
                hoverText = Localization.instance.Localize($"{__instance.m_name}\n<color=#FF0000>$piece_noaccess</color>");
                return hoverText;
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(MapTable), nameof(MapTable.GetReadHoverText))]
    static class MapTableGetReadHoverTextPatch
    {
        private static string Postfix(string __result, MapTable __instance)
        {
            if (!WardEnabled.Value)
                return __result;
            string hoverText = __result;
            if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false))
            {
                WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(__instance.transform.position);
                if (pa.GetMapTableInteractOn()) return __result;
                hoverText = Localization.instance.Localize($"{__instance.m_name}\n<color=#FF0000>$piece_noaccess</color>");
                return hoverText;
            }

            return __result;
        }
    }
}