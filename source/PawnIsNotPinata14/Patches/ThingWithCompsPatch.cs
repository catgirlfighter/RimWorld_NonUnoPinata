using HarmonyLib;
using Verse;

namespace NonUnoPinata.Patches
{
    [HarmonyPatch(typeof(ThingWithComps), "ExposeData")]
    static class ThingWithComps_ExposeData_NonUnoPinataPatch
    {
        internal static void Postfix(ThingWithComps __instance)
        {
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                bool a = false;
                Scribe_Values.Look(ref a, "NonUnoPinataShouldStrip", defaultValue: false);
                if (a) CompStripChecker.GetChecker(__instance, a);
            }
        }
    }
}
