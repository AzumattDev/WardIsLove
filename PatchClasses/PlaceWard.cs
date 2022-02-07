using HarmonyLib;
using UnityEngine;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    public class PlaceWard
    {
        [HarmonyPatch(typeof(Player), nameof(Player.GetPiece))]
        [HarmonyPostfix]
        private static void GetPiece(ref Player __instance, ref Piece __result)
        {
            FinalInit(ZNetScene.instance);
            RecipeFunction.UpdateWardRecipe(ref __result);
            if (__result != null)
                WILLogger.LogDebug(__result.m_name);
        }


        [HarmonyPatch(typeof(Player), nameof(Player.GetSelectedPiece))]
        [HarmonyPostfix]
        private static void GetSPiece(ref Player __instance, ref Piece __result)
        {
            FinalInit(ZNetScene.instance);
            RecipeFunction.UpdateWardRecipe(ref __result);
        }

        [HarmonyPatch(typeof(Piece), nameof(Piece.Awake))]
        public static class OverwritePieceResourcesOnAwake
        {
            private static void Prefix(ref Piece __instance)
            {
                if (!__instance.name.ToLower().Contains("planned") &&
                    !__instance.m_name.ToLower().Contains("planned") ||
                    !__instance.m_name.ToLower().Contains("thorwar")) return;
                Piece? planned = __instance.gameObject.GetComponent<Piece>();
                foreach (Piece.Requirement? req in __instance.m_resources) req.m_recover = false;
            }

            private static void Postfix(ref Piece __instance)
            {
                FinalInit(ZNetScene.instance);
                RecipeFunction.UpdateWardRecipe(ref __instance);
                if (!__instance.name.ToLower().Contains("planned") &&
                    !__instance.m_name.ToLower().Contains("planned") ||
                    !__instance.m_name.ToLower().Contains("thorwar")) return;
                Piece? planned = __instance.gameObject.GetComponent<Piece>();
                foreach (Piece.Requirement? req in __instance.m_resources) req.m_recover = false;
            }
        }

        [HarmonyPatch(typeof(WardMonoscript), nameof(WardMonoscript.OnDestroy))]
        public static class OnDestroyBwPatch
        {
            private static void Prefix(ref WardMonoscript __instance)
            {
                if (!__instance.name.ToLower().Contains("planned") &&
                    !__instance.m_name.ToLower().Contains("planned") ||
                    !__instance.m_name.ToLower().Contains("thorwar")) return;
                Piece? planned = __instance.gameObject.GetComponent<Piece>();
                foreach (Piece.Requirement? req in planned.m_resources) req.m_recover = false;
            }
        }

        /* Patch Player repair to prevent repairing outside the bubble */

        [HarmonyPatch(typeof(Player), nameof(Player.Repair))]
        static class Player_Repair_Patch
        {
            static bool Prefix(Player __instance, ItemDrop.ItemData toolItem, Piece repairPiece)
            {
                if (!__instance.InPlaceMode()) return false;
                Piece hoveringPiece = __instance.GetHoveringPiece();
                if (!(bool)(Object)hoveringPiece || !__instance.CheckCanRemovePiece(hoveringPiece) ||
                    !WardMonoscript.CheckAccess(hoveringPiece.transform.position))
                    return false;
                if (hoveringPiece.GetComponentInParent<WardMonoscript>())
                {
                    if (Vector3.Distance(Player.m_localPlayer.transform.position,
                            hoveringPiece.GetComponentInParent<WardMonoscript>().transform.position) <= 5)
                    {
                        return true;
                    }

                    return false;
                }

                return true;
            }
        }
    }
}