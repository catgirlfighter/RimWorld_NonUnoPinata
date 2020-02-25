using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace NonUnoPinata
{
    class JobDriver_StripPatch
    {
        //static MethodBase target;

        [HarmonyPatch(typeof(JobDriver_Strip), "MakeNewToils")]
        public static class JobDriver_Strip_MakeNewToilsIterator_NonUnoPinataPatch
        {
            public class JobDriver_StripA: JobDriver_Strip
            {
                public Map MapA
                {
                    get
                    {
                        return Map;
                    }
                }
            }

            static void Postfix(JobDriver_StripA __instance, ref IEnumerable<Toil> __result)
            {
                Toil t = new Toil
                {
                    initAction = delegate ()
                    {
                        Thing thing = __instance.job.targetA.Thing;
                        IStrippable strippable = thing as IStrippable;
                        if (strippable != null)
                        {
                            strippable.Strip();
                        }
                        Designation designation = __instance.MapA.designationManager.DesignationOn(thing, DesignationDefOf.Strip);
                        if (designation != null)
                        {
                            designation.Delete();
                        }
                        __instance.pawn.records.Increment(RecordDefOf.BodiesStripped);
                    },
                    defaultCompleteMode = ToilCompleteMode.Instant
                };

                //naive patch
                __result = __result.Select((x, i) => i == 2 ? t : x);
            }
        }
    }
}
