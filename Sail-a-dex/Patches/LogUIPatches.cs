using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static sailadex.Configs;

namespace sailadex
{
    internal class LogUIPatches
    {
        private static GameObject fishCaughtUI;
        private static GameObject portsVisitedUI;
        private static GameObject statsUI;
        private static List<GameObject> bookmarks;
        private static GPButtonLogMode[] logModeButtons;

        [HarmonyPatch(typeof(MissionListUI))]
        private class MissionListUIPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("ToggleMenu")]
            public static void TogglePatch()
            {
                bookmarks[0].SetActive(false);
                bookmarks[1].SetActive(false);
                bookmarks[2].SetActive(false);

                var inactivePos = new Stack<Vector3>();
                inactivePos.Push(new Vector3(-0.515f, 0.0032f, -0.4566f));
                inactivePos.Push(new Vector3(-0.387f, 0.0028f, -0.4566f));
                inactivePos.Push(new Vector3(-0.26f, -0.0027f, -0.4566f));

                var activePos = new Stack<Vector3>();
                activePos.Push(new Vector3(-0.515f, 0.0082f, -0.4516f));
                activePos.Push(new Vector3(-0.387f, 0.0138f, -0.4516f));
                activePos.Push(new Vector3(-0.26f, 0.0137f, -0.4516f));

                if (fishCaughtUIEnabled.Value)
                {
                    logModeButtons[5].SetPrivateField("inactiveLocalPos", inactivePos.Pop());
                    logModeButtons[5].SetPrivateField("activeLocalPos", activePos.Pop());                    
                    bookmarks[0].SetActive(true);
                }
                if (portsVisitedUIEnabled.Value)
                {
                    logModeButtons[6].SetPrivateField("inactiveLocalPos", inactivePos.Pop());
                    logModeButtons[6].SetPrivateField("activeLocalPos", activePos.Pop());                    
                    bookmarks[1].SetActive(true);
                }
                if (statsUIEnabled.Value)
                {
                    logModeButtons[7].SetPrivateField("inactiveLocalPos", inactivePos.Pop());
                    logModeButtons[7].SetPrivateField("activeLocalPos", activePos.Pop());
                    bookmarks[2].SetActive(true);
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("HideUI")]
            public static void HideUIPatches()
            {
                fishCaughtUI.SetActive(false);
                portsVisitedUI.SetActive(false);
                statsUI.SetActive(false);
            }

            [HarmonyPostfix]
            [HarmonyPatch("SwitchMode")]
            public static void SwitchModePatches(MissionListMode mode)
            {
                fishCaughtUI.SetActive(false);
                portsVisitedUI.SetActive(false);
                statsUI.SetActive(false);
                switch (mode)
                {
                    case UIBuilder.FISH_CAUGHT:
                        fishCaughtUI.SetActive(true);
                        FishCaughtUI.Instance.UpdatePage();
                        break;
                    case UIBuilder.PORTS_VISITED:
                        portsVisitedUI.SetActive(true);
                        PortsVisitedUI.Instance.UpdatePage();
                        break;
                    case UIBuilder.STATS:
                        statsUI.SetActive(true);
                        StatsUI.Instance.UpdatePage();
                        break;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch("Start")]
            public static void StartPatch(GameObject ___modeButtons, ref GPButtonLogMode[] ___logModeButtons, GameObject ___reputationUI)
            {
                var newLogModeButtons = new List<GPButtonLogMode>(___logModeButtons);
                (bookmarks, logModeButtons) = UIBuilder.MakeBookmarks(___modeButtons, newLogModeButtons);
                statsUI = UIBuilder.MakeStatsUI(___reputationUI);
                portsVisitedUI = UIBuilder.MakePortsVisitedUI(___reputationUI);
                fishCaughtUI = UIBuilder.MakeFishCaughtUI(___reputationUI);
                ___logModeButtons = logModeButtons;
            }
        }

        [HarmonyPatch(typeof(NotificationUi))]
        private class NotificationUiPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Start")]
            public static void StartPatch(NotificationUi ___instance)
            {
                if (notificationsEnabled.Value)
                    ___instance.gameObject.AddComponent<NotificationUiQueue>();
            }
        }
    }
}
