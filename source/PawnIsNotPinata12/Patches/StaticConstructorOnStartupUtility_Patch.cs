using System;
using System.Collections.Generic;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace NonUnoPinata
{
    //[HarmonyPatch(typeof(StaticConstructorOnStartupUtility), "CallAll")]
    [HarmonyPatch]
    static class StaticConstructorOnStartupUtility_CallAll_NonUnoPinataPatch
    {
        internal static MethodBase TargetMethod()
        {
            MethodBase LCallAll = AccessTools.Method("BetterLoading.Stage.InitialLoad.StageRunStaticCctors:PreCallAll");
            if (LCallAll == null)
            {
                LCallAll = AccessTools.Method("Verse.StaticConstructorOnStartupUtility:CallAll");
                if (LCallAll == null)
                    throw new Exception("Couldn't find StaticConstructorOnStartupUtility.CallAll()");
            }
            else
                Log.Message("[NonUnoPinata] BetterLoading detected, workaround initiated");
            return LCallAll;
        }
        //
        static void Postfix()
        {
            List<ThingDef> list = DefDatabase<ThingDef>.AllDefsListForReading;
                
            foreach(var thing in list)
            {
                if (thing.category == ThingCategory.Item && thing.thingClass == typeof(Thing))
                {
                    thing.thingClass = typeof(ThingWithComps);
                }
            }
        }
    }
}
