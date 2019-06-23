﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Text;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;


namespace NonUnoPinata
{
    class Toils_RecipePatch
    {
        [HarmonyPatch(typeof(Toils_Recipe), "CalculateIngredients")]
        static class Toils_Recipe_CalculateIngredients_NonUnoPinataPatch
        {
            static bool doStripCorpse(RecipeDef def, Thing thing)
            {
                if (!def.autoStripCorpses)
                {
                    IStrippable strippable = thing as IStrippable;
                    return strippable != null && thing.MapHeld != null && thing.MapHeld.designationManager.DesignationOn(thing, DesignationDefOf.Strip) != null;
                }
                else
                    return true;
            }

            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, MethodBase mb)
            {
                FieldInfo f = AccessTools.Field(typeof(RecipeDef), "autoStripCorpses");
                foreach (var i in instructions)
                {
                    if (i.opcode == OpCodes.Ldfld && i.operand == f)
                    {
                        yield return new CodeInstruction(OpCodes.Ldloc_S, (byte)5);
                        yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Toils_Recipe_CalculateIngredients_NonUnoPinataPatch), nameof(Toils_Recipe_CalculateIngredients_NonUnoPinataPatch.doStripCorpse)));
                    }
                    else
                    {
                        yield return i;
                    }

                }
            }
        }
    }
}
