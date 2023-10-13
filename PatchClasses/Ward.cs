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
                    !Game.instance.isActiveAndEnabled || !WardEnabled.Value) return;
                foreach (WardMonoscript? allArea in WardMonoscript.m_allAreas.Where(allArea => allArea.IsEnabled() &&
                                                                                               allArea.IsInside(Player.m_localPlayer.transform.position, 0.0f) &&
                                                                                               allArea.GetShowMarkerOn()))
                    allArea.ShowAreaMarker();
            }
        }

        [HarmonyPatch]
        private static class OverlapCheck
        {
            [HarmonyPatch(typeof(Player), nameof(Player.RemovePiece))]
            [HarmonyPrefix]
            private static bool PrefixRemovePiece(ref Player __instance)
            {
                if (!WardEnabled.Value)
                    return true;
                bool flag = false;

                RaycastHit hitInfo;
                if (!Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward,
                        out hitInfo, 50f, __instance.m_removeRayMask) ||
                    !(Vector3.Distance(hitInfo.point, __instance.m_eye.position) <
                      (double)__instance.m_maxPlaceDistance)) return false;
                Piece piece = hitInfo.collider.GetComponentInParent<Piece>();
                if (piece == null && hitInfo.collider.GetComponent<Heightmap>())
                    piece = TerrainModifier.FindClosestModifierPieceInRange(hitInfo.point, 2.5f);
                if (!piece || !piece.m_canBeRemoved) return false;

                /* For the piece */
                if (!WardMonoscript.CheckAccess(piece.transform.position, flash: false, wardCheck: true))
                {
                    __instance.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                    return flag;
                }

                /* For the player */
                if (!WardMonoscript.CheckAccess(__instance.transform.position, flash: false, wardCheck: true))
                {
                    __instance.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                    return flag;
                }

                return !flag;
            }

            [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
            [HarmonyPrefix]
            private static bool PlacePieceOverlapCheck(ref Player __instance, ref Piece piece)
            {
                if (piece == null) return true;
                WardMonoscript component5 = piece.GetComponent<WardMonoscript>();
                float radius = component5 ? component5.GetWardRadius() : 0.0f;
                if (WardMonoscript.CheckAccess(__instance.m_placementGhost.transform.position, radius, flash: false,
                        wardCheck: true)) return true;
                __instance.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
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
                        float radius = component5 ? component5.GetWardRadius() : 0.0f;
                        bool wardCheck = component5 != null;
                        if (!WardMonoscript.CheckAccess(__instance.m_placementGhost.transform.position, radius,
                                flashGuardStone, wardCheck))
                        {
                            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, "$msg_privatezone");

                            __instance.m_placementStatus = Player.PlacementStatus.PrivateZone;
                        }
                    }
                }
            }
        }
    }
}