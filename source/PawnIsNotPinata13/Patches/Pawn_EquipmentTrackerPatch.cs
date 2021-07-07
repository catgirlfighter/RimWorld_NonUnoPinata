using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;


namespace NonUnoPinata.Patches
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_PawnSpawned")]
    static class Pawn_EquipmentTracker_Notify_PawnSpawned_NonUnoPinataPatch
    {
        static bool doDrop(Pawn pawn)
        {
            return Settings.player_downed_drop_equipment && pawn.IsColonistPlayerControlled
                || Settings.nonplayer_downed_drop_equipment && !pawn.IsColonistPlayerControlled;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase mb)
        {
            //FieldInfo f = AccessTools.Field(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.capacities));
            FieldInfo pawn = AccessTools.Field(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.pawn));
            MethodInfo dodrop = AccessTools.Method(typeof(Pawn_EquipmentTracker_Notify_PawnSpawned_NonUnoPinataPatch), nameof(Pawn_EquipmentTracker_Notify_PawnSpawned_NonUnoPinataPatch.doDrop));

            foreach (var i in instructions)
            {

                yield return i;
                if (i.opcode == OpCodes.Beq_S)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, pawn);
                    yield return new CodeInstruction(OpCodes.Call, dodrop);
                    yield return new CodeInstruction(OpCodes.Brfalse_S, i.operand);
                }
            }
        }
    }
}
