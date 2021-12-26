using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    public class PlaceWard : MonoBehaviour
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
                    !__instance.m_name.ToLower().Contains("better")) return;
                Piece? planned = __instance.gameObject.GetComponent<Piece>();
                foreach (Piece.Requirement? req in __instance.m_resources) req.m_recover = false;
            }

            private static void Postfix(ref Piece __instance)
            {
                FinalInit(ZNetScene.instance);
                RecipeFunction.UpdateWardRecipe(ref __instance);
                if (!__instance.name.ToLower().Contains("planned") &&
                    !__instance.m_name.ToLower().Contains("planned") ||
                    !__instance.m_name.ToLower().Contains("betterw")) return;
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
                    !__instance.m_name.ToLower().Contains("better")) return;
                Piece? planned = __instance.gameObject.GetComponent<Piece>();
                foreach (Piece.Requirement? req in planned.m_resources) req.m_recover = false;
            }

            private static void Postfix(ref WardMonoscript __instance)
            {
#if DEBUG
                // WardIsLovePlugin.WILLogger.LogError(__instance.m_name);
#endif
                if (!Player.m_localPlayer || !__instance) return;
                List<GameObject> gameObjectList = new();
                double wardDetection = _wardRange.Value;

                foreach (Collider component in Physics.OverlapSphere(Player.m_localPlayer.transform.position,
                             (float)wardDetection))
                {
                    EffectArea componentInParent = component.GetComponentInParent<EffectArea>();


                    if ((!(bool)componentInParent ? 0 :
                            Vector3.Distance(Player.m_localPlayer.transform.position,
                                componentInParent.transform.position) <= wardDetection ? 1 : 0) == 0) continue;
                    if (gameObjectList.Contains(componentInParent.gameObject)) continue;
                    gameObjectList.Add(componentInParent.gameObject);
                    try
                    {
                        if (componentInParent.m_type is EffectArea.Type.NoMonsters or EffectArea.Type.PlayerBase)
                            DestroyImmediate(componentInParent);
                    }
                    catch (UnityException ex)
                    {
                        WILLogger.LogError(
                            $"Error, couldn't destroy the effect area {ex}");
                    }
                }
            }
        }

        /* I have to figure out a way to get this working properly. Strange bugs and terrible way to code this */

        //[HarmonyPatch]
        //public static class RPC_WardCount
        //{
        //    [HarmonyPatch(typeof(Player), "PlacePiece")]
        //    [HarmonyPrefix]
        //    private static bool Prefix(Player __instance, ref Piece piece)
        //    {

        //        List<ZDO> list = new List<ZDO>();
        //        List<ZDO> list2 = new List<ZDO>();
        //        List<ZDO> list3 = new List<ZDO>();
        //        List<ZDO> list4 = new List<ZDO>();
        //        List<ZDO> list5 = new List<ZDO>();

        //        ZDOMan.instance.GetAllZDOsWithPrefab(WardIsLovePlugin.WardIsLove.name, list);
        //        ZDOMan.instance.GetAllZDOsWithPrefab(WardIsLovePlugin.WardIsLove2.name, list2);
        //        ZDOMan.instance.GetAllZDOsWithPrefab(WardIsLovePlugin.WardIsLove3.name, list3);
        //        ZDOMan.instance.GetAllZDOsWithPrefab(WardIsLovePlugin.WardIsLove4.name, list4);
        //        ZDOMan.instance.GetAllZDOsWithPrefab("guard_stone", list5);
        //        int creator = "creator".GetStableHashCode();
        //        List<ZDO> ward1ByUList = list.Where(p => p.GetLong(creator) == Game.instance.GetPlayerProfile().m_playerID).ToList();
        //        List<ZDO> ward2ByUList = list2.Where(p => p.GetLong(creator) == Game.instance.GetPlayerProfile().m_playerID).ToList();
        //        List<ZDO> ward3ByUList = list3.Where(p => p.GetLong(creator) == Game.instance.GetPlayerProfile().m_playerID).ToList();
        //        List<ZDO> ward4ByUList = list4.Where(p => p.GetLong(creator) == Game.instance.GetPlayerProfile().m_playerID).ToList();
        //        List<ZDO> ward5ByUList = list5.Where(p => p.GetLong(creator) == Game.instance.GetPlayerProfile().m_playerID).ToList();
        //        print("ALL WardIsLove  " + list.Count);
        //        print("ALL WardIsLove_Type2  " + list2.Count);
        //        print("ALL WardIsLove_Type3  " + list3.Count);
        //        print("ALL WardIsLove_Type4  " + list4.Count);
        //        print("ALL Guardstones  " + list5.Count);
        //        print($"{ward1ByUList.Count} => WardIsLove planned by player");
        //        print($"{ward2ByUList.Count} => WardIsLove_Type2 planned by player");
        //        print($"{ward3ByUList.Count} => WardIsLove_Type3 planned by player");
        //        print($"{ward4ByUList.Count} => WardIsLove_Type4 planned by player");
        //        print($"{ward5ByUList.Count} => Guardstones planned by player");

        //        if (ward1ByUList.Count >= 2 && piece.name.ToLower().Contains("betterward") && (!piece.name.ToLower().Contains("ward_type2") || !piece.name.ToLower().Contains("ward_type3") || !piece.name.ToLower().Contains("ward_type4"))) { Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"<color=red>You have already planned ${ward1ByUList.Count.ToString()} of {piece.name}</color>"); return false; }
        //        else if (ward2ByUList.Count >= 2 && piece.name.ToLower().Contains("betterward_type2")) { Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"<color=red>You have already planned ${ward2ByUList.Count.ToString()} of {piece.name}</color>"); return false; }
        //        else if (ward3ByUList.Count >= 2 && piece.name.ToLower().Contains("betterward_type3")) { Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"<color=red>You have already planned ${ward3ByUList.Count.ToString()} of {piece.name}</color>"); return false; }
        //        else if (ward4ByUList.Count >= 2 && piece.name.ToLower().Contains("betterward_type4")) { Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"<color=red>You have already planned ${ward4ByUList.Count.ToString()} of {piece.name}</color>"); return false; }
        //        else if (ward5ByUList.Count >= 2 && piece.name.ToLower().Contains("ward") && (!piece.name.ToLower().Contains("betterward"))) { Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"<color=red>You have already planned ${ward5ByUList.Count.ToString()} of {piece.name}</color>"); return false; }
        //        else { Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"<color=red>PLACING ${ward1ByUList.Count.ToString()} of {piece.name}</color>"); return true; }
        //    }


        //}
    }
}