using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;


namespace NonUnoPinata.Patches
{
    [HarmonyPatch]
    static class Pawn_HealthTracker_MakeDowned_NonUnoPinataPatch
    {
        static MethodBase target;

        static bool Prepare()
        {
            if ((target = AccessTools.Method(typeof(Pawn_HealthTracker), "MakeDowned")) == null)
            {
                Log.Error($"Pawn_HealthTracker.MakeDowned: Couldn't get Pawn_HealthTracker.MakeDowned");
                return false;
            }
            return true;
        }

        static MethodBase TargetMethod()
        {
            return target;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase mb)
        {
            MethodInfo m = AccessTools.Method(typeof(Pawn), "DropAndForbidEverything");
            if (m == null)
            {
                Log.Error($"Pawn_HealthTracker.MakeDowned: Couldn't get Pawn.DropAndForbidEverything");
                foreach (var i in instructions)
                    yield return i;
                yield break;
            }

            foreach (var i in instructions)
            {
                if (i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == m)
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(NUPUtility), nameof(NUPUtility.DropOnDown)));
                else
                    yield return i;
            }
        }
    }
}
