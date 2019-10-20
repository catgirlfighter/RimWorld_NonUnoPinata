using System.Collections.Generic;
using Harmony;
using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;
using System.Reflection;
using System.Reflection.Emit;


namespace NonUnoPinata
{
    class ITab_Pawn_GearPatch
    {
        [HarmonyPatch(typeof(ITab_Pawn_Gear), "DrawThingRow")]
        public static class ITab_Pawn_Gear_DrawThingRow_NonUnoPinataPatch
        {
            static readonly Color hColor = new Color(1f, 0.8f, 0.8f, 1f);

            public static bool Prefix(ITab_Pawn_Gear __instance, ref float y, ref float width, Thing thing, ref bool inventory)
            {
                Pawn SelPawnForGear = Traverse.Create(__instance).Property("SelPawnForGear").GetValue<Pawn>();
                Corpse corpse = SelPawnForGear.Corpse;

                inventory = inventory && corpse == null;

                if (SelPawnForGear.Downed || corpse != null || SelPawnForGear.IsPrisoner && SelPawnForGear.guest.PrisonerIsSecure)
                {
                    CompStripChecker c = CompStripChecker.GetChecker(thing, false);
                    if (c == null)
                    {
                        width -= 24f;
                        return true;
                    }

                    ThingWithComps holder = corpse == null ? SelPawnForGear : (ThingWithComps)corpse;
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
                            c.ShouldStrip = false;
                            if (CompStripChecker.getFirstMarked(SelPawnForGear) == null && holder.Map != null)
                            {
                                Designation d = holder.Map.designationManager.DesignationOn(holder, DesignationDefOf.Strip);
                                if (d != null) holder.Map.designationManager.RemoveDesignation(d);
                            }
                        }
                        GUI.color = cl;
                    }
                    else
                    {
                        TooltipHandler.TipRegion(rect2, "StripThing".Translate());
                        if (Widgets.ButtonImage(rect2, ContentFinder<Texture2D>.Get("UI/Icons/Strip_Thing")))
                        {
                            SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                            bool b = CompStripChecker.getFirstMarked(SelPawnForGear) == null && holder.Map.designationManager.DesignationOn(holder, DesignationDefOf.Strip) == null;
                            c.ShouldStrip = true;
                            if (b) holder.Map.designationManager.AddDesignation(new Designation(holder, DesignationDefOf.Strip));
                        }
                    }
                    width -= 24f;
                }
                return true;
            }
        }
    }
}
