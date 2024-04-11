using System;
using HarmonyLib;
using Verse;
using Verse.Sound;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;

namespace NonUnoPinata.Patches
{
    //[HarmonyPatch]
    static class RPGStyleInventory_PopupMenu_NonUnoPinataPatch
    {
        static readonly Color hColor = new Color(1f, 0.8f, 0.8f, 1f);

        internal static void Postfix(List<FloatMenuOption> __result, Pawn pawn, Thing thing, bool inventory)
        {
            Corpse corpse = pawn.Corpse;

            inventory = inventory && corpse == null;

            if (corpse != null || StrippableUtility.CanBeStrippedByColony(pawn))
            {
                CompStripChecker c = CompStripChecker.GetChecker(thing, false);
                if (c == null)
                {
                    return;
                }

                ThingWithComps holder = corpse == null ? pawn : (ThingWithComps)corpse;
                if (c.ShouldStrip)
                {
                    __result.Add(new FloatMenuOption( "StripThingCancel".Translate(), delegate()
                    {
                        SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                        NUPUtility.SetShouldStrip(false, c, pawn, holder);
                    }, NUPUtility.texStripThingCancel, hColor));
                }
                else
                {
                    __result.Add(new FloatMenuOption("StripThing".Translate(), delegate ()
                    {
                        SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                        NUPUtility.SetShouldStrip(true, c, pawn, holder);
                    }, NUPUtility.texStripThing, Color.white));
                }
            }
        }
    }

    static class RPGStyleInventory_DrawSlotIcons_NonUnoPinataPatch
    {
        internal static void Postfix(object __instance, Thing thing, bool equipment, bool inventory, Rect slotRect, ref float x, ref float y)
        {
            var c = CompStripChecker.GetChecker(thing);
            if (c?.ShouldStrip == true)
            {
                RPGStyleInventory_NonUnoPinataPatch.LDrawSlotIcon.Invoke(__instance, new object[] { slotRect, x, y, NUPUtility.texStripThing, (string)"StripThing".Translate() });
            }
        }
    }

    [StaticConstructorOnStartup]
    static class RPGStyleInventory_NonUnoPinataPatch
    {
        //public void DrawSlotIcon(Rect slotRect, ref float x, ref float y, Texture2D tex, string tip)
        public static MethodInfo LDrawSlotIcon = null;
        static RPGStyleInventory_NonUnoPinataPatch()
        {
            var harmonyInstance = new Harmony("net.avilmask.rimworld.mod.NonUnoPinata.RPGStyleInventory");
            Type type;
            if ((type = AccessTools.TypeByName("Sandy_Detailed_RPG_GearTab")) != null)
            {
                LDrawSlotIcon = AccessTools.Method(type, "DrawSlotIcon");
                var mi = AccessTools.Method(type, "DrawThingRow", null, null);
                HarmonyMethod hm = new HarmonyMethod(typeof(ITab_Pawn_Gear_DrawThingRow_NonUnoPinataPatch), nameof(ITab_Pawn_Gear_DrawThingRow_NonUnoPinataPatch.Prefix), null);
                harmonyInstance.Patch(mi, hm, null);

                mi = AccessTools.Method(type, "PopupMenu", null, null);
                if (mi != null)
                {
                    hm = new HarmonyMethod(typeof(RPGStyleInventory_PopupMenu_NonUnoPinataPatch), nameof(RPGStyleInventory_PopupMenu_NonUnoPinataPatch.Postfix), null);
                    harmonyInstance.Patch(mi, null, hm);
                }

                mi = AccessTools.Method(type, "DrawSlotIcons");
                if (mi != null)
                {
                    hm = new HarmonyMethod(typeof(RPGStyleInventory_DrawSlotIcons_NonUnoPinataPatch), nameof(RPGStyleInventory_DrawSlotIcons_NonUnoPinataPatch.Postfix));
                    harmonyInstance.Patch(mi, null, hm);
                }
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
