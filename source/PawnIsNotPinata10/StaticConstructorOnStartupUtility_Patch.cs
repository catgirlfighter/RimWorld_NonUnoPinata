using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Harmony;
using RimWorld;

namespace NonUnoPinata
{
    public static class StaticConstructorOnStartupUtility_Patch
    {
        [HarmonyPatch(typeof(StaticConstructorOnStartupUtility), "CallAll")]
        static class StaticConstructorOnStartupUtility_CallAll_NonUnoPinataPatch
        {
            static void Postfix()
            {
                List<ThingDef> list = DefDatabase<ThingDef>.AllDefsListForReading;
                
                foreach(var thing in list)
                {
                    if (thing.category == ThingCategory.Item && thing.thingClass == typeof(Thing))
                    {
                        //Log.Message($"{thing}");
                        thing.thingClass = typeof(ThingWithComps);
                    }
                }
            }
        }
    }
}
