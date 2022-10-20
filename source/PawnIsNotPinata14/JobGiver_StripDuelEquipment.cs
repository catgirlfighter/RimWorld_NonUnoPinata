using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;

namespace NonUnoPinata
{
    public class JobGiver_StripDuelEquipment : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            Lord lord = pawn.GetLord();
            LordJob_Ritual_Duel duel;
            if ((duel = (((lord != null) ? lord.LordJob : null) as LordJob_Ritual_Duel)) == null)
            {
                return null;
            }
            foreach (var duelist in duel.duelists)
                if ((duelist.Downed || duelist.Dead) && duelist.equipment != null)
                {
                    foreach (ThingWithComps thingWithComps in duelist.equipment.AllEquipmentListForReading)
                    {
                        if (thingWithComps.def.IsWeapon)
                        {
                            if(duelist.Corpse != null)
                                return JobMaker.MakeJob(NonUnoPinataDefOf.NonUnoPinata_StripEquipment, duelist.Corpse);
                            else
                                return JobMaker.MakeJob(NonUnoPinataDefOf.NonUnoPinata_StripEquipment, duelist);
                        }
                    }
                }

            return null;
        }
    }
}
