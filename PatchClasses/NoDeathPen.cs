using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using WardIsLove.Extensions;
using WardIsLove.Util;
using static WardIsLove.WardIsLovePlugin;

namespace WardIsLove.PatchClasses
{

    [HarmonyPatch(typeof(Skills), nameof(Skills.OnDeath))]
    static class Skills_OnDeath_Patch
    {
        static bool Prefix(Skills __instance)
        {
            Vector3 position = Player.m_localPlayer.transform.position;
            return !WardMonoscript.CheckInWardMonoscript(position) || !CustomCheck.CheckAccess(
                Player.m_localPlayer.GetPlayerID(), position, 1f,
                false) || !WardNoDeathPen.Value || !WardEnabled.Value;
        }
        
        
        [UsedImplicitly]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instrs = instructions.ToList();
            for (int i = 0; i < instrs.Count; ++i)
            {
                if (instrs[i].opcode == OpCodes.Ldc_R4 && Math.Abs((float)instrs[i].operand - 0.25f) < 0.01)
                {
                    // Replace the ldc.r4 operation with a call to Skills_OnDeath_Patch.GetDeathValue
                    instrs[i] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Skills_OnDeath_Patch), nameof(GetDeathValue)));
                }
            }

            return instrs.AsEnumerable();
        }
        
        private static float GetDeathValue()
        {
            if(Player.m_localPlayer == null) return 0.25f;
            Vector3 position = Player.m_localPlayer.transform.position;
            if (!WardMonoscript.CheckInWardMonoscript(position) || !CustomCheck.CheckAccess(
                Player.m_localPlayer.GetPlayerID(), position, 1f,
                false) || !WardEnabled.Value)
                return 0.25f;
            WardMonoscript ward = WardMonoscriptExt.GetWardMonoscript(position);
            return ward.GetNoDeathPenOn() ? 0f : 0.25f;
        }
    }
}