using System.Reflection;
using BepInEx;
using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Bootstrap;

namespace sailadex
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency(MODSAVEBACKUPS_GUID, MODSAVEBACKUPS_VERSION)]
    [BepInDependency(PASSAGEDUDE_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(RANDOMENCOUNTERS_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class SAD_Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "com.raddude.sailadex";
        public const string PLUGIN_NAME = "Sail-a-dex";
        public const string PLUGIN_VERSION = "1.7.0";

        public const string MODSAVEBACKUPS_GUID = "com.raddude.modsavebackups";
        public const string MODSAVEBACKUPS_VERSION = "1.2.0";
        public const string PASSAGEDUDE_GUID = "pr0skynesis.passagedude";
        public const string RANDOMENCOUNTERS_GUID = "com.raddude.randomencounters";
        internal static BaseUnityPlugin RE_PluginInstance {  get; private set; } 

        internal static SAD_Plugin Instance { get; private set; }
        internal static Harmony HarmonyInstance { get; private set; }
        internal static ManualLogSource _logger;

        internal static void LogDebug(string message) => _logger.LogDebug(message);
        internal static void LogInfo(string message) => _logger.LogInfo(message);
        internal static void LogWarning(string message) => _logger.LogWarning(message);
        internal static void LogError(string message) => _logger.LogError(message);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this; 
            _logger = Logger;

            Configs.InitializeConfigs();
            AssetsLoader.Start();
            HarmonyInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PLUGIN_GUID);

            foreach (var plugin in Chainloader.PluginInfos)
            {
                var metadata = plugin.Value.Metadata;
                if (metadata.GUID.Equals(PASSAGEDUDE_GUID))
                {
                    LogInfo("PassageDude mod found");
                    PassageDude.PatchMod();
                }
                if (metadata.GUID.Equals(RANDOMENCOUNTERS_GUID))
                {
                    LogInfo("RandomEncounters mod found");
                    RE_PluginInstance = plugin.Value.Instance;
                    RandomEncounters.PatchMod();
                }
            }
        }
    }
}
