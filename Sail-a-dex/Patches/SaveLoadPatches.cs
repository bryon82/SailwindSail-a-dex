using HarmonyLib;
using ModSaveBackups;
using System;
using System.Collections.Generic;
using System.IO;
using static sailadex.SAD_Plugin;

namespace sailadex
{
    internal class SaveLoadPatches
    {
        internal static bool loadedNewData = false;

        [HarmonyPatch(typeof(SaveLoadManager))]
        private class SaveLoadManagerPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("LoadModData")]
            public static void LoadModDataOld()
            {
                var oldSavesFile = $"{ModSave.GetSaveDirectory(SaveSlots.currentSlot)}/com.raddude82.sailadex.save";
                if (File.Exists(oldSavesFile))
                {
                    LogDebug($"Found old save file");
                    RenameOldSaves();
                }

                if (loadedNewData)
                {
                    LogDebug("Already loaded mod data from GameState.modData, skipping loading from ModSave file.");
                    return;
                }

                if (!ModSave.Load(Instance.Info, out SailadexSaveContainer saveContainer))
                {
                    LogWarning("Save file loading failed. If this is the first time loading this save with this mod, this is normal.");
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
                if (saveContainer.lastStorm != null)
                {
                    StatsUI.Instance.lastStorm = saveContainer.lastStorm;
                }
                if (saveContainer.lastPortVisited != null)
                {
                    StatsUI.Instance.lastPortVisited = saveContainer.lastPortVisited;
                }
                StatsUI.Instance.IsUnderway = saveContainer.isUnderway;
            }
        }

        public static void LoadDictionary<T>(Dictionary<string, T> saveDict, Dictionary<string, T> gameDict)
        {
            foreach (KeyValuePair<string, T> item in saveDict)
            {
                if (!gameDict.ContainsKey(item.Key))
                {
                    LogWarning($"LoadData: {item.Key} not found in game");
                    continue;
                }
                gameDict[item.Key] = item.Value;
            }
        }

        public static void RenameOldSaves()
        {
            var oldSavesDir = ModSave.GetSaveDirectory(SaveSlots.currentSlot);
            if (Directory.Exists(oldSavesDir))
            {
                foreach (var file in Directory.GetFiles(oldSavesDir))
                {
                    var newFileName = file.Replace("raddude82.sailadex", "raddude.sailadex");
                    LogInfo($"Renaming old sailadex save file {file} to {newFileName}");
                    File.Move(file, newFileName);
                }
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
        public bool isUnderway;
        public string lastStorm;
        public string lastPortVisited;
    }
}
