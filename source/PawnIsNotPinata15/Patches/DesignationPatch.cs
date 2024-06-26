﻿using System.Collections.Generic;
using HarmonyLib;
using Verse;
using RimWorld;
namespace NonUnoPinata.Patches
{
    [HarmonyPatch(typeof(Designation), "Notify_Removing")]
    public static class Designation_Notify_Removing_NonUnoPinataPatch
    {
        public static bool Prefix(Designation __instance)
        {
            // designation gets removed before stripping is happened, so it doesn't work as intended
            if (__instance.def == DesignationDefOf.Strip && __instance.target.HasThing)
            {
                if (__instance.target.Thing is Corpse c)
                {
                    if (c.InnerPawn != null)
                        CompStripChecker.MarkAll(c.InnerPawn, false);
                }
                else
                    CompStripChecker.MarkAll((Pawn)__instance.target.Thing, false);
                return false;
            }
            return true;
                
        }
    }
}
