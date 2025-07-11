using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace NonUnoPinata.Patches
{
    [HarmonyPatch(typeof(JobDriver_Strip), "MakeNewToils")]
    static class JobDriver_Strip_MakeNewToilsIterator_NonUnoPinataPatch
    {

        //in vanilla it deletes designation first and then strips, we need it other way around
        internal static void Postfix(JobDriver_Strip __instance, ref IEnumerable<Toil> __result)
        {
            Toil t = new Toil
            {
                initAction = delegate ()
                {
                    Thing thing = __instance.job.targetA.Thing;
                    if (thing is IStrippable strippable)
                    {
                        strippable.Strip();
                    }
                    Designation designation = __instance.pawn.MapHeld.designationManager.DesignationOn(thing, DesignationDefOf.Strip);
                    designation?.Delete();
                    __instance.pawn.records.Increment(RecordDefOf.BodiesStripped);
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };

            //naive patch
            __result = __result.Select((x, i) => i == 2 ? t : x);
        }
    }
}
