using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static sailadex.SAD_Plugin;

namespace sailadex
{
    internal class UIBuilder
    {
        internal const MissionListMode FISH_CAUGHT = (MissionListMode)5;
        internal const MissionListMode PORTS_VISITED = (MissionListMode)6;
        internal const MissionListMode STATS = (MissionListMode)7;

        private const int FISH_CAUGHT_FONT_SIZE = 50;
        private const int STAT_HEADER_FONT_SIZE = 45;
        private const int STAT_FONT_SIZE = 40;

        #region bookmarks

        internal static (List<GameObject>, GPButtonLogMode[]) MakeBookmarks(GameObject modeButtons, List<GPButtonLogMode> existingLogModeButtons)
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
            Traverse.Create(fcGPButtonLogMode).Field("mode").SetValue(FISH_CAUGHT);
            Object.Destroy(bookmarkFishCaught.GetComponent<cakeslice.Outline>());
            bookmarks.Add(bookmarkFishCaught);
            existingLogModeButtons.Add(fcGPButtonLogMode);

            var bookmarkPortsVisited = GameObject.Instantiate(bookmarkReceipts);
            bookmarkPortsVisited.transform.parent = modeButtons.transform;
            bookmarkPortsVisited.transform.localRotation = bookmarkReceipts.transform.localRotation;
            bookmarkPortsVisited.transform.localScale = bookmarkReceipts.transform.localScale;
            bookmarkPortsVisited.name = "bookmark ports visited";
            bookmarkPortsVisited.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "ports visited";
            var pvGPButtonLogMode = bookmarkPortsVisited.GetComponent<GPButtonLogMode>();
            Traverse.Create(pvGPButtonLogMode).Field("mode").SetValue(PORTS_VISITED);
            Object.Destroy(bookmarkPortsVisited.GetComponent<cakeslice.Outline>());
            bookmarks.Add(bookmarkPortsVisited);
            existingLogModeButtons.Add(pvGPButtonLogMode);

            var bookmarkStats = GameObject.Instantiate(bookmarkReceipts);
            bookmarkStats.transform.parent = modeButtons.transform;
            bookmarkStats.transform.localRotation = bookmarkReceipts.transform.localRotation;
            bookmarkStats.transform.localScale = bookmarkReceipts.transform.localScale;
            bookmarkStats.name = "bookmark stats";
            bookmarkStats.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "stats & transit";
            var sGPButtonLogMode = bookmarkStats.GetComponent<GPButtonLogMode>();
            Traverse.Create(sGPButtonLogMode).Field("mode").SetValue(STATS);
            Object.Destroy(bookmarkStats.GetComponent<cakeslice.Outline>());
            bookmarks.Add(bookmarkStats);
            existingLogModeButtons.Add(sGPButtonLogMode);

            return (bookmarks, existingLogModeButtons.ToArray());
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
            Object.Destroy(fishCaughtUI.transform.GetChild(7).gameObject);
            Object.Destroy(fishCaughtUI.transform.GetChild(6).gameObject);
            Object.Destroy(fishCaughtUI.transform.GetChild(5).gameObject);
            Object.Destroy(fishCaughtUI.transform.GetChild(4).gameObject);
            Object.Destroy(fishCaughtUI.transform.GetChild(3).gameObject);
            Object.Destroy(fishCaughtUI.transform.GetChild(2).gameObject);
            Object.Destroy(fishCaughtUI.transform.GetChild(0).gameObject);
            fishCaughtUI.AddComponent<FishCaughtUI>();

            var fishCaughtTextGO = fishCaughtUI.transform.GetChild(1).gameObject;
            fishCaughtTextGO.SetActive(true);
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
                    Instance.StartCoroutine(CreateBadgeObject(badgeGOs, badgeName, newFishCaughtTextGO.transform, new Vector3(12f, 12f, 1f), new Vector3(75f + j * 13f, 0f, 0f)));
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
                Instance.StartCoroutine(CreateBadgeObject(badgeGOs, badgeName, totalCaughtTextGO.transform, new Vector3(15f, 15f, 1f), new Vector3(15f + j * 20f, -13f, 0f)));
            }

            Object.Destroy(fishCaughtTextGO);

            FishCaughtUI.Instance.SetUIElems(fishnameTexts, caughtCountTexts, badgeGOs);

