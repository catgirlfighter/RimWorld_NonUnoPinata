using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
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

                foreach (var i in instructions)
                {
                    if (i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == m)
                        yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PawnPatch), nameof(PawnPatch.DropOnDown)));
                    else
                        yield return i;
                }
            }
        }

        /*
        [HarmonyPatch(typeof(Pawn_HealthTracker), "CheckForStateChange")]
        public static class Pawn_HealthTracker_CheckForStateChange_NonUnoPinataPatch
        {
            private static bool checkAwake(Pawn pawn)
            {
                Log.Message($"{pawn},awake={pawn.Awake()}");
                return pawn.Awake();
                
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase mb)
            {
                FieldInfo f = AccessTools.Field(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.capacities));
                FieldInfo pawn = AccessTools.Field(typeof(Pawn_HealthTracker), "pawn"); //private
                MethodInfo awake = AccessTools.Method(typeof(Pawn_HealthTracker_CheckForStateChange_NonUnoPinataPatch), nameof(Pawn_HealthTracker_CheckForStateChange_NonUnoPinataPatch.checkAwake));

                CodeInstruction oldi = null;
                foreach (var i in instructions)
                {

                    if (oldi != null)
                    {
                        if (oldi.opcode == OpCodes.Ldarg_0 && i.opcode == OpCodes.Ldfld && i.operand == (object)f)
                        {
                            Label l = il.DefineLabel();
                            yield return new CodeInstruction(OpCodes.Ldarg_0);
                            yield return new CodeInstruction(OpCodes.Ldfld, pawn);
                            yield return new CodeInstruction(OpCodes.Call, awake);
                            yield return new CodeInstruction(OpCodes.Brtrue_S, l);
                            yield return new CodeInstruction(OpCodes.Ret);
                            oldi.labels.Add(l);
                        }
                        yield return oldi;
                    }
                    oldi = i;
                }
                yield return oldi;
            }
        }
        */
    }
}
