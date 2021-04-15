using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
using RimWorld.Planet;
using UnityEngine;
using Verse.Sound;
using System;

namespace NonUnoPinata
{
    [Flags]
    public enum StripFlags
    {
        None = 0,
        Inventory = 1,
        Equipment = 2,
        Apparel = 4,
        Smeltable = 8,
        Untainted = 16,
        Unburnable = 32
    }

    public static class NUPUtility
    {
        public static void DropUnmarkableNearPawn(Pawn_InventoryTracker inventory, IntVec3 pos, bool forbid = false, bool unforbid = false)
        {
            if (inventory.pawn.MapHeld == null)
            {
                Log.Error("Tried to drop all inventory near pawn but the pawn is unspawned. pawn=" + inventory.pawn, false);
                return;
            }
            List<Thing> tmpThingList = new List<Thing>();
            tmpThingList.AddRange(inventory.innerContainer);
            for (int i = 0; i < tmpThingList.Count; i++)
            {
                ThingWithComps twc = tmpThingList[i] as ThingWithComps;

                if (twc != null)
                    continue;

                Thing thing;
                inventory.innerContainer.TryDrop(tmpThingList[i], pos, inventory.pawn.MapHeld, ThingPlaceMode.Near, out thing, delegate (Thing t, int unused)
                {
                    if (forbid)
                    {
                        t.SetForbiddenIfOutsideHomeArea();
                    }
                    if (unforbid)
                    {
                        t.SetForbidden(false, false);
                    }
                    if (t.def.IsPleasureDrug)
                    {
                        LessonAutoActivator.TeachOpportunity(ConceptDefOf.DrugBurning, OpportunityType.Important);
                    }
                }, null);
            }
        }

        public static void DropOnKill(Pawn pawn, bool keepInventoryAndEquipmentIfInBed)
        {
            DropThings(pawn, keepInventoryAndEquipmentIfInBed, true);
        }

        public static void DropOnDown(Pawn pawn, bool keepInventoryAndEquipmentIfInBed)
        {
            DropThings(pawn, keepInventoryAndEquipmentIfInBed, false);
        }

        public static void DropThings(Pawn pawn, bool keepInventoryAndEquipmentIfInBed, bool IsAKill)
        {
            if (pawn.kindDef.destroyGearOnDrop || pawn.InContainerEnclosed || keepInventoryAndEquipmentIfInBed && pawn.InBed())
                pawn.DropAndForbidEverything(keepInventoryAndEquipmentIfInBed);
            //
            if (pawn.equipment != null
                && (pawn.IsColonistPlayerControlled && (IsAKill && Settings.player_killed_drop_equipment || !IsAKill && Settings.player_downed_drop_equipment)
                    || (IsAKill && Settings.nonplayer_killed_drop_equipment || !IsAKill && Settings.nonplayer_downed_drop_equipment)))
            {
                pawn.equipment.DropAllEquipment(pawn.PositionHeld, true);
            }
            //
            if (pawn.inventory != null && pawn.inventory.innerContainer.TotalStackCount > 0
                && (pawn.IsColonistPlayerControlled && (IsAKill && Settings.player_killed_drop_inventory || !IsAKill && Settings.player_downed_drop_inventory)
                    || (IsAKill && Settings.nonplayer_killed_drop_inventory || !IsAKill && Settings.nonplayer_downed_drop_inventory)))
            {
                pawn.inventory.DropAllNearPawn(pawn.PositionHeld, true, false);
            }
            else
            {
                DropUnmarkableNearPawn(pawn.inventory, pawn.PositionHeld, true, false);
            }
        }

        static readonly Color hColor = new Color(1f, 0.8f, 0.8f, 1f);

        public static void DrawThingRow(Pawn pawn, ref float y, ref float width, Thing thing, ref bool inventory)
        {
            Corpse corpse = pawn.Corpse;

            inventory = inventory && corpse == null;

            if (pawn.Downed || corpse != null || pawn.IsPrisoner && pawn.guest.PrisonerIsSecure)
            {
                CompStripChecker c = CompStripChecker.GetChecker(thing, false);
                if (c == null)
                {
                    width -= 24f;
                    return;
                }

                ThingWithComps holder = corpse == null ? pawn : (ThingWithComps)corpse;
                Rect rect = new Rect(0f, y, width, 28f);

                Rect rect2 = new Rect(rect.width - 24f, y, 24f, 24f);

                if (c.ShouldStrip)
                {
                    TooltipHandler.TipRegion(rect2, "StripThingCancel".Translate());

                    //weird shenanigans with colors
                    var cl = GUI.color;
                    if (Widgets.ButtonImage(rect2, ContentFinder<Texture2D>.Get("UI/Icons/Strip_Thing_Cancel"), hColor))
                    {
                        SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                        SetShouldStrip(false, c, pawn, holder);
                    }
                    GUI.color = cl;
                }
                else
                {
                    TooltipHandler.TipRegion(rect2, "StripThing".Translate());
                    if (Widgets.ButtonImage(rect2, ContentFinder<Texture2D>.Get("UI/Icons/Strip_Thing")))
                    {
                        SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                        SetShouldStrip(true, c, pawn, holder);
                    }
                }
                width -= 24f;
            }
            return;
        }

        // Signals the CompStripChecker and toggles the designation if required
        public static void SetShouldStrip(bool shouldStrip, CompStripChecker c, Pawn pawn, ThingWithComps holder)
        {
            if (!shouldStrip) c.ShouldStrip = shouldStrip;
            // is it not marked yet?
            if (CompStripChecker.getFirstMarked(pawn) == null)
            {
                var designation = holder.Map?.designationManager.DesignationOn(holder, DesignationDefOf.Strip);
                bool stripping = designation != null;

                //                   | - | 🡓 | 🡓 | -
                // shouldStrip       | t | f | t | f
                // stripping         | t | t | f | f
                // AddDesignation    | f | f | t | f
                // RemoveDesignation | f | t | f | f

                if (shouldStrip != stripping)
                {
                    if (stripping)
                    {
                        holder.Map.designationManager.RemoveDesignation(designation);
                    }
                    else
                    {
                        holder.Map.designationManager.AddDesignation(new Designation(holder, DesignationDefOf.Strip));
                    }
                }
            }

            if (shouldStrip) c.ShouldStrip = shouldStrip;
        }
    }
}
