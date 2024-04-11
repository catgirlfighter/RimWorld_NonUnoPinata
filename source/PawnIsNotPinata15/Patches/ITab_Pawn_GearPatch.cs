using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;

namespace NonUnoPinata.Patches
{
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "DrawThingRow")]
    static class ITab_Pawn_Gear_DrawThingRow_NonUnoPinataPatch
    {
        public static void Prefix(ITab_Pawn_Gear __instance, ref float y, ref float width, Thing thing, ref bool inventory)
        {
            Pawn SelPawnForGear = Traverse.Create(__instance).Property("SelPawnForGear").GetValue<Pawn>();
            NUPUtility.DrawThingRow(SelPawnForGear, ref y, ref width, thing, ref inventory);
        }
    }
}
