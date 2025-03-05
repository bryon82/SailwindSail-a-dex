using System.Reflection;
using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Configuration;
using BepInEx.Bootstrap;

namespace sailadex
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(MODSAVEBACKUPS_GUID, MODSAVEBACKUPS_VERSION)]
    [BepInDependency(PASSAGEDUDE_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(RANDOMENCOUNTERS_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.raddude82.sailadex";
        public const string PLUGIN_NAME = "Sail-A-Dex";
        public const string PLUGIN_VERSION = "1.4.3";

        public const string MODSAVEBACKUPS_GUID = "com.raddude82.modsavebackups";
        public const string MODSAVEBACKUPS_VERSION = "1.1.1";
        public const string PASSAGEDUDE_GUID = "pr0skynesis.passagedude";
        public const string RANDOMENCOUNTERS_GUID = "com.raddude82.randomencounters";

        internal static ConfigEntry<bool> fishNamesHidden;
        internal static ConfigEntry<bool> portNamesHidden;
        internal static ConfigEntry<bool> fishCaughtUIEnabled;
        internal static ConfigEntry<bool> portsVisitedUIEnabled;
        internal static ConfigEntry<bool> statsUIEnabled;
        internal static ConfigEntry<bool> notificationsEnabled;
        internal static ConfigEntry<float> notificationSoundVolume;
        internal static ConfigEntry<string> updateMilesSailed;

        internal static Plugin instance;
        internal static ManualLogSource logger;
        internal static Harmony harmony;

        private void Awake()
        {            
            fishNamesHidden = Config.Bind("Settings", "Hide Fish Names Before Caught", true, "true = fish names will be hidden before being caught for the first time.");
            portNamesHidden = Config.Bind("Settings", "Hide Port Names Before Visited", false, "true = port names will be hidden before visited for the first time.");
            fishCaughtUIEnabled = Config.Bind("Settings", "Enable Fish Caught UI", true, "true = UI for how many fish you caught will be enabled. Setting to false, continuing a game where previously enabled, and then saving will erase all previous fish caught progress.");
            portsVisitedUIEnabled = Config.Bind("Settings", "Enable Ports Visited UI", true, "true = UI for which ports you have visited will be enabled. Setting to false, continuing a game where previously enabled, and then saving will erase all previous port visit progress.");
            statsUIEnabled = Config.Bind("Settings", "Enable Stats UI", true, "true = UI for various stats will be enabled. Setting to false, continuing a game where previously enabled, and then saving will erase all previously recorded stats.");
            notificationsEnabled = Config.Bind("Settings", "Enable Notifications", true, "true = notifications on badge earned will be enabled.");
            notificationSoundVolume = Config.Bind("Settings", "Notification Volume", 0.2f, "Above 1f is loud and not recommended. Set to 0f to disable.");
            updateMilesSailed = Config.Bind("Settings", "Miles Sailed Updates", "moored", new ConfigDescription("Miles sailed text will be updated once moored, going to sleeping or moored, or in real time.", new AcceptableValueList<string>("moored", "sleep", "realtime")) );

            instance = this;
            logger = Logger;
            harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_GUID);

            foreach (var plugin in Chainloader.PluginInfos)
            {
                var metadata = plugin.Value.Metadata;
                if (metadata.GUID.Equals(PASSAGEDUDE_GUID))
                {
                    logger.LogInfo($"{PASSAGEDUDE_GUID} found");
                    PassageDude.PatchMod();
                }
                if (metadata.GUID.Equals(RANDOMENCOUNTERS_GUID))
                {
                    logger.LogInfo($"{RANDOMENCOUNTERS_GUID} found");
                    RandomEncounters.pluginInstance = plugin.Value.Instance;
                    RandomEncounters.PatchMod();
                }
            }            
        }
    }
}
