using HarmonyLib;

namespace sailadex.Patches
{
    internal class ModDataPatches
    {
        [HarmonyPatch(typeof(SaveLoadManager))]
        private class SaveLoadManagerPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("SaveModData")]
            public static void SaveModData()
            {
                FishCaughtUI.Instance.SaveFishCaughtUI();
                PortsVisitedUI.Instance.SavePortsVisitedUI();
                StatsUI.Instance.SaveStatsUI();
            }

            [HarmonyPrefix]
            [HarmonyPatch("LoadModData")]
            public static void LoadModData()
            {
                FishCaughtUI.Instance.LoadFishCaughtUI();
                PortsVisitedUI.Instance.LoadPortsVisitedUI();
                StatsUI.Instance.LoadStatsUI();
            }
        }
    }
}
