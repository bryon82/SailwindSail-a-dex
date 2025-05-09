using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using static sailadex.SAD_Plugin;

namespace sailadex
{
    internal class RandomEncounters
    {
        internal static bool IsSeaLifeModEnabled
        { 
            get 
            {
                var seaLifeInstalled = RE_PluginInstance.GetPrivateField<BaseUnityPlugin>("seaLifeModInstance");
                var seaLifeControlled = RE_PluginInstance.GetPrivateField<ConfigEntry<bool>>("controlSeaLifeMod");
                if (seaLifeControlled != null)
                    return seaLifeInstalled != null && seaLifeControlled.Value;
                return RE_PluginInstance.GetPrivateProperty<bool>("IsSeaLifeModEnabled");
            }
        }

        internal static bool IsFlotsamEnabled
        { 
            get
            {
                if (RE_PluginInstance.GetPrivateField<ConfigEntry<bool>>("enableFlotsam") != null)
                    return RE_PluginInstance.GetPrivateField<ConfigEntry<bool>>("enableFlotsam").Value;
                return RE_PluginInstance.GetPrivateProperty<bool>("IsFlotsamEnabled");
            }
        }

        internal static bool IsDenseFogEnabled
        {
            get 
            {
                if (RE_PluginInstance.GetPrivateField<ConfigEntry<bool>>("enableDenseFog") != null)
                    return RE_PluginInstance.GetPrivateField<ConfigEntry<bool>>("enableDenseFog").Value;
                return RE_PluginInstance.GetPrivateProperty<bool>("IsDenseFogEnabled");
            }
        }
        internal static bool IsFishingBonanzaEnabled
        {
            get
            {
                if (RE_PluginInstance.GetPrivateField<ConfigEntry<bool>>("enableFishingBonanza") != null)
                    return RE_PluginInstance.GetPrivateField<ConfigEntry<bool>>("enableFishingBonanza").Value;
                return RE_PluginInstance.GetPrivateProperty<bool>("IsFishingBonanzaEnabled");
            }
        }
        internal static bool IsIntenseStormEnabled
        {
            get
            {
                if (RE_PluginInstance.GetPrivateField<ConfigEntry<bool>>("enableIntenseStorm") != null)
                    return RE_PluginInstance.GetPrivateField<ConfigEntry<bool>>("enableIntenseStorm").Value;
                return RE_PluginInstance.GetPrivateProperty<bool>("IsIntenseStormEnabled");
            }
        }

        public static void PatchMod()
        {
            var encounterGeneratorClass = AccessTools.TypeByName("EncounterGenerator");
            var methodsToPatch = new (string originalMethodName, string patchMethodName)[]
            {
                ("GenerateFlotsam", "FlotsamCountPatch"),
                ("GenerateDenseFog", "DenseFogCountPatch"),
                ("GenerateWhales", "SeaLifeCountPatch"),
                ("GenerateFishingBonanza", "FishingBonanzaCountPatch"),
                ("GenerateIntenseStorm", "IntenseStormCountPatch")
            };

            foreach (var (originalMethodName, patchMethodName) in methodsToPatch)
            {
                MethodInfo original = AccessTools.Method(encounterGeneratorClass, originalMethodName);
                if (original == null && originalMethodName.Equals("GenerateWhales"))                
                    original = AccessTools.Method(encounterGeneratorClass, "GenerateWhale");                
                MethodInfo patch = AccessTools.Method(typeof(EncounterGeneratorPatches), patchMethodName);
                HarmonyInstance.Patch(original, new HarmonyMethod(patch));
            }
        }

        public class EncounterGeneratorPatches
        {
            [HarmonyPostfix]
            public static void FlotsamCountPatch()
            {
                if (IsFlotsamEnabled)
                    StatsUI.Instance.IncrementIntStat("FlotsamEncounters");
            }

            [HarmonyPostfix]
            public static void DenseFogCountPatch()
            {
                if (IsDenseFogEnabled)
                    StatsUI.Instance.IncrementIntStat("DenseFogEncounters");
            }

            [HarmonyPostfix]
            public static void SeaLifeCountPatch()
            {
                if (IsSeaLifeModEnabled)
                    StatsUI.Instance.IncrementIntStat("SeaLifeEncounters");
            }

            [HarmonyPostfix]
            public static void FishingBonanzaCountPatch()
            {
                if (IsFishingBonanzaEnabled)
                    StatsUI.Instance.IncrementIntStat("FishingBonanzaEnc");
            }

            [HarmonyPostfix]
            public static void IntenseStormCountPatch()
            {
                if (IsIntenseStormEnabled)
                    StatsUI.Instance.IncrementIntStat("IntenseStormEnc");
            }
        }
    }
}
