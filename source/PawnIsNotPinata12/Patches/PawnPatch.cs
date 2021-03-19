using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace NonUnoPinata.Patches
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
            foreach(var i in instructions)
            {
                if (i.opcode == OpCodes.Call && (MethodInfo)i.operand == m)
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(NUPUtility), nameof(NUPUtility.DropOnKill)));
                else
                    yield return i;
            }
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
