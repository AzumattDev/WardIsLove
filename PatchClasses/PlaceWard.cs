using HarmonyLib;
using UnityEngine;
using WardIsLove.Util;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    public class PlaceWard
    {
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