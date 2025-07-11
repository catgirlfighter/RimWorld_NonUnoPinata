using HarmonyLib;
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
            var harmony = new Harmony("net.avilmask.rimworld.mod.NonUnoPinata");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            GetSettings<Settings>();
        }

        public void Save()
        {
            LoadedModManager.GetMod<NonUnoPinata>().GetSettings<Settings>().Write();
        }

        public override string SettingsCategory()
        {
            return "Non Uno Pinata";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }
}