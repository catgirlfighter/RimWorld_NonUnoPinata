using UnityEngine;
using Verse;

namespace NonUnoPinata
{
    public class Settings : ModSettings
    {
        public static bool strip_apparel = false;
        public static bool strip_equipment = true;
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

        public static void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.Begin(inRect);
            listing_Standard.Label("cremated".Translate());
            listing_Standard.Label("strip_designated".Translate());
            listing_Standard.CheckboxLabeled("strip_apparel".Translate(), ref strip_apparel);
            listing_Standard.CheckboxLabeled("strip_equipment".Translate(), ref strip_equipment);
            listing_Standard.CheckboxLabeled("strip_inventory".Translate(), ref strip_inventory);
            listing_Standard.CheckboxLabeled("corpse_display_equipment".Translate(), ref corpse_display_equipment);

            listing_Standard.GapLine();
            listing_Standard.Label("downed".Translate());
            listing_Standard.CheckboxLabeled("player_downed_drop_equipment".Translate(), ref player_downed_drop_equipment);
            listing_Standard.CheckboxLabeled("player_downed_drop_inventory".Translate(), ref player_downed_drop_inventory);
            listing_Standard.CheckboxLabeled("nonplayer_downed_drop_equipment".Translate(), ref nonplayer_downed_drop_equipment);
            listing_Standard.CheckboxLabeled("nonplayer_downed_drop_inventory".Translate(), ref nonplayer_downed_drop_inventory);

            listing_Standard.GapLine();
            listing_Standard.Label("killed".Translate());
            listing_Standard.CheckboxLabeled("player_killed_drop_equipment".Translate(), ref player_killed_drop_equipment);
            listing_Standard.CheckboxLabeled("player_killed_drop_inventory".Translate(), ref player_killed_drop_inventory);
            listing_Standard.CheckboxLabeled("nonplayer_killed_drop_equipment".Translate(), ref nonplayer_killed_drop_equipment);
            listing_Standard.CheckboxLabeled("nonplayer_killed_drop_inventory".Translate(), ref nonplayer_killed_drop_inventory);
            


            listing_Standard.End();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref strip_apparel, "strip_apparel", false, false);
            Scribe_Values.Look(ref strip_equipment, "strip_equipment", true, false);
            Scribe_Values.Look(ref strip_inventory, "strip_inventory", true, false);

            Scribe_Values.Look(ref player_downed_drop_equipment, "player_downed_drop_equipment", false, false);
            Scribe_Values.Look(ref player_downed_drop_inventory, "player_downed_drop_inventory", false, false);
            Scribe_Values.Look(ref nonplayer_downed_drop_equipment, "nonplayer_downed_drop_equipment", false, false);
            Scribe_Values.Look(ref nonplayer_downed_drop_inventory, "nonplayer_downed_drop_inventory", false, false);

            Scribe_Values.Look(ref player_killed_drop_equipment, "player_killed_drop_equipment", false, false);
            Scribe_Values.Look(ref player_killed_drop_inventory, "player_killed_drop_inventory", false, false);
            Scribe_Values.Look(ref nonplayer_killed_drop_equipment, "nonplayer_killed_drop_equipment", false, false);
            Scribe_Values.Look(ref nonplayer_killed_drop_inventory, "nonplayer_killed_drop_inventory", false, false);
        }
    }
}
