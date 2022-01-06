using HarmonyLib;
using UnityEngine;
using WardIsLove.Util;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch(typeof(Attack), nameof(Attack.SpawnOnHitTerrain))]
    static class WILAttack_SpawnOnHitTerrain_Patch
    {
        static bool Prefix(Attack __instance, Vector3 hitPoint, GameObject prefab)
        {
            TerrainModifier componentInChildren1 = prefab.GetComponentInChildren<TerrainModifier>();
            if (componentInChildren1 &&
                (!WardMonoscript.CheckAccess(hitPoint, componentInChildren1.GetRadius())))
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            TerrainOp componentInChildren2 = prefab.GetComponentInChildren<TerrainOp>();
            if (componentInChildren2 &&
                (!WardMonoscript.CheckAccess(hitPoint, componentInChildren2.GetRadius())))
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(TerrainOp), nameof(TerrainOp.Awake))]
        static class TerrainOp_Awake_Patch
        {
            static bool Prefix(TerrainOp __instance)
            {
                if (!Player.m_localPlayer) return true;
                if (!WardMonoscript.CheckInWardMonoscript(Player.m_localPlayer.transform.position) ||
                    CustomCheck.CheckAccess(
                        Player.m_localPlayer.GetPlayerID(), __instance.transform.position,
                        flash: false)) return true;
                MessageHud.instance.ShowMessage(MessageHud.MessageType.TopLeft, "$msg_privatezone");
                return false;
            }
        }
    }
}