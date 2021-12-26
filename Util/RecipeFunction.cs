using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using WardIsLove.PatchClasses;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.Util
{
    [HarmonyPatch]
    internal class RecipeFunction
    {
        public static void UpdateWardRecipe(ref Piece __result)
        {
            if (!Player.m_localPlayer) return;
            List<Piece.Requirement> newReqs = new();

            if (__result && __result != null && __result.m_name.Contains("thorward") &&
                !__result.m_name.ToLower().Contains("planned"))
            {
                string name = __result.name;
                string[] arrayItems = _thorwardItemReqs.Value.Trim().Split(',').ToArray();
                string[] arrayReco = _thorwardReco.Value.Trim().Split(',').ToArray();
                string[] arrayAmou = _thorwardItemAmou.Value.Trim().Split(',').ToArray();
                if (arrayItems.Length <= 4)
                {
                    newReqs.AddRange(arrayItems.Select(t => ObjectDBWrapper.GetItemWithPrefab(t.Trim(), name)).Select(
                        (item, i) =>
                            new Piece.Requirement
                            {
                                m_amount = Convert.ToInt32(arrayAmou[i].Trim()),
                                m_amountPerLevel = 1,
                                m_recover = Convert.ToBoolean(arrayReco[i].Trim()),
                                m_resItem = item
                            }));

                    __result.m_resources = newReqs.ToArray();
                }
                else
                {
                    WILLogger.LogError("The items defined for this ward exceed 4.");
                }
            }
        }

        public static void UpdateRecipeFinal()
        {
            if (!Player.m_localPlayer) return;
            Piece thorWard = Thorward.GetComponent<Piece>();
            List<Piece.Requirement> newReqs = new();


            /* THOR WARD */
            string[] arrayItemsW2 = _thorwardItemReqs.Value.ToLower().Trim().Split(',').ToArray();
            string[] arrayRecoW2 = _thorwardReco.Value.ToLower().Trim().Split(',').ToArray();
            string[] arrayAmouW2 = _thorwardItemAmou.Value.ToLower().Trim().Split(',').ToArray();
            if (arrayItemsW2.Length <= 4)
            {
                newReqs.AddRange(arrayItemsW2.Select(t => ObjectDBWrapper.GetItemWithPrefab(t.Trim(), thorWard.name))
                    .Select((item2, i2) =>
                        new Piece.Requirement
                        {
                            m_amount = Convert.ToInt32(arrayAmouW2[i2].Trim()),
                            m_amountPerLevel = 1,
                            m_recover = Convert.ToBoolean(arrayRecoW2[i2].Trim()),
                            m_resItem = item2
                        }));

                thorWard.m_resources = newReqs.ToArray();
            }
            else
            {
                WILLogger.LogError($"The items defined for {thorWard.name} exceed 4.");
            }

            fInit = true;
        }
    }
}