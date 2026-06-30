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

            fishCaughtUIEnabled = config.Bind(
                "Enable/Disable UI",
                "Fish Caught UI",
                true);
            portsVisitedUIEnabled = config.Bind(
                "Enable/Disable UI",
                "Ports Visited UI",
                true);
            statsUIEnabled = config.Bind(
                "Enable/Disable UI",
                "Stats UI",
                true);

            fishNamesHidden = config.Bind(
                "Gameplay Settings",
                "Hide Fish Names Before Caught",
                true);
            portNamesHidden = config.Bind(
                "Gameplay Settings",
                "Hide Port Names Before Visited",
                false);
            updateMilesSailed = config.Bind(
                "Gameplay Settings",
                "Miles Sailed Updates",
                "moored",
                new ConfigDescription("Miles sailed text will be updated once moored, going to sleeping or moored, or in real time.", new AcceptableValueList<string>("moored", "sleep", "realtime")));

            notificationsEnabled = config.Bind(
                "Notification Settings",
                "Notification on badge earned",
                true);
            notificationSoundVolume = config.Bind(
                "Notification Settings",
                "Notification Volume",
                0.2f,
                "Above 1f is loud and not recommended. Set to 0f to disable.");
        }
    }
}
