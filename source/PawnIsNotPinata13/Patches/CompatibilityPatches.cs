using System;
using HarmonyLib;
using Verse;
using System.Reflection;

namespace NonUnoPinata.Patches
{
    [StaticConstructorOnStartup]
    static class RPGStyleInventory_NonUnoPinataPatch
    {
        static RPGStyleInventory_NonUnoPinataPatch()
        {
            var harmonyInstance = new Harmony("net.avilmask.rimworld.mod.NonUnoPinata.RPGStyleInventory");
            Type type;
            if ((type = AccessTools.TypeByName("Sandy_Detailed_RPG_GearTab")) != null)
            {
                var mi = AccessTools.Method(type, "DrawThingRow", null, null);
                HarmonyMethod hm = new HarmonyMethod(typeof(ITab_Pawn_Gear_DrawThingRow_NonUnoPinataPatch), nameof(ITab_Pawn_Gear_DrawThingRow_NonUnoPinataPatch.Prefix), null);
                harmonyInstance.Patch(mi, hm, null);
            }
        }
    }

    [StaticConstructorOnStartup]
    static class AwesomeInventory_NonUnoPinataPatch
    {
         static AwesomeInventory_NonUnoPinataPatch()
        {
            var harmonyInstance = new Harmony("net.avilmask.rimworld.mod.NonUnoPinata.AwesomeInventory");
            Type type;
            if ((type = AccessTools.TypeByName("AwesomeInventory.UI.DrawGearTabWorker")) != null)
            {
                var mi = AccessTools.Method(type, "DrawThingRow", null, null);
                HarmonyMethod hm = new HarmonyMethod(typeof(AwesomeInventory_NonUnoPinataPatch), nameof(AwesomeInventory_NonUnoPinataPatch.Prefix), null);
                harmonyInstance.Patch(mi, hm, null);
            }
        }

        static void Prefix(Pawn selPawn, ref float y, ref float width, Thing thing, ref bool inventory)
        {
            if (selPawn == null || thing == null) return;
            NUPUtility.DrawThingRow(selPawn, ref y, ref width, thing, ref inventory);
        }
    }
}
