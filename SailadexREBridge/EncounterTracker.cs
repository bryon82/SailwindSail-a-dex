using System.Collections.Generic;
using RandomEncounters.API;
using sailadex;
using static sailadex.SAD_Plugin;

namespace RandomEncountersBridge
{
    public static class EncounterTracker
    {
        private static readonly Dictionary<string, string> _encounterNames = new Dictionary<string, string>()
        {
            { "Flotsam", "FlotsamEncounters" },
            { "Dense Fog", "DenseFogEncounters" },
            { "Whales", "SeaLifeEncounters" },
            { "Fishing Bonanza", "FishingBonanzaEncounters" },
            { "Intense Storm", "IntenseStormEncounters" }
        };

        public static void TrackEncounters()
        {
            LogDebug("Tracking encounters from RandomEncounters mod.");
            EncounterEvents.EncounterTriggered += enc => LogInfo("Encounter triggered: " + enc.Name);
            EncounterEvents.EncounterCompleted += enc => EncounterCompleted(enc.Name);
        }

        private static void EncounterCompleted(string encounterName)
        {
            LogInfo($"Encounter completed: {encounterName}");

            if (_encounterNames.TryGetValue(encounterName, out string statName))
                StatsUI.Instance.IncrementIntStat(statName);
        }
    }
}
