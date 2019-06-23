using Harmony;
using System.Reflection;
using Verse;
using UnityEngine;
using RimWorld;

namespace NonUnoPinata
{
    [StaticConstructorOnStartup]
    public class NonUnoPinata : Mod
    {
        public NonUnoPinata(ModContentPack content) : base(content)
        {
            var harmony = HarmonyInstance.Create("net.avilmask.rimworld.mod.NonUnoPinata");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}