            LogDebug("Loaded fish caught UI");

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
            Object.Destroy(portsVisitedUI.transform.GetChild(7).gameObject);
            Object.Destroy(portsVisitedUI.transform.GetChild(6).gameObject);
            Object.Destroy(portsVisitedUI.transform.GetChild(5).gameObject);
            Object.Destroy(portsVisitedUI.transform.GetChild(4).gameObject);
            Object.Destroy(portsVisitedUI.transform.GetChild(3).gameObject);
            portsVisitedUI.AddComponent<PortsVisitedUI>();

            portsVisitedUI.transform.GetChild(0).localPosition = new Vector3(0.75f, 0.245f, portsVisitedUI.transform.GetChild(0).localPosition[2]);
            portsVisitedUI.transform.GetChild(1).localPosition = new Vector3(0.75f, -0.05f, portsVisitedUI.transform.GetChild(1).localPosition[2]);
            portsVisitedUI.transform.GetChild(2).localPosition = new Vector3(0.02f, 0.245f, portsVisitedUI.transform.GetChild(2).localPosition[2]);
            portsVisitedUI.transform.GetChild(1).gameObject.SetActive(true);
            portsVisitedUI.transform.GetChild(2).gameObject.SetActive(true);

            var lagoon = GameObject.Instantiate(portsVisitedUI.transform.GetChild(1).gameObject);
            lagoon.transform.parent = portsVisitedUI.transform;
            lagoon.transform.localPosition = new Vector3(0.02f, -0.05f, portsVisitedUI.transform.GetChild(1).localPosition[2]);
            lagoon.transform.localRotation = portsVisitedUI.transform.GetChild(1).localRotation;
            lagoon.transform.localScale = portsVisitedUI.transform.GetChild(1).localScale;
            lagoon.name = "lagoon";
            lagoon.GetComponent<TextMesh>().text = "Fire Fish Lagoon";

            var portNameTMs = new TextMesh[Region.AllPorts.Count];
            var portVisitedTMs = new TextMesh[Region.AllPorts.Count];
            var badgeGOs = new Dictionary<string, GameObject>();
            int portVisitedIndex = 0;
            int[] numPorts = 
            { 
                Region.AlAnkh.Ports.Count,
                Region.EmeraldArchipelago.Ports.Count,
                Region.Aestrin.Ports.Count,
                Region.FireFishLagoon.Ports.Count
            };

            for (int r = 0; r < numPorts.Length; r++)
            {
                var portsVisitedGO = portsVisitedUI.transform.GetChild(r);
                if (r == 3)
                    portsVisitedGO = portsVisitedUI.transform.GetChild(8);
                Object.Destroy(portsVisitedGO.GetChild(1).gameObject);
                Object.Destroy(portsVisitedGO.GetChild(2).gameObject);
                Object.Destroy(portsVisitedGO.GetChild(4).gameObject);
                if (r == 0)
                {
                    Object.Destroy(portsVisitedGO.GetChild(5).gameObject);
                    portsVisitedGO.GetChild(6).GetComponent<TextMesh>().alignment = TextAlignment.Left;
                    portsVisitedGO.GetChild(6).GetComponent<TextMesh>().anchor = TextAnchor.MiddleLeft;
                    portsVisitedGO.GetChild(6).GetComponent<TextMesh>().fontSize = 60;
                    portsVisitedGO.GetChild(6).transform.localPosition = Vector3.zero;
                }

                var portNameTMTemplate = portsVisitedGO.GetChild(0).gameObject;
                var portVisitedTMTemplate = portsVisitedGO.GetChild(3).gameObject;
                portVisitedTMTemplate.SetActive(true);
                portNameTMTemplate.name = "port1";
                portNameTMTemplate.transform.localPosition = new Vector3(5f, -8f, -0.1f);
                portNameTMTemplate.GetComponent<TextMesh>().alignment = TextAlignment.Left;
                portNameTMTemplate.GetComponent<TextMesh>().anchor = TextAnchor.MiddleLeft;
                portNameTMTemplate.GetComponent<TextMesh>().fontSize = 50;
                portVisitedTMTemplate.name = "visited port1";
                portVisitedTMTemplate.transform.localPosition = new Vector3(70f, -9f, 0f);
                portVisitedTMTemplate.GetComponent<TextMesh>().alignment = TextAlignment.Right;
                portVisitedTMTemplate.GetComponent<TextMesh>().anchor = TextAnchor.MiddleRight;
                portVisitedTMTemplate.GetComponent<TextMesh>().fontSize = 50;
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
                    portVisitedTM.name = $"visited port{(i + 1)}";
                    portNameTMs[portVisitedIndex] = portNameTM.GetComponent<TextMesh>();
                    portVisitedTMs[portVisitedIndex] = portVisitedTM.GetComponent<TextMesh>();
                    portVisitedIndex++;
                }

