using System.Linq;
//using Multiplayer.API;
using Verse;

namespace NonUnoPinata
{
    // temp disabled due to 1.5
    /*
    [StaticConstructorOnStartup]
    static class Multiplayer
    {
        static Multiplayer() {
            if (!MP.enabled) return; // Only runs whenever MP is enabled

            // This method is called whenever the user clicks the strip button in the ITab
            MP.RegisterSyncMethod(typeof(NUPUtility), nameof(NUPUtility.SetShouldStrip));

            // Register the worker that encodes/decodes the CompStripChecker
            MP.RegisterSyncWorker<CompStripChecker>(SyncWorkerForCompStripChecker);

            // Under Multiplayer we can't have the state altered without a SyncMethod.
            // CompStripChecker.GetChecker creates the comp if not found, which is disallowed.
            // To keep the logic being used in ITab_Pawn_GearPatch, it's required that the comp exists.
            // This workaround has no impact on performance
            InjectComp();
        }

        // Sends the ThingWithComps parent for reference on write
        // Retrieves CompStripChecker from the parent on read
        static void SyncWorkerForCompStripChecker(SyncWorker sw, ref CompStripChecker comp)
        {
            if (sw.isWriting) {
                sw.Write(comp.parent);
            } else {
                comp = sw.Read<ThingWithComps>().GetComp<CompStripChecker>();
            }
        }

        // Searches for haulable items and adds the CompStripChecker
        static void InjectComp()
        {
            var compProperties = new Verse.CompProperties { compClass = typeof(CompStripChecker) };
            var defs = DefDatabase<ThingDef>.AllDefs.Where(
                def => typeof(ThingWithComps).IsAssignableFrom(def.thingClass)
                    && def.EverHaulable
                    && !def.HasComp(typeof(CompStripChecker))
            );
            foreach (var def in defs) {
                def.comps.Add(compProperties);
            }
        }
    }
    */
}