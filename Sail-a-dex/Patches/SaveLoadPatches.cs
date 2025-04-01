using HarmonyLib;
using ModSaveBackups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace sailadex
{
    internal class SaveLoadPatches
    {
        [HarmonyPatch(typeof(SaveLoadManager))]
        private class SaveLoadManagerPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("SaveModData")]
            public static void DoSaveGamePatch()
            {
                var saveContainer = new SailadexSaveContainer();
               
                saveContainer.caughtFish = FishCaughtUI.instance.caughtFish.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value);

                saveContainer.fishBadges = FishCaughtUI.instance.fishBadges.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value);
                
                saveContainer.visitedPorts = PortsVisitedUI.instance.visitedPorts.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value);

                saveContainer.portBadges = PortsVisitedUI.instance.portBadges.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value);
               
                saveContainer.floatStats = StatsUI.instance.floatStats.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value);

                saveContainer.intStats = StatsUI.instance.intStats.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value);

                saveContainer.boolArrayStats = StatsUI.instance.boolArrayStats.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value);                

                ModSave.Save(Plugin.instance.Info, saveContainer);
            }

            [HarmonyPostfix]
            [HarmonyPatch("LoadModData")]
            public static void LoadModDataPatch()
            {
                if (!ModSave.Load(Plugin.instance.Info, out SailadexSaveContainer saveContainer))
                { 
                    Plugin.logger.LogWarning("Save file loading failed. If this is the first time loading this save with this mod, this is normal.");
                    return;                                                                          
                }

                if (saveContainer.caughtFish != null)
                {
                    //LoadDictionary(saveContainer.caughtFish, FishCaughtUI.instance.caughtFish);
                    ConvertFishNames(saveContainer.caughtFish, FishCaughtUI.instance.caughtFish);
                }

                if (saveContainer.fishBadges != null)
                {
                    LoadDictionary(saveContainer.fishBadges, FishCaughtUI.instance.fishBadges);
                }                

                if (saveContainer.visitedPorts != null)
                {
                    LoadDictionary(saveContainer.visitedPorts, PortsVisitedUI.instance.visitedPorts);
                }

                if (saveContainer.portBadges != null)
                {
                    LoadDictionary(saveContainer.portBadges, PortsVisitedUI.instance.portBadges);
                }                

                if (saveContainer.floatStats != null)
                {
                    // TODO: remove in next major version
                    if (saveContainer.floatStats.ContainsKey("recordGrcDcTransitTime"))
                    {
                        ConvertTransitFloats(saveContainer.floatStats);
                    }
                        
                    LoadDictionary(saveContainer.floatStats, StatsUI.instance.floatStats);                       
                }

                if (saveContainer.intStats != null)
                {
                    // TODO: remove in next major version
                    if (saveContainer.intStats.ContainsKey("recordGrcDcTransitDay"))
                    {
                        ConvertTransitInts(saveContainer.intStats);
                    }

                    LoadDictionary(saveContainer.intStats, StatsUI.instance.intStats);
                }

                if (saveContainer.boolArrayStats != null)
                {
                    // TODO: remove in next major version
                    if (saveContainer.boolArrayStats.ContainsKey("grcTransit"))
                    {
                        ConvertTransitBools(saveContainer.boolArrayStats);
                    }

                    foreach (KeyValuePair<string, bool[]> item in saveContainer.boolArrayStats)
                    {
                        if (StatsUI.instance.boolArrayStats.ContainsKey(item.Key))
                        {
                            StatsUI.instance.boolArrayStats[item.Key] = (bool[])item.Value.Clone();
                            continue;
                        }
                        Plugin.logger.LogWarning($"LoadData: {item.Key} not found in game");
                    }
                }                
            }  
            
            public static void LoadDictionary<T>(Dictionary<string, T> saveDict, Dictionary<string, T> gameDict)
            {
                foreach (KeyValuePair<string, T> item in saveDict)
                {
                    if (gameDict.ContainsKey(item.Key))
                    {
                        gameDict[item.Key] = item.Value;
                        continue;
                    }
                    Plugin.logger.LogWarning($"LoadData: {item.Key} not found in game");
                }                
            }

            // Conversion done to accomodate different way of getting fish names so ffl fish are included
            // TODO: Remove this at next major version
            public static void ConvertFishNames(Dictionary<string, int> oldFishCount, Dictionary<string, int> newFishCount) 
            {
                // load normally if already converted
                if (!oldFishCount.ContainsKey("31 templefish (A)"))
                {
                    LoadDictionary(oldFishCount, newFishCount);
                    return;
                }

                // convert to new naming style
                Plugin.logger.LogDebug("Converting fishNames in fish caught counts");
                foreach (KeyValuePair<string, int> item in oldFishCount)
                {
                    var name = item.Key;
                    if (Regex.IsMatch(name, @"^\d"))
                        name = name.Substring(3, name.IndexOf("(") - 4);
                    newFishCount[name] = item.Value;                    
                }
            }

            // Conversion done to accomodate using regions instead of capitals for transit stats
            // TODO: Remove this at next major version
            public static void ConvertTransitInts(Dictionary<string, int> transitInts)
            {
                Plugin.logger.LogDebug("Converting transit ints");

                for (int i = 0; i < Names.regionTransitNames.Length; i++)
                {
                    transitInts["last" + Names.regionTransitNames[i] + "TransitDay"] = transitInts["last" + Names.transitNames[i] + "TransitDay"];
                    transitInts["record" + Names.regionTransitNames[i] + "TransitDay"] = transitInts["record" + Names.transitNames[i] + "TransitDay"];
                    transitInts.Remove("last" + Names.transitNames[i] + "TransitDay");
                    transitInts.Remove("record" + Names.transitNames[i] + "TransitDay");
                }

                for (int i = 0; i < Names.regions.Length; i++)
                {
                    transitInts[Names.regions[i] + "UnderwayDay"] = transitInts[Names.capitals[i] + "UnderwayDay"];
                    transitInts.Remove(Names.capitals[i] + "UnderwayDay");
                }
            }

            // Conversion done to accomodate using regions instead of capitals for transit stats
            // TODO: Remove this at next major version
            public static void ConvertTransitFloats(Dictionary<string, float> transitFloats)
            {
                Plugin.logger.LogDebug("Converting transit floats");

                for (int i = 0; i < Names.regionTransitNames.Length; i++)
                {
                    transitFloats["last" + Names.regionTransitNames[i] + "TransitTime"] = transitFloats["last" + Names.transitNames[i] + "TransitTime"];
                    transitFloats["record" + Names.regionTransitNames[i] + "TransitTime"] = transitFloats["record" + Names.transitNames[i] + "TransitTime"];
                    transitFloats.Remove("last" + Names.transitNames[i] + "TransitTime");
                    transitFloats.Remove("record" + Names.transitNames[i] + "TransitTime");
                }

                for (int i = 0; i < Names.regions.Length; i++)
                {
                    transitFloats[Names.regions[i] + "UnderwayTime"] = transitFloats[Names.capitals[i] + "UnderwayTime"];
                    transitFloats.Remove(Names.capitals[i] + "UnderwayTime");
                }
            }


            // Conversion done to accomodate using regions instead of capitals for transit stats
            // TODO: Remove this at next major version
            public static void ConvertTransitBools(Dictionary<string, bool[]> transitBools) 
            {
                Plugin.logger.LogDebug("Converting transit bools");

                for (int i = 0; i < Names.regions.Length; i++)
                {
                    transitBools[Names.regions[i] + "Transit"] = (bool[])transitBools[Names.capitals[i] + "Transit"].Clone();
                    transitBools.Remove(Names.capitals[i] + "Transit");
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
    }
}
