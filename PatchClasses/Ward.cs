using System.Linq;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    internal class Ward
    {
        private static void WardRangeEffect(Component parent, EffectArea.Type includedTypes, float newRadius)
        {
            if (parent == null) return;
            EffectArea effectArea = parent.GetComponentInChildren<EffectArea>();
            if (effectArea == null) return;
            if ((effectArea.m_type & includedTypes) == 0) return;
            SphereCollider collision = effectArea.GetComponent<SphereCollider>();
            //WardIsLovePlugin.WILLogger.LogError(collision.transform.name);
            if (collision != null) collision.radius = newRadius;
        }

        [HarmonyPatch(typeof(WardMonoscript), nameof(WardMonoscript.RPC_ToggleEnabled))]
        public class HarmonyPatch_RPC_ToggleEnabled
        {
            // Rewrite of the original patch to allow permitted players to toggle the ward on and off. The original way that I did this prevented some users from "holding the interact key"s
            [HarmonyPrefix]
            public static bool RPC_ToggleEnabled(long uid, long playerID, WardMonoscript __instance)
            {
                // If ward interaction should be the original behavior, then just return true and run the original code.
                if (__instance.GetAccessMode() == WardInteractBehaviorEnums.Default) return true;
#if DEBUG
                WILLogger.LogDebug("Toggle enabled from " + playerID + "  creator is " +
                                   __instance.m_piece.GetCreator());
#endif
                if (__instance.m_nview.IsOwner() &&
                    (__instance.IsPermitted(playerID) &&
                     __instance.GetAccessMode() == WardInteractBehaviorEnums.Default ||
                     __instance.GetAccessMode() == WardInteractBehaviorEnums.Everyone ||
                     __instance.m_piece.IsCreator()))
                    __instance.SetEnabled(!__instance.IsEnabled());

                return false;
            }
        }

        // Show area marker patch. Separated this out due to issues of it not showing when put in player update patch above.
        // Also gives more control
        [HarmonyPatch]
        public static class ShowAreaMarker
        {
            [HarmonyPatch(typeof(Player), nameof(Player.Update))]
            [HarmonyPostfix]
            private static void Postfix(ref Player __instance)
            {
                if (__instance.InPlaceMode() || !Player.m_localPlayer)
                    return;
                if (!WardMonoscript.CheckInWardMonoscript(Player.m_localPlayer.transform.position) ||
                    !Game.instance.isActiveAndEnabled || !_wardEnabled.Value) return;
                foreach (WardMonoscript? allArea in WardMonoscript.m_allAreas.Where(allArea => allArea.IsEnabled() &&
                             allArea.IsInside(Player.m_localPlayer.transform.position, 0.0f) &&
                             allArea.GetShowMarkerOn()))
                    allArea.ShowAreaMarker();
            }
        }

        [HarmonyPatch(typeof(WardMonoscript), nameof(WardMonoscript.IsInside))]
        private static class IsInside_Patch
        {
            private static void Prefix(WardMonoscript __instance, ref float ___m_radius)
            {
                foreach (WardMonoscript? pa in WardMonoscript.m_allAreas)
                {
                    pa.m_areaMarker.m_radius = pa.GetWardRadius();
                    pa.m_radiusNMA = pa.GetWardRadius();
                    pa.m_radiusBurning = pa.GetWardRadius();
                    pa.m_playerBase.GetComponent<SphereCollider>().radius = pa.GetWardRadius();
                    if ((bool)(Object)__instance.m_areaMarker)
                        __instance.m_areaMarker.m_radius = __instance.GetWardRadius();
                    __instance.m_enabledNMAEffect.GetComponent<SphereCollider>().radius =
                        __instance.GetWardRadius();
                    __instance.m_enabledBurningEffect.GetComponent<SphereCollider>().radius =
                        __instance.GetWardRadius();
                    WardRangeEffect(__instance, EffectArea.Type.PlayerBase, __instance.GetWardRadius());
                    ___m_radius = pa.GetWardRadius();
                }
            }
        }

        [HarmonyPatch]
        private static class OverlapCheck
        {
            [HarmonyPatch(typeof(Player), nameof(Player.RemovePiece))]
            [HarmonyPrefix]
            private static bool Prefix(ref Player __instance)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward,
                        out hitInfo, 50f, __instance.m_removeRayMask) &&
                    Vector3.Distance(hitInfo.point, __instance.m_eye.position) <
                    (double)__instance.m_maxPlaceDistance)
                {
                    Piece piece = hitInfo.collider.GetComponentInParent<Piece>();
                    if (piece == null &&
                        (bool)(Object)hitInfo.collider.GetComponent<Heightmap>())
                        piece = TerrainModifier.FindClosestModifierPieceInRange(hitInfo.point, 2.5f);
                    if ((bool)(Object)piece && piece.m_canBeRemoved)
                    {
                        if (Location.IsInsideNoBuildLocation(piece.transform.position))
                        {
                            __instance.Message(MessageHud.MessageType.Center, "$msg_nobuildzone");
                            return false;
                        }

                        if (!WardMonoscript.CheckAccess(piece.transform.position))
                        {
                            __instance.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                            return false;
                        }

                        if (!__instance.CheckCanRemovePiece(piece))
                            return false;
                        ZNetView component1 = piece.GetComponent<ZNetView>();
                        if (component1 == null)
                            return false;
                        if (!piece.CanBeRemoved())
                        {
                            __instance.Message(MessageHud.MessageType.Center, "$msg_cantremovenow");
                            return false;
                        }

                        WearNTear component2 = piece.GetComponent<WearNTear>();
                        if ((bool)(Object)component2)
                        {
                            component2.Remove();
                        }
                        else
                        {
                            ZLog.Log("Removing non WNT object with hammer " + piece.name);
                            component1.ClaimOwnership();
                            piece.DropResources();
                            piece.m_placeEffect.Create(piece.transform.position, piece.transform.rotation,
                                piece.gameObject.transform);
                            __instance.m_removeEffects.Create(piece.transform.position, Quaternion.identity);
                            ZNetScene.instance.Destroy(piece.gameObject);
                        }

                        ItemDrop.ItemData rightItem = __instance.GetRightItem();
                        if (rightItem != null)
                        {
                            __instance.FaceLookDirection();
                            __instance.m_zanim.SetTrigger(rightItem.m_shared.m_attack.m_attackAnimation);
                        }

                        return true;
                    }
                }

                return false;
            }

            [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
            [HarmonyPrefix]
            private static bool PlacePieceOverlapCheck(ref Player __instance, ref Piece piece)
            {
                if (piece == null) return true;
                if (!piece.GetComponent<WardMonoscript>()) return true;
                WardMonoscript? wardPiece = piece.GetComponent<WardMonoscript>();
                bool isInArea = false;
                if (!WardMonoscript.CheckAccess(__instance.m_placementGhost.transform.position, wardPiece.m_radius,
                        true,
                        true))
                {
                    isInArea = true;

                    MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");
                }

                return !isInArea;
            }

            [HarmonyPatch(typeof(Player), nameof(Player.UpdatePlacementGhost))]
            [HarmonyPrefix]
            private static void OverlapCheck_2(ref Player __instance, ref bool flashGuardStone)
            {
                if (__instance.m_placementGhost != null)
                {
                    Piece component1 = __instance.m_placementGhost.GetComponent<Piece>();
                    bool water = component1.m_waterPiece || component1.m_noInWater;
                    Vector3 point;
                    Vector3 normal;
                    Piece piece;
                    Heightmap heightmap;
                    Collider waterSurface;
                    if (__instance.PieceRayTest(out point, out normal, out piece, out heightmap, out waterSurface,
                            water))
                    {
                        __instance.m_placementStatus = Player.PlacementStatus.Valid;

                        WardMonoscript component5 = component1.GetComponent<WardMonoscript>();
                        float radius = component5 ? component5.GetWardRadius() * 2 : 0.0f;
                        bool wardCheck = component5 != null;
                        if (!WardMonoscript.CheckAccess(__instance.m_placementGhost.transform.position, radius,
                                flashGuardStone, wardCheck))
                        {
                            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");

                            __instance.m_placementStatus = Player.PlacementStatus.PrivateZone;
                        }
                    }

                    else
                    {
                        if (__instance.m_placementMarkerInstance)
                            __instance.m_placementMarkerInstance.SetActive(false);
                        __instance.m_placementGhost.SetActive(false);
                        __instance.m_placementStatus = Player.PlacementStatus.Invalid;
                    }

                    __instance.SetPlacementGhostValid(__instance.m_placementStatus == Player.PlacementStatus.Valid);
                }
            }
        }
    }
}