using System;
using System.Linq;
using System.Collections.Generic;
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

            if (thing == null || !(thing is ThingWithComps) && !thing.GetType().IsSubclassOf(typeof(ThingWithComps)))
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
                    pawn.apparel.TryDrop(apparel[i], out derp, pos, false);
                }

            if (pawn.Faction != null)
                pawn.Faction.Notify_MemberStripped(pawn, Faction.OfPlayer);

            return true;
        }

        static bool flagMark(ThingWithComps thing, StripFlags crossFlags, StripFlags stripFlags)
        {
            if (thing == null || crossFlags.HasFlag(StripFlags.Equipment) && !stripFlags.HasFlag(StripFlags.Unburnable) && thing.Stuff != null && !thing.Stuff.burnableByRecipe)
                return false;
            if (stripFlags.HasFlag(StripFlags.Inventory) && crossFlags.HasFlag(StripFlags.Inventory)
                || stripFlags.HasFlag(StripFlags.Equipment) && crossFlags.HasFlag(StripFlags.Equipment)
                || stripFlags.HasFlag(StripFlags.Apparel) && crossFlags.HasFlag(StripFlags.Apparel) && (!stripFlags.HasFlag(StripFlags.Untainted) || !thing.def.IsApparel || !((Apparel)thing).WornByCorpse)
                || stripFlags.HasFlag(StripFlags.Smeltable) && (crossFlags.HasFlag(StripFlags.Equipment) || crossFlags.HasFlag(StripFlags.Apparel)) && thing.Smeltable)
                return true;
            //
            return false;
        }

        public static bool MarkAll(Pawn pawn, bool val, StripFlags flags = StripFlags.Apparel | StripFlags.Equipment | StripFlags.Inventory)
        {
            bool result = false;

            ThingOwner<Thing> inventory = pawn.inventory == null ? null : pawn.inventory.innerContainer;
            if (!inventory.NullOrEmpty())
                foreach (var t in inventory)
                    if (flagMark(t as ThingWithComps, StripFlags.Inventory, flags))
                    {
                        result |= true;
                        GetChecker(t).ShouldStrip = val;
                    }
            //
            List<ThingWithComps> equipment = pawn.equipment == null ? null : pawn.equipment.AllEquipmentListForReading;
            if (!equipment.NullOrEmpty())
                foreach (var t in equipment)
                    if (flagMark(t as ThingWithComps, StripFlags.Equipment, flags))
                    {
                        result |= true;
                        GetChecker(t).ShouldStrip = val;
                    }
            //
            List<Apparel> apparel = pawn.apparel == null ? null : pawn.apparel.WornApparel;
            if (!apparel.NullOrEmpty())
                foreach (var t in apparel)
                    if (flagMark(t as ThingWithComps, StripFlags.Apparel, flags))
                    {
                        result |= true;
                        GetChecker(t).ShouldStrip = val;
                    }
            //
            return result;
        }
    }
}
