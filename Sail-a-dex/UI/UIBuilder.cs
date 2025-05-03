using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace sailadex
{
    internal class UIBuilder
    {
        private static readonly ManualLogSource logger = SAD_Plugin.logger;

        internal const MissionListMode fishCaught = (MissionListMode)5;
        internal const MissionListMode portsVisited = (MissionListMode)6;
        internal const MissionListMode stats = (MissionListMode)7;

        private const int FISH_CAUGHT_FONT_SIZE = 50;
        private const int STAT_HEADER_FONT_SIZE = 45;
        private const int STAT_FONT_SIZE = 40;

        #region bookmarks

        internal static List<GameObject> MakeBookmarks(GameObject modeButtons)
        {
            var bookmarks = new List<GameObject>();
            var bookmarkReceipts = modeButtons.transform.GetChild(9).gameObject;

            var bookmarkFishCaught = GameObject.Instantiate(bookmarkReceipts);
            bookmarkFishCaught.transform.parent = modeButtons.transform;
            bookmarkFishCaught.transform.localRotation = bookmarkReceipts.transform.localRotation;
            bookmarkFishCaught.transform.localScale = bookmarkReceipts.transform.localScale;
            bookmarkFishCaught.name = "bookmark fish caught";
            bookmarkFishCaught.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "fish caught";
            var fcGPButtonLogMode = bookmarkFishCaught.GetComponent<GPButtonLogMode>();
            Traverse.Create(fcGPButtonLogMode).Field("mode").SetValue(fishCaught);
            Object.Destroy(bookmarkFishCaught.GetComponent<cakeslice.Outline>());
            bookmarks.Add(bookmarkFishCaught);

            var bookmarkPortsVisited = GameObject.Instantiate(bookmarkReceipts);
            bookmarkPortsVisited.transform.parent = modeButtons.transform;
            bookmarkPortsVisited.transform.localRotation = bookmarkReceipts.transform.localRotation;
            bookmarkPortsVisited.transform.localScale = bookmarkReceipts.transform.localScale;
            bookmarkPortsVisited.name = "bookmark ports visited";
            bookmarkPortsVisited.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "ports visited";
            var pvGPButtonLogMode = bookmarkPortsVisited.GetComponent<GPButtonLogMode>();
            Traverse.Create(pvGPButtonLogMode).Field("mode").SetValue(portsVisited);
            Object.Destroy(bookmarkPortsVisited.GetComponent<cakeslice.Outline>());
            bookmarks.Add(bookmarkPortsVisited);

            var bookmarkStats = GameObject.Instantiate(bookmarkReceipts);
            bookmarkStats.transform.parent = modeButtons.transform;
            bookmarkStats.transform.localRotation = bookmarkReceipts.transform.localRotation;
            bookmarkStats.transform.localScale = bookmarkReceipts.transform.localScale;
            bookmarkStats.name = "bookmark stats";
            bookmarkStats.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "stats & transit";
            var sGPButtonLogMode = bookmarkStats.GetComponent<GPButtonLogMode>();
            Traverse.Create(sGPButtonLogMode).Field("mode").SetValue(stats);
            Object.Destroy(bookmarkStats.GetComponent<cakeslice.Outline>());
            bookmarks.Add(bookmarkStats);

            return bookmarks;
        }

        #endregion

        #region FishCaughtUI

        internal static GameObject MakeFishCaughtUI(GameObject repUI)
        {
            var fishCaughtUI = GameObject.Instantiate(repUI);
            Object.Destroy(fishCaughtUI.GetComponent<ReputationUI>());
            fishCaughtUI.transform.parent = repUI.transform.parent;
            fishCaughtUI.transform.localPosition = repUI.transform.localPosition;
            fishCaughtUI.transform.localRotation = repUI.transform.localRotation;
            fishCaughtUI.transform.localScale = repUI.transform.localScale;
            fishCaughtUI.name = "fish caught ui";
            Object.Destroy(fishCaughtUI.transform.GetChild(2).gameObject);
            Object.Destroy(fishCaughtUI.transform.GetChild(1).gameObject);
            fishCaughtUI.AddComponent<FishCaughtUI>();

            var fishCaughtTextGO = fishCaughtUI.transform.GetChild(0).gameObject;
            fishCaughtTextGO.GetComponent<TextMesh>().font = fishCaughtTextGO.transform.GetChild(1).GetComponent<TextMesh>().font;
            fishCaughtTextGO.GetComponent<MeshRenderer>().material = fishCaughtTextGO.transform.GetChild(1).GetComponent<MeshRenderer>().material;
            fishCaughtTextGO.transform.GetChild(1).gameObject.name = "caught count";
            fishCaughtTextGO.transform.GetChild(1).localPosition = new Vector3(55f, 0f, fishCaughtTextGO.transform.GetChild(1).localPosition[2]);
            fishCaughtTextGO.transform.GetChild(1).GetComponent<TextMesh>().fontSize = FISH_CAUGHT_FONT_SIZE;
            
            var caughtCountTexts = new TextMesh[FishCaughtUI.FishNames.Length + 1];
            var fishnameTexts = new TextMesh[FishCaughtUI.FishNames.Length + 1];
            var badgeGOs = new Dictionary<string, GameObject>();

            for (int i = 0; i < FishCaughtUI.FishNames.Length; i++)
            {
                var newFishCaughtTextGO = GameObject.Instantiate(fishCaughtTextGO);
                Object.Destroy(newFishCaughtTextGO.transform.GetChild(4).gameObject);
                Object.Destroy(newFishCaughtTextGO.transform.GetChild(3).gameObject);
                Object.Destroy(newFishCaughtTextGO.transform.GetChild(2).gameObject);
                Object.Destroy(newFishCaughtTextGO.transform.GetChild(0).gameObject);
                newFishCaughtTextGO.GetComponent<TextMesh>().fontSize = FISH_CAUGHT_FONT_SIZE;
                newFishCaughtTextGO.name = FishCaughtUI.FishNames[i];
                newFishCaughtTextGO.transform.parent = fishCaughtTextGO.transform.parent;
                newFishCaughtTextGO.transform.localPosition = new Vector3(0.8f, 0.24f - 0.0375f * i, fishCaughtTextGO.transform.localPosition[2]);
                newFishCaughtTextGO.transform.localRotation = fishCaughtTextGO.transform.localRotation;
                newFishCaughtTextGO.transform.localScale = fishCaughtTextGO.transform.localScale;
                fishnameTexts[i] = newFishCaughtTextGO.GetComponent<TextMesh>();
                caughtCountTexts[i] = newFishCaughtTextGO.transform.GetChild(1).GetComponent<TextMesh>();

                string[] badgeNums = { "25", "50", "100" };

                for (int j = 0; j < badgeNums.Length; j++)
                {
                    var badgeName = newFishCaughtTextGO.name + badgeNums[j];
                    var badge = CreateBadgeObject(badgeName, newFishCaughtTextGO.transform, new Vector3(12f, 12f, 1f), new Vector3(75f + j * 13f, 0f, 0f));
                    badgeGOs.Add(badgeName, badge);
                }
            }

            var totalCaughtTextGO = GameObject.Instantiate(fishCaughtTextGO);
            Object.Destroy(totalCaughtTextGO.transform.GetChild(4).gameObject);
            Object.Destroy(totalCaughtTextGO.transform.GetChild(3).gameObject);
            Object.Destroy(totalCaughtTextGO.transform.GetChild(2).gameObject);
            Object.Destroy(totalCaughtTextGO.transform.GetChild(0).gameObject);
            totalCaughtTextGO.name = "totalCaught";
            totalCaughtTextGO.transform.parent = fishCaughtTextGO.transform.parent;
            totalCaughtTextGO.transform.localPosition = new Vector3(0.8f, -0.21f, fishCaughtTextGO.transform.localPosition[2]);
            totalCaughtTextGO.transform.localRotation = fishCaughtTextGO.transform.localRotation;
            totalCaughtTextGO.transform.localScale = fishCaughtTextGO.transform.localScale;
            fishnameTexts[FishCaughtUI.FishNames.Length] = totalCaughtTextGO.GetComponent<TextMesh>();
            caughtCountTexts[FishCaughtUI.FishNames.Length] = totalCaughtTextGO.transform.GetChild(1).GetComponent<TextMesh>();

            string[] totalCaughtBadges = { "caught50", "caught250", "caught500", "caughtAll" };

            for (int j = 0; j < totalCaughtBadges.Length; j++)
            {
                var badgeName = totalCaughtBadges[j];
                var badge = CreateBadgeObject(badgeName, totalCaughtTextGO.transform, new Vector3(15f, 15f, 1f), new Vector3(15f + j * 20f, -13f, 0f));
                badgeGOs.Add(badgeName, badge);
            }

            Object.Destroy(fishCaughtTextGO);

            FishCaughtUI.Instance.SetUIElems(fishnameTexts, caughtCountTexts, badgeGOs);

            logger.LogInfo("Loaded fish caught UI");

            return fishCaughtUI;
        }

        #endregion

        #region PortsVisitedUI

        internal static GameObject MakePortsVisitedUI(GameObject repUI)
        {
            var portsVisitedUI = GameObject.Instantiate(repUI);
            Object.Destroy(portsVisitedUI.GetComponent<ReputationUI>());
            portsVisitedUI.transform.parent = repUI.transform.parent;
            portsVisitedUI.transform.localPosition = repUI.transform.localPosition;
            portsVisitedUI.transform.localRotation = repUI.transform.localRotation;
            portsVisitedUI.transform.localScale = repUI.transform.localScale;
            portsVisitedUI.name = "ports visited ui";
            portsVisitedUI.AddComponent<PortsVisitedUI>();

            portsVisitedUI.transform.GetChild(0).localPosition = new Vector3(0.75f, 0.22f, portsVisitedUI.transform.GetChild(0).localPosition[2]);
            portsVisitedUI.transform.GetChild(1).localPosition = new Vector3(0.75f, 0.01f, portsVisitedUI.transform.GetChild(1).localPosition[2]);
            portsVisitedUI.transform.GetChild(2).localPosition = new Vector3(0.02f, 0.22f, portsVisitedUI.transform.GetChild(2).localPosition[2]);

            var lagoon = GameObject.Instantiate(portsVisitedUI.transform.GetChild(1).gameObject);
            lagoon.transform.parent = portsVisitedUI.transform;
            lagoon.transform.localPosition = new Vector3(0.02f, -0.065f, portsVisitedUI.transform.GetChild(1).localPosition[2]);
            lagoon.transform.localRotation = portsVisitedUI.transform.GetChild(1).localRotation;
            lagoon.transform.localScale = portsVisitedUI.transform.GetChild(1).localScale;
            lagoon.name = "lagoon";
            lagoon.GetComponent<TextMesh>().text = "Fire Fish Lagoon";

            var portNameTMs = new TextMesh[Region.AllPorts.Count];
            var portVisitedTMs = new TextMesh[Region.AllPorts.Count];
            var badgeGOs = new Dictionary<string, GameObject>();
            int portVisitedIndex = 0;
            int[] numPorts = { 7, 6, 10, 4 };

            for (int r = 0; r < 4; r++)
            {
                var portsVisitedGO = portsVisitedUI.transform.GetChild(r);
                Object.Destroy(portsVisitedGO.GetChild(1).gameObject);
                Object.Destroy(portsVisitedGO.GetChild(2).gameObject);
                Object.Destroy(portsVisitedGO.GetChild(4).gameObject);

                var portNameTMTemplate = portsVisitedGO.GetChild(0).gameObject;
                var portVisitedTMTemplate = portsVisitedGO.GetChild(3).gameObject;
                portNameTMTemplate.name = "port1";
                portNameTMTemplate.transform.localPosition = new Vector3(5f, -8f, -0.1f);
                portVisitedTMTemplate.name = "visited port1";
                portVisitedTMTemplate.transform.localPosition = new Vector3(70f, -9f, 0f);
                portNameTMs[portVisitedIndex] = portNameTMTemplate.GetComponent<TextMesh>();
                portVisitedTMs[portVisitedIndex] = portVisitedTMTemplate.GetComponent<TextMesh>();
                portVisitedIndex++;

                for (int i = 1; i < numPorts[r]; i++)
                {
                    var portNameTM = GameObject.Instantiate(portNameTMTemplate);
                    portNameTM.transform.parent = portNameTMTemplate.transform.parent;
                    portNameTM.transform.localPosition = new Vector3(portNameTMTemplate.transform.localPosition[0], portNameTMTemplate.transform.localPosition[1] - 7.5f * i, portNameTMTemplate.transform.localPosition[2]);
                    portNameTM.transform.localRotation = portNameTMTemplate.transform.localRotation;
                    portNameTM.transform.localScale = portNameTMTemplate.transform.localScale;
                    portNameTM.name = "port" + (i + 1);

                    var portVisitedTM = GameObject.Instantiate(portVisitedTMTemplate);
                    portVisitedTM.transform.parent = portVisitedTMTemplate.transform.parent;
                    portVisitedTM.transform.localPosition = new Vector3(portVisitedTMTemplate.transform.localPosition[0], portVisitedTMTemplate.transform.localPosition[1] - 7.5f * i, portVisitedTMTemplate.transform.localPosition[2]);
                    portVisitedTM.transform.localRotation = portVisitedTMTemplate.transform.localRotation;
                    portVisitedTM.transform.localScale = portVisitedTMTemplate.transform.localScale;
                    portVisitedTM.name = "visited port" + (i + 1);

                    portNameTMs[portVisitedIndex] = portNameTM.GetComponent<TextMesh>();
                    portVisitedTMs[portVisitedIndex] = portVisitedTM.GetComponent<TextMesh>();
                    portVisitedIndex++;
                }

                var badgeName = portsVisitedGO.name + "Badge";
                var badge = CreateBadgeObject(badgeName, portsVisitedGO, new Vector3(15f, 15f, 1f), new Vector3(-8f, -2f, 0f));
                badgeGOs.Add(badgeName, badge);
            }

            var allPortsBN = "allPortsBadge";
            var allPortsBadge = CreateBadgeObject(allPortsBN, portsVisitedUI.transform, new Vector3(0.1f, 0.0675f, 1f), new Vector3(0.55f, -0.22f, -0.007f));
            allPortsBadge.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            badgeGOs.Add(allPortsBN, allPortsBadge);

            PortsVisitedUI.Instance.SetUIElems(portNameTMs, portVisitedTMs, badgeGOs);

            logger.LogInfo("Loaded ports visited UI");

            return portsVisitedUI;
        }

        #endregion

        #region StatsUI

        internal static GameObject MakeStatsUI(GameObject repUI)
        {
            var statsUI = GameObject.Instantiate(repUI);
            Object.Destroy(statsUI.GetComponent<ReputationUI>());
            statsUI.transform.parent = repUI.transform.parent;
            statsUI.transform.localPosition = repUI.transform.localPosition;
            statsUI.transform.localRotation = repUI.transform.localRotation;
            statsUI.transform.localScale = repUI.transform.localScale;
            statsUI.name = "stats ui";
            Object.Destroy(statsUI.transform.GetChild(2).gameObject);
            Object.Destroy(statsUI.transform.GetChild(1).gameObject);
            statsUI.AddComponent<StatsUI>();

            var statsTextGO = statsUI.transform.GetChild(0).gameObject;
            statsTextGO.GetComponent<TextMesh>().font = statsTextGO.transform.GetChild(1).GetComponent<TextMesh>().font;
            statsTextGO.GetComponent<TextMesh>().fontSize = STAT_FONT_SIZE;
            statsTextGO.GetComponent<MeshRenderer>().material = statsTextGO.transform.GetChild(1).GetComponent<MeshRenderer>().material;
            statsTextGO.transform.GetChild(0).gameObject.name = "current value";
            statsTextGO.transform.GetChild(0).localPosition = new Vector3(38f, 0f, 0f);
            statsTextGO.transform.GetChild(0).GetComponent<TextMesh>().font = statsTextGO.transform.GetChild(1).GetComponent<TextMesh>().font;
            statsTextGO.transform.GetChild(0).GetComponent<TextMesh>().fontSize = STAT_FONT_SIZE;
            statsTextGO.transform.GetChild(0).GetComponent<TextMesh>().anchor = TextAnchor.MiddleLeft;
            statsTextGO.transform.GetChild(0).GetComponent<MeshRenderer>().material = statsTextGO.transform.GetChild(1).GetComponent<MeshRenderer>().material;
            statsTextGO.transform.GetChild(1).gameObject.name = "record value";
            statsTextGO.transform.GetChild(1).localPosition = new Vector3(80f, 0f, 0f);
            statsTextGO.transform.GetChild(1).GetComponent<TextMesh>().fontSize = STAT_FONT_SIZE;
            statsTextGO.transform.GetChild(1).GetComponent<TextMesh>().anchor = TextAnchor.MiddleLeft;
            statsTextGO.transform.GetChild(1).GetComponent<TextMesh>().fontStyle = FontStyle.Normal;
            var statTMs = new Dictionary<string, TextMesh>();

            AddTrackedStat(statsTextGO, "CargoMass", 0.215f, statTMs);            
            AddTrackedStat(statsTextGO, "TotalMass", 0.215f - 0.025f, statTMs);
            AddTrackedStat(statsTextGO, "UnderwayTime", 0.215f - 0.025f * 2, statTMs);

            AddTrackedStat(statsTextGO, "MilesSailed", 0.09f, statTMs, true);
            var yPosList = new List<float>();
            int j = 0;
            foreach (string ltStat in StatsUI.IntStatNames)
            {
                if (ltStat == "UnderwayDay") continue;
                if (StatsUI.RandomEncounterStats.ContainsKey(ltStat))
                    yPosList.Add(0.065f - 0.025f * j);
                AddTrackedStat(statsTextGO, ltStat, 0.065f - 0.025f * j, statTMs, true);
                j++;
            }
            StatsUI.RandomEncounterStatYPos = yPosList;

            int i = 0;
            foreach (string transit in Region.TransitCodes)
            {
                AddTrackedStat(statsTextGO, transit, 0.215f - 0.025f * i, statTMs, false, true);                
                i++;
            }

            StatsUI.Instance.SetUIElems(statTMs);

            statsTextGO.name = "stats header";
            statsTextGO.GetComponent<TextMesh>().text = "Stat                           Current                         Record";
            statsTextGO.transform.localPosition = new Vector3(0.82f, 0.24f, -0.007f);
            statsTextGO.GetComponent<TextMesh>().fontSize = STAT_HEADER_FONT_SIZE;
            statsTextGO.GetComponent<TextMesh>().fontStyle = FontStyle.Bold;
            Object.Destroy(statsTextGO.transform.GetChild(4).gameObject);
            Object.Destroy(statsTextGO.transform.GetChild(3).gameObject);
            Object.Destroy(statsTextGO.transform.GetChild(2).gameObject);
            Object.Destroy(statsTextGO.transform.GetChild(1).gameObject);
            Object.Destroy(statsTextGO.transform.GetChild(0).gameObject);

            var ltStatsTextGO = GameObject.Instantiate(statsTextGO, statsTextGO.transform.parent);
            ltStatsTextGO.name = "lifetime stats header";
            ltStatsTextGO.GetComponent<TextMesh>().text = "Lifetime Stats";
            ltStatsTextGO.transform.localPosition = new Vector3(0.82f, 0.115f, -0.007f);
            Object.Destroy(ltStatsTextGO.transform.GetChild(4).gameObject);
            Object.Destroy(ltStatsTextGO.transform.GetChild(3).gameObject);
            Object.Destroy(ltStatsTextGO.transform.GetChild(2).gameObject);
            Object.Destroy(ltStatsTextGO.transform.GetChild(1).gameObject);
            Object.Destroy(ltStatsTextGO.transform.GetChild(0).gameObject);

            var transitTextGO = GameObject.Instantiate(statsTextGO, statsTextGO.transform.parent);
            transitTextGO.name = "transit times header";
            transitTextGO.GetComponent<TextMesh>().text = "Transit Times             Last                               Record";
            transitTextGO.transform.localPosition = new Vector3(0.14f, 0.24f, -0.007f);
            Object.Destroy(transitTextGO.transform.GetChild(4).gameObject);
            Object.Destroy(transitTextGO.transform.GetChild(3).gameObject);
            Object.Destroy(transitTextGO.transform.GetChild(2).gameObject);
            Object.Destroy(transitTextGO.transform.GetChild(1).gameObject);
            Object.Destroy(transitTextGO.transform.GetChild(0).gameObject);

            logger.LogInfo("Loaded stats & transit UI");

            return statsUI;
        }

        private static void AddTrackedStat(GameObject templateGO, string name, float yPos, Dictionary<string, TextMesh> statTMs, bool ltime = false, bool transit = false)
        {
            var xPos = transit ? 0.14f : 0.82f;
            var preText = transit ? "last" : "current";
            var newTextGO = GameObject.Instantiate(templateGO);
            Object.Destroy(newTextGO.transform.GetChild(4).gameObject);
            Object.Destroy(newTextGO.transform.GetChild(3).gameObject);
            Object.Destroy(newTextGO.transform.GetChild(2).gameObject);
            if (ltime)
            {
                Object.Destroy(newTextGO.transform.GetChild(1).gameObject);
                newTextGO.transform.GetChild(0).localPosition = new Vector3(41f, 0f, 0f);
            }
            newTextGO.name = name;
            newTextGO.transform.parent = templateGO.transform.parent;
            newTextGO.transform.localPosition = new Vector3(xPos, yPos, templateGO.transform.localPosition[2]);
            newTextGO.transform.localRotation = templateGO.transform.localRotation;
            newTextGO.transform.localScale = templateGO.transform.localScale;
            statTMs[name] = newTextGO.GetComponent<TextMesh>();
            statTMs[preText + name] = newTextGO.transform.GetChild(0).GetComponent<TextMesh>();
            if (!ltime)
                statTMs["record" + name] = newTextGO.transform.GetChild(1).GetComponent<TextMesh>();
            AddLine(newTextGO.transform.GetChild(1).gameObject);
        }

        private static void AddLine(GameObject templateGO)
        {
            var lineGO = GameObject.Instantiate(templateGO);
            lineGO.name = "line";
            lineGO.transform.parent = templateGO.transform.parent;
            lineGO.transform.localPosition = new Vector3(-1.2f, -1.75f, 0f);
            lineGO.transform.localRotation = templateGO.transform.localRotation;
            lineGO.transform.localScale = templateGO.transform.localScale;
            var textMesh = lineGO.GetComponent<TextMesh>();
            textMesh.color = new Color32(0x02,0x02, 0x0A, 0x85);
            textMesh.text = "--------------------------------------------------------";
        }

        #endregion

        private static GameObject CreateBadgeObject(string name, Transform parent, Vector3 scale, Vector3 position)
        {
            var badge = GameObject.CreatePrimitive(PrimitiveType.Quad);
            badge.layer = 5;
            Object.Destroy(badge.GetComponent<MeshCollider>());
            badge.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            badge.transform.SetParent(parent, false);
            badge.transform.localScale = scale;
            badge.transform.localPosition = position;
            badge.name = name;
            badge.GetComponent<MeshRenderer>().material = AssetsLoader.materials[name];
            return badge;
        }
    }
}
