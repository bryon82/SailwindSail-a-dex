
using BepInEx;
using BepInEx.Configuration;
using System;
using System.Linq;
using HarmonyLib;
using System.Reflection;

namespace sailadex
{
    internal class RandomEncounters
    {
        internal static BaseUnityPlugin pluginInstance;
        internal static bool IsSeaLifeEnabled 
        { 
            get 
            {
                var seaLifeInstalled = pluginInstance.GetPrivateField<BaseUnityPlugin>("seaLifeModInstance");
                var seaLifeControlled = pluginInstance.GetPrivateField<ConfigEntry<bool>>("controlSeaLifeMod");
                return seaLifeInstalled != null && seaLifeControlled.Value;
            }
        }

        internal static bool FlotsamEnabled { 
            get
            {
                return pluginInstance.GetPrivateField<ConfigEntry<bool>>("enableFlotsam").Value;
            }
        }

        internal static bool DenseFogEnabled
        {
            get 
            {
                return pluginInstance.GetPrivateField<ConfigEntry<bool>>("enableDenseFog").Value;
            }
        }
        internal static bool FishingBonanzaEnabled
        {
            get
            {
                return pluginInstance.GetPrivateField<ConfigEntry<bool>>("enableFishingBonanza").Value;
            }
        }
        internal static bool IntenseStormEnabled
        {
            get
            {
                return pluginInstance.GetPrivateField<ConfigEntry<bool>>("enableIntenseStorm").Value;
            }
        }

        public static void PatchMod()
        {
            Type encounterGeneratorClass = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(type => type.IsClass && type.Name == "EncounterGenerator")
                .Single();

            var methodsToPatch = new (string originalMethodName, string patchMethodName)[]
            {
                ("GenerateFlotsam", "FlotsamCountPatch"),
                ("GenerateDenseFog", "DenseFogCountPatch"),
                ("GenerateWhale", "SeaLifeCountPatch"),
                ("GenerateFishingBonanza", "FishingBonanzaCountPatch"),
                ("GenerateIntenseStorm", "IntenseStormCountPatch")
            };
            
            foreach (var (originalMethodName, patchMethodName) in methodsToPatch)
            {
                MethodInfo original = AccessTools.Method(encounterGeneratorClass, originalMethodName);
                MethodInfo patch = AccessTools.Method(typeof(EncounterGeneratorPatches), patchMethodName);
                SAD_Plugin.harmony.Patch(original, new HarmonyMethod(patch));
            }
        }

        public class EncounterGeneratorPatches
        {
            [HarmonyPostfix]
            public static void FlotsamCountPatch()
            {
                if (FlotsamEnabled)
                    StatsUI.Instance.IncrementIntStat("FlotsamEncounters");
            }

            [HarmonyPostfix]
            public static void DenseFogCountPatch()
            {
                if (DenseFogEnabled)
                    StatsUI.Instance.IncrementIntStat("DenseFogEncounters");
            }

            [HarmonyPostfix]
            public static void SeaLifeCountPatch()
            {
                if (IsSeaLifeEnabled)
                    StatsUI.Instance.IncrementIntStat("SeaLifeEncounters");
            }

            [HarmonyPostfix]
            public static void FishingBonanzaCountPatch()
            {
                if (FishingBonanzaEnabled)
                    StatsUI.Instance.IncrementIntStat("FishingBonanzaEnc");
            }

            [HarmonyPostfix]
            public static void IntenseStormCountPatch()
            {
                if (IntenseStormEnabled)
                    StatsUI.Instance.IncrementIntStat("IntenseStormEnc");
            }
        }
    }
}
