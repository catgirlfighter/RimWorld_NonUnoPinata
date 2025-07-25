﻿using UnityEngine;
using Verse;
using System;

namespace NonUnoPinata
{
    public class Settings : ModSettings
    {
        public static bool strip_apparel = false;
        public static bool strip_untainted = false;
        public static bool strip_equipment = true;
        public static bool strip_smeltable = true;
        public static bool strip_inventory = true;
        public static bool player_downed_drop_equipment = false;
        public static bool player_downed_drop_inventory = false;
        public static bool player_killed_drop_equipment = false;
        public static bool player_killed_drop_inventory = false;
        public static bool nonplayer_downed_drop_equipment = false;
        public static bool nonplayer_downed_drop_inventory = false;
        public static bool nonplayer_killed_drop_equipment = false;
        public static bool nonplayer_killed_drop_inventory = false;
        public static bool corpse_display_equipment = false;
        public static bool allow_cremate_nonburnable = false;
        public static bool strip_despite_autoStripCorpses = false;
        public static bool strip_apparel_b = true;
        public static bool strip_untainted_b = false;
        public static bool strip_equipment_b = true;
        public static bool strip_smeltable_b = false;
        public static bool strip_inventory_b = true;
        public static bool allow_cremate_nonburnable_b = false;
        private static Vector2 ScrollPos = Vector2.zero;

        public static StripFlags GetStripFlags(bool autoStripCorpses)
        {
            StripFlags flags = StripFlags.None;

            if (autoStripCorpses)
            {
                if (strip_apparel_b) flags |= StripFlags.Apparel;
                if (strip_equipment_b) flags |= StripFlags.Equipment;
                if (strip_inventory_b) flags |= StripFlags.Inventory;
                if (strip_smeltable_b) flags |= StripFlags.Smeltable;
                if (strip_untainted_b) flags |= StripFlags.Untainted;
                if (!allow_cremate_nonburnable_b) flags |= StripFlags.Unburnable;
            }
            else
            {
                if (strip_apparel) flags |= StripFlags.Apparel;
                if (strip_equipment) flags |= StripFlags.Equipment;
                if (strip_inventory) flags |= StripFlags.Inventory;
                if (strip_smeltable) flags |= StripFlags.Smeltable;
                if (strip_untainted) flags |= StripFlags.Untainted;
                if (!allow_cremate_nonburnable) flags |= StripFlags.Unburnable;
            }
            return flags;
        }

