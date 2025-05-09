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

                var bookmarkPos = new Stack<float>();
                bookmarkPos.Push(-0.515f);
                bookmarkPos.Push(-0.387f);
                bookmarkPos.Push(-0.26f);

                if (fishCaughtUIEnabled.Value)
                {
                    bookmarks[0].transform.localPosition = new Vector3(bookmarkPos.Pop(), 0.0027f, -0.4566f);
                    bookmarks[0].SetActive(true);
                }
                if (portsVisitedUIEnabled.Value)
                {
                    bookmarks[1].transform.localPosition = new Vector3(bookmarkPos.Pop(), 0.0028f, -0.4566f);
                    bookmarks[1].SetActive(true);
                }
                if (statsUIEnabled.Value)
                {
                    bookmarks[2].transform.localPosition = new Vector3(bookmarkPos.Pop(), -0.0032f, -0.4566f);
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
                    case UIBuilder.fishCaught:
                        fishCaughtUI.SetActive(true);
                        FishCaughtUI.Instance.UpdatePage();
                        break;
                    case UIBuilder.portsVisited:
                        portsVisitedUI.SetActive(true);
                        PortsVisitedUI.Instance.UpdatePage();
                        break;
                    case UIBuilder.stats:
                        statsUI.SetActive(true);
                        StatsUI.Instance.UpdatePage();
                        break;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch("Start")]
            public static void StartPatch(GameObject ___modeButtons, GameObject ___reputationUI)
            {
                bookmarks = UIBuilder.MakeBookmarks(___modeButtons);
                statsUI = UIBuilder.MakeStatsUI(___reputationUI);
                portsVisitedUI = UIBuilder.MakePortsVisitedUI(___reputationUI);
                fishCaughtUI = UIBuilder.MakeFishCaughtUI(___reputationUI);
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
