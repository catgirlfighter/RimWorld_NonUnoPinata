using System.Collections.Generic;
using HarmonyLib;
using Verse;
using RimWorld;
namespace NonUnoPinata.Patches
{
    [HarmonyPatch(typeof(Designation), "Notify_Removing")]
    static class Designation_Notify_Removing_NonUnoPinataPatch
    {
        static bool Prefix(Designation __instance)
        {
            //return true;
            // designation gets removed before stripping is happened, so it doesn't work as intended
            if (__instance.def == DesignationDefOf.Strip && __instance.target.HasThing)
            {
                Corpse c = __instance.target.Thing as Corpse;
                if (c != null)
                {
                    if(c.InnerPawn != null)
                        CompStripChecker.MarkAll(c.InnerPawn, false);
                } else
                    CompStripChecker.MarkAll((Pawn)__instance.target.Thing, false);
                return false;
            }
            return true;
                
        }
    }
}
