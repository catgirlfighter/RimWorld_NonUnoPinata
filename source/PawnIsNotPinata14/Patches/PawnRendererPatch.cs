using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace NonUnoPinata.Patches
{
    //private void RenderPawnInternal(Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
    //private void RenderPawnInternal(Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, RotDrawMode bodyDrawType, PawnRenderFlags flags)
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(RotDrawMode), typeof(PawnRenderFlags) })]
    static class PawnRenderer_RenderPawnInternal_NonUnoPinataPatch
    {
        static void renderStaticEquipment(Pawn pawn, Rot4 bodyFacing, Vector3 vector, Quaternion quaternion, PawnRenderFlags flags)
        {
            if (!Settings.corpse_display_equipment || flags.FlagSet(PawnRenderFlags.Portrait) || !pawn.Dead || pawn.Spawned || pawn.equipment == null || pawn.equipment.Primary == null) return;
            var eq = pawn.equipment.Primary;
            var offset = eq.def.equippedAngleOffset % 360f;
            Graphic_StackCount graphic_StackCount = eq.Graphic as Graphic_StackCount;
            Material matSingle;
            if (graphic_StackCount == null)
                matSingle = eq.Graphic.MatSingle;
            else
                matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;

            float an = 0;
            Vector3 ax = Vector3.up;
            quaternion.ToAngleAxis(out an, out ax);
            GenDraw.DrawMeshNowOrLater(MeshPool.plane10, vector + quaternion * new Vector3(0, 0, -0.22f) + new Vector3(0f, 0.0367346928f, 0f), Quaternion.AngleAxis(-45f - (an - 180f) * 0.2f + offset, Vector3.up), matSingle, flags.FlagSet(PawnRenderFlags.DrawNow));
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase mb)
        {
            List<CodeInstruction> l = instructions.ToList();
            object label = null;
            MethodInfo LrenderStaticEquipment = AccessTools.Method(typeof(PawnRenderer_RenderPawnInternal_NonUnoPinataPatch), nameof(PawnRenderer_RenderPawnInternal_NonUnoPinataPatch.renderStaticEquipment));
            FieldInfo Lpawn = AccessTools.Field(typeof(PawnRenderer), "pawn");
            bool b = false;
            for (int i = 0; i < l.Count; i++)
            {
                if ((l[i].opcode == OpCodes.Brfalse_S || l[i].opcode == OpCodes.Brfalse) && l[i - 1].opcode == OpCodes.Ldarg_3)
                    label = l[i].operand;
                else if (label != null && l[i].labels.Contains((Label)label))
                {
                    b = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, Lpawn);
                    yield return new CodeInstruction(OpCodes.Ldarg_S, (byte)4);
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_S, (byte)6);
                    yield return new CodeInstruction(OpCodes.Call, LrenderStaticEquipment);
                }
                yield return l[i];
            }
            if (!b) Log.Error("Couldn't patch PawnRenderer.RenderPawnInternal");
        }
    }
}
 