using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace WardIsLove.Util
{
    [HarmonyPatch]
    public static class CustomCheck
    {
        public static bool CheckAccess(long playerID, Vector3 point, float radius = 0.0f, bool flash = true)
        {
            /* Made this just in case I want to change how CheckAccess works by default */
            bool flag = false;
            List<WardMonoscript> WardMonoscriptList = new();
            foreach (WardMonoscript allArea in WardMonoscript.m_allAreas)
                if (allArea.IsEnabled() && allArea.IsInside(point, radius))
                {
                    Piece component = allArea.GetComponent<Piece>();
                    if (component != null && component.GetCreator() == playerID || allArea.IsPermitted(playerID))
                    {
                        flag = true;
                        break;
                    }

                    WardMonoscriptList.Add(allArea);
                    break;
                }

            if (flag || WardMonoscriptList.Count <= 0)
                return true;
            if (!flash) return false;
            foreach (WardMonoscript WardMonoscript in WardMonoscriptList)
                WardMonoscript.FlashShield(false);
            return false;
        }
    }
}