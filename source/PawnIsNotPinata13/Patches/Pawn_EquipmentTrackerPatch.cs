using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;
using RimWorld;


namespace NonUnoPinata.Patches
{
    static class Pawn_EquipmentTracker_NonUnoPinata_Events
    {
        public static void Notify_EquipmentChanged(this Pawn_EquipmentTracker tracker)
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                tracker.pawn.Drawer.renderer.graphics.SetApparelGraphicsDirty();
                PortraitsCache.SetDirty(tracker.pawn);
                GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(tracker.pawn);
            });
        }
    }

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

    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded")]
    static class Pawn_EquipmentTracker_Notify_EquipmentAdded_NonUnoPinataPatch
    {
        static void Prefix(Pawn_EquipmentTracker __instance)
        {
            __instance.Notify_EquipmentChanged();
        }
    }

    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentRemoved")]
    static class Pawn_EquipmentTracker_Notify_EquipmentRemoved_NonUnoPinataPatch
    {
        static void Prefix(Pawn_EquipmentTracker __instance)
        {
            __instance.Notify_EquipmentChanged();
        }
    }
}
