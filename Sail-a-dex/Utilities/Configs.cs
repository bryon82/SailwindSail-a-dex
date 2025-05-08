using BepInEx.Configuration;

namespace sailadex
{
    internal class Configs
    {
        internal static ConfigEntry<bool> fishNamesHidden;
        internal static ConfigEntry<bool> portNamesHidden;
        internal static ConfigEntry<bool> fishCaughtUIEnabled;
        internal static ConfigEntry<bool> portsVisitedUIEnabled;
        internal static ConfigEntry<bool> statsUIEnabled;
        internal static ConfigEntry<bool> notificationsEnabled;
        internal static ConfigEntry<float> notificationSoundVolume;
        internal static ConfigEntry<string> updateMilesSailed;

        internal static void InitializeConfigs()
        {
            var config = SAD_Plugin.Instance.Config;

            fishNamesHidden = config.Bind("Settings", "Hide Fish Names Before Caught", true, "true = fish names will be hidden before being caught for the first time.");
            portNamesHidden = config.Bind("Settings", "Hide Port Names Before Visited", false, "true = port names will be hidden before visited for the first time.");
            fishCaughtUIEnabled = config.Bind("Settings", "Enable Fish Caught UI", true, "true = UI for how many fish you caught will be enabled.");
            portsVisitedUIEnabled = config.Bind("Settings", "Enable Ports Visited UI", true, "true = UI for which ports you have visited will be enabled.");
            statsUIEnabled = config.Bind("Settings", "Enable Stats UI", true, "true = UI for various stats will be enabled.");
            notificationsEnabled = config.Bind("Settings", "Enable Notifications", true, "true = notifications on badge earned will be enabled.");
            notificationSoundVolume = config.Bind("Settings", "Notification Volume", 0.2f, "Above 1f is loud and not recommended. Set to 0f to disable.");
            updateMilesSailed = config.Bind("Settings", "Miles Sailed Updates", "moored", new ConfigDescription("Miles sailed text will be updated once moored, going to sleeping or moored, or in real time.", new AcceptableValueList<string>("moored", "sleep", "realtime")));
        }
    }
}
