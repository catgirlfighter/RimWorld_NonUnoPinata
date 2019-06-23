using System.Collections.Generic;
using Harmony;
using Verse;
using RimWorld;
namespace NonUnoPinata
{
    class DesignationPatch
    {
        [HarmonyPatch(typeof(Designation), "Notify_Removing")]
        static class Designation_Notify_Removing_NonUnoPinata
        {
            static bool Prefix(Designation __instance)
            {
                return true;
                /* designation gets removed before stripping is happened, so it doesn't work as intended
                if (__instance.def == DesignationDefOf.Strip && __instance.target.HasThing)
                {
                    Corpse c = __instance.target.Thing as Corpse;
                    if (c != null)
                    {
                        if(c.InnerPawn != null)
                            CompStripChecker.UnmarkAll(c.InnerPawn);
                    } else
                        CompStripChecker.UnmarkAll((Pawn)__instance.target.Thing);
                    return false;
                }
                return true;
                */
            }
        }
    }
}
