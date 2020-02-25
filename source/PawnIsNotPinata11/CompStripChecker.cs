using System;
using System.Linq;
using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace NonUnoPinata
{
    public class CompStripChecker : ThingComp
    {
        public bool ShouldStrip = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
            if (parent.ParentHolder != null)
            {
                Scribe_Values.Look(ref ShouldStrip, "NonUnoPinataShouldStrip", defaultValue: false);
            }
        }

        public void Init(bool AShouldStrip)
        {
            ShouldStrip = AShouldStrip;
        }

        static public CompStripChecker GetChecker(Thing thing, bool InitShouldStrip = false)
        {

            if (!(thing is ThingWithComps) && !thing.GetType().IsSubclassOf(typeof(ThingWithComps)))
                return null;
            ThingWithComps TWC = (ThingWithComps)thing;
            if (TWC.AllComps == null)
                return null;
            CompStripChecker thingComp = thing.TryGetComp<CompStripChecker>();
            if (thingComp == null)
            {
                thingComp = (CompStripChecker)Activator.CreateInstance(typeof(CompStripChecker));
                thingComp.parent = TWC;
                TWC.AllComps.Add(thingComp);
            }
            thingComp.ShouldStrip = thingComp.ShouldStrip || InitShouldStrip;
            return thingComp;
        }

        public static Thing getFirstMarked(Pawn pawn)
        {
            Thing t = null;
            if (pawn.inventory != null) t = pawn.inventory.innerContainer.FirstOrDefault(x => x.TryGetComp<CompStripChecker>() != null && x.TryGetComp<CompStripChecker>().ShouldStrip);
            if (t == null && pawn.equipment != null) t = pawn.equipment.AllEquipmentListForReading.FirstOrDefault(x => x.TryGetComp<CompStripChecker>() != null && x.TryGetComp<CompStripChecker>().ShouldStrip);
            if (t == null && pawn.apparel != null) t = pawn.apparel.WornApparel.FirstOrDefault(x => x.TryGetComp<CompStripChecker>() != null && x.TryGetComp<CompStripChecker>().ShouldStrip);
            return t;
        }

        public static bool TryDropMarked(Pawn pawn)
        {
            IntVec3 pos = (pawn.Corpse == null) ? pawn.PositionHeld : pawn.Corpse.PositionHeld;

            List<Thing> inventory = pawn.inventory == null ? null : pawn.inventory.innerContainer.Where(x => x.TryGetComp<CompStripChecker>() != null && x.TryGetComp<CompStripChecker>().ShouldStrip).ToList();
            List<ThingWithComps> equipment = pawn.equipment == null ? null : pawn.equipment.AllEquipmentListForReading.Where(x => x.TryGetComp<CompStripChecker>() != null && x.TryGetComp<CompStripChecker>().ShouldStrip).ToList();
            List<Apparel> apparel = pawn.apparel == null ? null : pawn.apparel.WornApparel.Where(x => x.TryGetComp<CompStripChecker>() != null && x.TryGetComp<CompStripChecker>().ShouldStrip).ToList();

            if (inventory.NullOrEmpty() && equipment.NullOrEmpty() && apparel.NullOrEmpty())
                return false;

            if (!inventory.NullOrEmpty())
                for (int i = inventory.Count - 1; i >= 0; i--)
                {
                    Thing derp;
                    pawn.inventory.innerContainer.TryDrop(inventory[i], pos, pawn.MapHeld, ThingPlaceMode.Near, out derp);
                }

            if (!equipment.NullOrEmpty())
                for (int i = equipment.Count - 1; i >= 0; i--)
                {
                    ThingWithComps derp;
                    pawn.equipment.TryDropEquipment(equipment[i], out derp, pos, false);
                }

            if (!apparel.NullOrEmpty())
                for (int i = apparel.Count - 1; i >= 0; i--)
                {
                    Apparel derp;
                    pawn.apparel.TryDrop(apparel[i], out derp, pos);
                }
            return true;
        }

        public static bool MarkAll(Pawn pawn, bool val, int flags = 7)
        {
            bool result = false;
            if ((flags & 1) == 1)
            {
                ThingOwner<Thing> inventory = pawn.inventory == null ? null : pawn.inventory.innerContainer;
                if (!inventory.NullOrEmpty())
                {
                    result = true;
                    for (int i = inventory.Count - 1; i >= 0; i--)
                        if (inventory[i] is ThingWithComps)
                            GetChecker(inventory[i]).ShouldStrip = val;
                }
            }

            if ((flags & 2) == 2)
            {
                List<ThingWithComps> equipment = pawn.equipment == null ? null : pawn.equipment.AllEquipmentListForReading;
                if (!equipment.NullOrEmpty())
                {
                    result = true;
                    for (int i = equipment.Count - 1; i >= 0; i--)
                        if(equipment[i] is ThingWithComps)
                            GetChecker(equipment[i]).ShouldStrip = val;
                }
            }

            if ((flags & 4) == 4)
            {
                List<Apparel> apparel = pawn.apparel == null ? null : pawn.apparel.WornApparel;
                if (!apparel.NullOrEmpty())
                {
                    result = true;
                    for (int i = apparel.Count - 1; i >= 0; i--)
                        if(apparel[i] is ThingWithComps)
                            GetChecker(apparel[i]).ShouldStrip = val;
                }
            }
            return result;
        }
    }

    [HarmonyPatch(typeof(GenDrop), nameof(GenDrop.TryDropSpawn))]
    static class GenPlace_TryPlaceThing_NonUnoPinataPatch
    {

        static void Postfix(Thing thing, IntVec3 dropCell, Map map, ThingPlaceMode mode, Thing resultingThing, Action<Thing, int> placedAction, Predicate<IntVec3> nearPlaceValidator)
        {
            CompStripChecker UChecker = resultingThing.TryGetComp<CompStripChecker>();
            if (UChecker != null)
            {
                UChecker.ShouldStrip = false;
            }
        }
    }

    [HarmonyPatch(typeof(ThingWithComps), "ExposeData")]
    static class ThingWithComps_ExposeData_NonUnoPinataPatch
    {
        static void Postfix(ThingWithComps __instance)
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
