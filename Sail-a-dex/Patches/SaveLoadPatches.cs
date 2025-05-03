using BepInEx.Logging;
using HarmonyLib;
using ModSaveBackups;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sailadex
{
    internal class SaveLoadPatches
    {
        static readonly ManualLogSource logger = SAD_Plugin.logger;

        [HarmonyPatch(typeof(SaveLoadManager))]
        private class SaveLoadManagerPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("SaveModData")]
            public static void DoSaveGamePatch()
            {
                var saveContainer = new SailadexSaveContainer
                {
                    caughtFish = FishCaughtUI.Instance.CaughtFish.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value),

                    fishBadges = FishCaughtUI.Instance.FishBadges.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value),

                    visitedPorts = PortsVisitedUI.Instance.VisitedPorts.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value),

                    portBadges = PortsVisitedUI.Instance.PortBadges.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value),

                    floatStats = StatsUI.Instance.FloatStats.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value),

                    intStats = StatsUI.Instance.IntStats.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value),

                    boolArrayStats = StatsUI.Instance.BoolArrayStats.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value)
                };

                ModSave.Save(SAD_Plugin.Instance.Info, saveContainer);
            }

            [HarmonyPostfix]
            [HarmonyPatch("LoadModData")]
            public static void LoadModDataPatch()
            {
                if (!ModSave.Load(SAD_Plugin.Instance.Info, out SailadexSaveContainer saveContainer))
                { 
                    logger.LogWarning("Save file loading failed. If this is the first time loading this save with this mod, this is normal.");
                    return;
                }

                if (saveContainer.caughtFish != null)
                    FishCaughtUI.Instance.LoadCaughtFish(saveContainer.caughtFish);
                
                if (saveContainer.fishBadges != null)
                    FishCaughtUI.Instance.LoadFishBadges(saveContainer.fishBadges);

                if (saveContainer.visitedPorts != null)
                    PortsVisitedUI.Instance.LoadVisitedPorts(saveContainer.visitedPorts);
                
                if (saveContainer.portBadges != null)
                    PortsVisitedUI.Instance.LoadPortBadges(saveContainer.portBadges);
                
                if (saveContainer.floatStats != null)
                    StatsUI.Instance.LoadFloatStats(saveContainer.floatStats);

                if (saveContainer.intStats != null)
                    StatsUI.Instance.LoadIntStats(saveContainer.intStats);

                if (saveContainer.boolArrayStats != null)
                {
                    StatsUI.Instance.LoadBoolArrayStats(saveContainer.boolArrayStats);
                }                
            }           
        }

        public static void LoadDictionary<T>(Dictionary<string, T> saveDict, Dictionary<string, T> gameDict)
        {
            foreach (KeyValuePair<string, T> item in saveDict)
            {
                if (!gameDict.ContainsKey(item.Key))
                {
                    logger.LogWarning($"LoadData: {item.Key} not found in game");
                    continue;
                }                
                gameDict[item.Key] = item.Value;
            }
        }
    }

    [Serializable]
    public class SailadexSaveContainer
    {
        public Dictionary<string, int> caughtFish;
        public Dictionary<string, bool> visitedPorts;
        public Dictionary<string, bool> fishBadges;
        public Dictionary<string, bool> portBadges;
        public Dictionary<string, float> floatStats;
        public Dictionary<string, int> intStats;
        public Dictionary<string, bool[]> boolArrayStats;
    }
}
