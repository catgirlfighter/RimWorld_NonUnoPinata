using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using Verse;


namespace NonUnoPinata
{
    class HealthTrackerPatch
    {
        static MethodBase target;

        [HarmonyPatch]
        public static class Pawn_HealthTracker_MakeDowned_NonUnoPinataPatch
        {
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

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase mb)
            {
                MethodInfo m = AccessTools.Method(typeof(Pawn), "DropAndForbidEverything");
                if (m == null)
                {
                    Log.Error($"Pawn_HealthTracker.MakeDowned: Couldn't get Pawn.DropAndForbidEverything");
                    foreach (var i in instructions)
                        yield return i;
                    yield break;
                }
                
                foreach(var i in instructions)
                {
                    if (i.opcode == OpCodes.Callvirt && i.operand == m)
                        yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PawnPatch), nameof(PawnPatch.DropOnDown)));
                    else
                        yield return i;
                }

                //
                /*
                int ld = -1;
                int call = -1;
                List<CodeInstruction> list = instructions.ToList();
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].opcode == OpCodes.Ldarg_0) ld = i;
                    if (list[i].opcode == OpCodes.Callvirt && list[i].operand == m)
                    {
                        call = i;
                        break;
                    }
                }
                //
                if (ld == -1 || call == -1)
                {
                    Log.Error($"Pawn_HealthTracker.MakeDowned: Couldn't find Pawn.DropAndForbidEverything entry");
                    foreach (var i in instructions)
                        yield return i;
                    yield break;
                }
                //
                for (int i = 0; i < ld; i++)
                    yield return list[i];
                for (int i = call + 1; i < list.Count(); i++)
                    yield return list[i];
                */
            }
        }
    }
}
