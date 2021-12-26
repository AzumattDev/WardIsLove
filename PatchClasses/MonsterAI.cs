using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

namespace WardIsLove.PatchClasses
{
    [HarmonyPatch]
    internal class MonsterAITEST
    {
        [HarmonyPatch(typeof(MonsterAI), nameof(MonsterAI.UpdateAI))]
        private class AllowTamedInNoMonsterArea
        {
            private static readonly MethodInfo TamedCheckAdder =
                AccessTools.DeclaredMethod(typeof(AllowTamedInNoMonsterArea), nameof(AddTamedCheck));

            private static readonly MethodInfo AreaChecker =
                AccessTools.DeclaredMethod(typeof(EffectArea), nameof(EffectArea.IsPointInsideArea));

            private static EffectArea? AddTamedCheck(EffectArea? targetEffectArea, MonsterAI monsterAI)
            {
                return monsterAI.m_character.IsTamed() ? null : targetEffectArea;
            }

            [UsedImplicitly]
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> instrs = instructions.ToList();
                for (int i = 0; i < instrs.Count; ++i)
                {
                    yield return instrs[i];
                    if (instrs[i].opcode != OpCodes.Call || !instrs[i].OperandIs(AreaChecker) ||
                        instrs[i - 2].opcode != OpCodes.Ldc_I4_S ||
                        !instrs[i - 2].OperandIs((int)EffectArea.Type.NoMonsters)) continue;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, TamedCheckAdder);
                }
            }
        }
    }
}