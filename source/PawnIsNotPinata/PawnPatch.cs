﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace NonUnoPinata
{
    public class PawnPatch
    {
        [HarmonyPatch(typeof(Pawn), "Kill")]
        static class Pawn_Kill_NonUnoPinataPatch
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase mb)
            {
                MethodInfo m = AccessTools.Method(typeof(Pawn), "DropAndForbidEverything");
                if (m == null)
                {
                    Log.Error($"Pawn.Kill: Couldn't get Pawn.DropAndForbidEverything");
                    foreach (var i in instructions)
                        yield return i;
                    yield break;
                }
                //
                int ld = -1;
                int call = -1;
                List<CodeInstruction> list = instructions.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].opcode == OpCodes.Ldarg_0) ld = i;
                    if (list[i].opcode == OpCodes.Call && list[i].operand == m)
                    {
                        call = i;
                        break;
                    }
                }
                //
                if (ld == -1 || call == -1)
                {
                    Log.Error($"Pawn.Kill: Couldn't find Pawn.DropAndForbidEverything entry");
                    foreach (var i in instructions)
                        yield return i;
                    yield break;
                }
                //
                for (int i = 0; i < ld; i++)
                    yield return list[i];
                for (int i = call + 1; i < list.Count(); i++)
                    yield return list[i];
            }
        }

        [HarmonyPatch(typeof(Pawn), "Strip")]
        static class Pawn_Strip_NonUnoPinataPatch
        {
            static bool Prefix(Pawn __instance)
            {             
                return __instance.GetCaravan() != null || !CompStripChecker.TryDropMarked(__instance);
            }
        }
    }
}
