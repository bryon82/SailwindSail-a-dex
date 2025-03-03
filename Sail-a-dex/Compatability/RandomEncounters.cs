
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
        internal static bool isSeaLifeEnabled;
        internal static bool flotsamEnabled;
        internal static bool denseFogEnabled;

        public static void PatchMod()
        {
            var seaLifeInstalled = pluginInstance.GetPrivateField<BaseUnityPlugin>("seaLifeModInstance");
            var seaLifeControlled = pluginInstance.GetPrivateField<ConfigEntry<bool>>("controlSeaLifeMod");
            isSeaLifeEnabled = seaLifeInstalled != null && seaLifeControlled.Value;
            flotsamEnabled = pluginInstance.GetPrivateField<ConfigEntry<bool>>("enableFlotsam").Value;
            denseFogEnabled = pluginInstance.GetPrivateField<ConfigEntry<bool>>("enableDenseFog").Value;

            Type encounterGeneratorClass = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(type => type.IsClass && type.Name == "EncounterGenerator")
                .Single();

            MethodInfo flotsamOriginal = AccessTools.Method(encounterGeneratorClass, "GenerateFlotsam");
            MethodInfo flotsamPatch = AccessTools.Method(typeof(EncounterGeneratorPatches), "FlotsamCountPatch");
            Plugin.harmony.Patch(flotsamOriginal, new HarmonyMethod(flotsamPatch));

            MethodInfo denseFogOriginal = AccessTools.Method(encounterGeneratorClass, "GenerateDenseFog");
            MethodInfo denseFogPatch = AccessTools.Method(typeof(EncounterGeneratorPatches), "DenseFogCountPatch");
            Plugin.harmony.Patch(denseFogOriginal, new HarmonyMethod(denseFogPatch));

            MethodInfo sealifeOriginal = AccessTools.Method(encounterGeneratorClass, "GenerateWhale");
            MethodInfo sealifePatch = AccessTools.Method(typeof(EncounterGeneratorPatches), "SeaLifeCountPatch");
            Plugin.harmony.Patch(sealifeOriginal, new HarmonyMethod(sealifePatch));
        }


        public class EncounterGeneratorPatches
        {
            [HarmonyPostfix]
            public static void FlotsamCountPatch()
            {
                if (flotsamEnabled)
                    StatsUI.instance.IncrementIntStat("FlotsamEncounters");
            }

            [HarmonyPostfix]
            public static void DenseFogCountPatch()
            {
                if (denseFogEnabled)
                    StatsUI.instance.IncrementIntStat("DenseFogEncounters");
            }

            [HarmonyPostfix]
            public static void SeaLifeCountPatch()
            {
                if (isSeaLifeEnabled)
                    StatsUI.instance.IncrementIntStat("SeaLifeEncounters");
            }
        }
    }
}
