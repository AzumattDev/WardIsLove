/*using System;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Util;

namespace WardIsLove.Extensions;

public class PieceChecker : MonoBehaviour, Interactable, Hoverable
{
    public void Awake()
    {
        WardIsLovePlugin.WILLogger.LogInfo("PieceChecker Awake");
    }

    public bool Interact(Humanoid character, bool hold, bool alt)
    {
        if (gameObject.layer == LayerMask.NameToLayer("piece"))
        {
            if (Player.m_localPlayer == null) return false;
            var player = Player.m_localPlayer;
            if (!WardIsLovePlugin.WardEnabled.Value)
                return true;
            bool flag = false;
            if (!WardMonoscript.CheckInWardMonoscript(gameObject.transform.position) ||
                CustomCheck.CheckAccess(
                    player.GetPlayerID(), gameObject.transform.position,
                    flash: false)) return !flag;
            WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(gameObject.transform.position);
            if (pa.GetChestInteractOn()) return true;

            player.Message(MessageHud.MessageType.Center, "$msg_privatezone");
            return false;
        }

        return true;
    }

    public bool UseItem(Humanoid user, ItemDrop.ItemData item) => false;

    public string GetHoverText() => "";

    public string GetHoverName() => "";
}

/*[HarmonyPatch(typeof(Piece), nameof(Piece.Awake))]
static class PieceAwakePatch
{
    static void Postfix(Piece __instance)
    {
        if (!__instance.m_nview.IsValid()) return;
        __instance.TryGetComponent<PieceChecker>(out var pieceCheckerComponent);
        if (pieceCheckerComponent == null) return;
        __instance.transform.root.gameObject.AddComponent<PieceChecker>();
    }
}#1#

/*[HarmonyPatch(typeof(Interactable), nameof(Interactable.Interact))]
static class InteractableInteractPatch
{
    static bool Prefix(Interactable __instance, Humanoid user, bool hold, bool alt)
    {
        if (user.IsPlayer())
        {
            if (Player.m_localPlayer == null) return true;
            if (user == Player.m_localPlayer)
            {
                var player = Player.m_localPlayer;
                GameObject hoverObject = player.GetHoverObject();
                Hoverable hoverable = hoverObject
                    ? hoverObject.GetComponentInParent<Hoverable>()
                    : null;
                if (hoverable != null && !TextViewer.instance.IsVisible())
                {
                    if (hoverObject.layer == LayerMask.NameToLayer("piece"))
                    {
                        if (!WardIsLovePlugin.WardEnabled.Value)
                            return true;
                        bool flag = false;
                        if (!WardMonoscript.CheckInWardMonoscript(hoverObject.transform.position) ||
                            CustomCheck.CheckAccess(
                                player.GetPlayerID(), hoverObject.transform.position,
                                flash: false)) return !flag;
                        WardMonoscript pa = WardMonoscriptExt.GetWardMonoscript(hoverObject.transform.position);
                        if (pa.GetChestInteractOn()) return true;

                        player.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                        return false;
                    }
                }
            }
        }

        return true;
    }
}#1#*/