﻿using System;
using HarmonyLib;
using Verse;

namespace NonUnoPinata
{
    [StaticConstructorOnStartup]
    public static class PatchRPG
    {
        // Token: 0x06000056 RID: 86 RVA: 0x000040C4 File Offset: 0x000022C4
        static PatchRPG()
        {
            var harmonyInstance = new Harmony("net.avilmask.rimworld.mod.NonUnoPinata.RPGInventory");
            Type type;
            if ((type = AccessTools.TypeByName("Sandy_Detailed_RPG_GearTab")) != null)
            {
                var mi = AccessTools.Method(type, "DrawThingRow", null, null);
                HarmonyMethod hm = new HarmonyMethod(typeof(ITab_Pawn_GearPatch.ITab_Pawn_Gear_DrawThingRow_NonUnoPinataPatch), nameof(ITab_Pawn_GearPatch.ITab_Pawn_Gear_DrawThingRow_NonUnoPinataPatch.Prefix), null);
                harmonyInstance.Patch(mi, hm, null);
            }
        }
    }
}