                var badgeName = $"{portsVisitedGO.name}Badge";
                Instance.StartCoroutine(CreateBadgeObject(badgeGOs, badgeName, portsVisitedGO, new Vector3(15f, 15f, 1f), new Vector3(-8f, -2f, 0f)));
            }

            Instance.StartCoroutine(CreateBadgeObject(badgeGOs, "allPortsBadge", portsVisitedUI.transform, new Vector3(0.1f, 0.0675f, 1f), new Vector3(-0.15f, -0.23f, -0.007f)));            

            PortsVisitedUI.Instance.SetUIElems(portNameTMs, portVisitedTMs, badgeGOs);

            LogDebug("Loaded ports visited UI");

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
            Object.Destroy(statsUI.transform.GetChild(7).gameObject);
            Object.Destroy(statsUI.transform.GetChild(6).gameObject);
            Object.Destroy(statsUI.transform.GetChild(5).gameObject);
            Object.Destroy(statsUI.transform.GetChild(4).gameObject);
            Object.Destroy(statsUI.transform.GetChild(3).gameObject);
            Object.Destroy(statsUI.transform.GetChild(2).gameObject);
            Object.Destroy(statsUI.transform.GetChild(0).gameObject);
            statsUI.AddComponent<StatsUI>();

            var statsTextGO = statsUI.transform.GetChild(1).gameObject;
            statsTextGO.SetActive(true);
            statsTextGO.GetComponent<TextMesh>().font = statsTextGO.transform.GetChild(1).GetComponent<TextMesh>().font;
            statsTextGO.GetComponent<TextMesh>().fontSize = STAT_FONT_SIZE;
            statsTextGO.GetComponent<TextMesh>().characterSize = 1f;
            statsTextGO.GetComponent<MeshRenderer>().material = statsTextGO.transform.GetChild(1).GetComponent<MeshRenderer>().material;
            statsTextGO.transform.GetChild(0).gameObject.name = "current value";
            statsTextGO.transform.GetChild(0).localPosition = new Vector3(38f, 0f, 0f);
            statsTextGO.transform.GetChild(0).GetComponent<TextMesh>().font = statsTextGO.transform.GetChild(1).GetComponent<TextMesh>().font;
            statsTextGO.transform.GetChild(0).GetComponent<TextMesh>().fontSize = STAT_FONT_SIZE;
            statsTextGO.transform.GetChild(0).GetComponent<TextMesh>().anchor = TextAnchor.MiddleLeft;
            statsTextGO.transform.GetChild(0).GetComponent<TextMesh>().fontStyle = FontStyle.Normal;
            statsTextGO.transform.GetChild(0).GetComponent<TextMesh>().characterSize = 1f;
            statsTextGO.transform.GetChild(0).GetComponent<TextMesh>().lineSpacing = 1f;
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

            LogDebug("Loaded stats & transit UI");

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

        private static IEnumerator CreateBadgeObject(Dictionary<string, GameObject> badgeGOs, string name, Transform parent, Vector3 scale, Vector3 position)
        {
            yield return new WaitUntil(() => AssetsLoader.BadgesLoaded);
            var badge = GameObject.CreatePrimitive(PrimitiveType.Quad);
            badge.layer = 5;
            badge.transform.SetParent(parent, false);
            badge.transform.localScale = scale;
            badge.transform.localPosition = position;
            badge.name = name;
            if (name.Equals("allPortsBadge"))            
                badge.transform.localEulerAngles = new Vector3(0f, 180f, 0f);            
            var meshRenderer = badge.GetComponent<MeshRenderer>();
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.material = AssetsLoader.Materials[name];
            Object.Destroy(badge.GetComponent<MeshCollider>());
            badgeGOs.Add(name, badge);
        }
    }
}