        public static void DoSettingsWindowContents(Rect inRect)
        {
            int rowCount = 30;
            Rect viewRect = new Rect(0f, 0f, inRect.width, rowCount * 26f);
            viewRect.xMax *= 0.9f;
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(viewRect);
            GUI.EndGroup();
            Widgets.BeginScrollView(inRect, ref ScrollPos, viewRect);
            //
            listing_Standard.CheckboxLabeled("corpse_display_equipment".Translate(), ref corpse_display_equipment, "");
            listing_Standard.Label("cremated".Translate());
            listing_Standard.Label("strip_designated".Translate());
            listing_Standard.CheckboxLabeled("allow_cremate_nonburnable".Translate(), ref allow_cremate_nonburnable, "");
            listing_Standard.CheckboxLabeled("strip_apparel".Translate(), ref strip_apparel, "");
            if(strip_apparel)
                listing_Standard.CheckboxLabeled("strip_untainted".Translate(), ref strip_untainted, "");
            listing_Standard.CheckboxLabeled("strip_equipment".Translate(), ref strip_equipment, "");
            listing_Standard.CheckboxLabeled("strip_smeltable".Translate(), ref strip_smeltable, "");
            listing_Standard.CheckboxLabeled("strip_inventory".Translate(), ref strip_inventory, "");

            listing_Standard.GapLine();
            listing_Standard.CheckboxLabeled("butchered".Translate(), ref strip_despite_autoStripCorpses, "");
            if (strip_despite_autoStripCorpses)
            {
                listing_Standard.Label("strip_designated".Translate());
                listing_Standard.CheckboxLabeled("allow_cremate_nonburnable".Translate(), ref allow_cremate_nonburnable_b, "");
                listing_Standard.CheckboxLabeled("strip_apparel".Translate(), ref strip_apparel_b, "");
                if (strip_apparel_b)
                    listing_Standard.CheckboxLabeled("strip_untainted".Translate(), ref strip_untainted_b, "");
                listing_Standard.CheckboxLabeled("strip_equipment".Translate(), ref strip_equipment_b, "");
                listing_Standard.CheckboxLabeled("strip_smeltable".Translate(), ref strip_smeltable_b, "");
                listing_Standard.CheckboxLabeled("strip_inventory".Translate(), ref strip_inventory_b, "");
            }

            listing_Standard.GapLine();
            listing_Standard.Label("downed".Translate());
            listing_Standard.CheckboxLabeled("player_downed_drop_equipment".Translate(), ref player_downed_drop_equipment, "");
            listing_Standard.CheckboxLabeled("player_downed_drop_inventory".Translate(), ref player_downed_drop_inventory, "");
            listing_Standard.CheckboxLabeled("nonplayer_downed_drop_equipment".Translate(), ref nonplayer_downed_drop_equipment, "");
            listing_Standard.CheckboxLabeled("nonplayer_downed_drop_inventory".Translate(), ref nonplayer_downed_drop_inventory, "");

            listing_Standard.GapLine();
            listing_Standard.Label("killed".Translate());
            listing_Standard.CheckboxLabeled("player_killed_drop_equipment".Translate(), ref player_killed_drop_equipment, "");
            listing_Standard.CheckboxLabeled("player_killed_drop_inventory".Translate(), ref player_killed_drop_inventory, "");
            listing_Standard.CheckboxLabeled("nonplayer_killed_drop_equipment".Translate(), ref nonplayer_killed_drop_equipment, "");
            listing_Standard.CheckboxLabeled("nonplayer_killed_drop_inventory".Translate(), ref nonplayer_killed_drop_inventory, "");
            Widgets.EndScrollView();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref strip_apparel, "strip_apparel", false, false);
            Scribe_Values.Look(ref strip_equipment, "strip_equipment", true, false);
            Scribe_Values.Look(ref strip_inventory, "strip_inventory", true, false);
            Scribe_Values.Look(ref strip_smeltable, "strip_smeltable", true, false);
            Scribe_Values.Look(ref strip_untainted, "strip_untainted", false, false);
            Scribe_Values.Look(ref allow_cremate_nonburnable, "allow_cremate_nonburnable", false, false);
            Scribe_Values.Look(ref corpse_display_equipment, "corpse_display_equipment", true, false);

            Scribe_Values.Look(ref player_downed_drop_equipment, "player_downed_drop_equipment", false, false);
            Scribe_Values.Look(ref player_downed_drop_inventory, "player_downed_drop_inventory", false, false);
            Scribe_Values.Look(ref nonplayer_downed_drop_equipment, "nonplayer_downed_drop_equipment", false, false);
            Scribe_Values.Look(ref nonplayer_downed_drop_inventory, "nonplayer_downed_drop_inventory", false, false);

            Scribe_Values.Look(ref player_killed_drop_equipment, "player_killed_drop_equipment", false, false);
            Scribe_Values.Look(ref player_killed_drop_inventory, "player_killed_drop_inventory", false, false);
            Scribe_Values.Look(ref nonplayer_killed_drop_equipment, "nonplayer_killed_drop_equipment", false, false);
            Scribe_Values.Look(ref nonplayer_killed_drop_inventory, "nonplayer_killed_drop_inventory", false, false);

            Scribe_Values.Look(ref strip_despite_autoStripCorpses, "strip_despite_autoStripCorpses", false, false);
            Scribe_Values.Look(ref strip_apparel_b, "strip_apparel_b", true, false);
            Scribe_Values.Look(ref strip_equipment_b, "strip_equipment_b", true, false);
            Scribe_Values.Look(ref strip_inventory_b, "strip_inventory_b", true, false);
            Scribe_Values.Look(ref strip_smeltable_b, "strip_smeltable_b", false, false);
            Scribe_Values.Look(ref strip_untainted_b, "strip_untainted_b", false, false);
            Scribe_Values.Look(ref allow_cremate_nonburnable_b, "allow_cremate_nonburnable_b", false, false);
        }
    }
}
