using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace NonUnoPinata
{
    public class JobDriver_StripEquipment : JobDriver
    {
        // Token: 0x0600337B RID: 13179 RVA: 0x000FEF96 File Offset: 0x000FD196
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
        }

        // Token: 0x0600337C RID: 13180 RVA: 0x0012740A File Offset: 0x0012560A
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOnAggroMentalState(TargetIndex.A);
            this.FailOn(() => !StrippableUtility.CanBeStrippedByColony(base.TargetThingA));
            Toil toil = new Toil();
            toil.initAction = delegate ()
            {
                this.pawn.pather.StartPath(base.TargetThingA, PathEndMode.ClosestTouch);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            toil.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return toil;
            yield return Toils_General.Wait(60, TargetIndex.None).WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            yield return new Toil
            {
                initAction = delegate ()
                {
                    Thing thing = job.targetA.Thing;
                    Corpse corpse = thing as Corpse;
                    Pawn pawn;
                    if (corpse == null)
                        pawn = thing as Pawn; 
                    else
                        pawn = corpse.InnerPawn;
                    //
                    if (pawn == null)
                        Log.Error("target pawn is null");

                    foreach (ThingWithComps thingWithComps in pawn.equipment.AllEquipmentListForReading)
                    {
                        if (thingWithComps.def.IsWeapon)
                        {
                            ThingWithComps tmp;
                            pawn.equipment.TryDropEquipment(thingWithComps, out tmp, thing.Position);
                            break;
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield break;
        }
    }
}
