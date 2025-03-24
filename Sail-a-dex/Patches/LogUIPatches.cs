using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace sailadex
{
    internal class LogUIPatches
    {
        private static GameObject fishCaughtUI;
        private static GameObject portsVisitedUI;
        private static GameObject statsUI;
        private const MissionListMode fishCaught = (MissionListMode)5;
        private const MissionListMode portsVisited = (MissionListMode)6;
        private const MissionListMode stats = (MissionListMode)7;
        private static Stack<float> bookmarkPos;

        [HarmonyPatch(typeof(MissionListUI))]
        private class MissionListUIPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("HideUI")]
            public static void HideUIPatches(MissionListUI __instance)
            {
                if (Plugin.fishCaughtUIEnabled.Value)
                    fishCaughtUI.SetActive(value: false);
                if (Plugin.portsVisitedUIEnabled.Value)
                    portsVisitedUI.SetActive(value: false);
                if (Plugin.statsUIEnabled.Value)
                    statsUI.SetActive(value: false);
            }

            [HarmonyPostfix]
            [HarmonyPatch("SwitchMode")]
            public static void SwitchModePatches(MissionListMode mode)
            {
                if (Plugin.fishCaughtUIEnabled.Value)
                    fishCaughtUI.SetActive(value: false);
                if (Plugin.portsVisitedUIEnabled.Value)
                    portsVisitedUI.SetActive(value: false);
                if (Plugin.statsUIEnabled.Value)
                    statsUI.SetActive(value: false);
                switch (mode)
                {
                    case fishCaught:
                        fishCaughtUI.SetActive(value: true);
                        FishCaughtUI.instance.UpdatePage();
                        break;
                    case portsVisited:
                        portsVisitedUI.SetActive(value: true);
                        PortsVisitedUI.instance.UpdatePage();
                        break;
                    case stats:
                        statsUI.SetActive(value: true);
                        StatsUI.instance.UpdatePage();
                        break;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch("Start")]
            public static void StartPatch(MissionListUI __instance, GameObject ___modeButtons, GameObject ___reputationUI)
            {
                AssetsLoader.Start();

                bookmarkPos = new Stack<float>();
                bookmarkPos.Push(-0.515f);
                bookmarkPos.Push(-0.387f);
                bookmarkPos.Push(-0.26f);

                if (Plugin.notificationSoundVolume.Value > 0f)
                    AssetsLoader.LoadAudio();

                if (Plugin.fishCaughtUIEnabled.Value)
                {
                    AssetsLoader.LoadFishBadges();
                    MakeFishCaughtUI(___modeButtons, ___reputationUI);
                }

                if (Plugin.portsVisitedUIEnabled.Value)
                {
                    AssetsLoader.LoadPortBadges();
                    MakePortsVisitedUI(___modeButtons, ___reputationUI);
                }

                if (Plugin.statsUIEnabled.Value)
                    MakeStatsUI(___modeButtons, ___reputationUI);
            }

            private static void MakeFishCaughtUI(GameObject modeButtons, GameObject repUI)
            {
                GameObject bookmarkReceipts = modeButtons.transform.GetChild(9).gameObject;
                GameObject bookmarkFishCaught = GameObject.Instantiate(bookmarkReceipts);
                bookmarkFishCaught.transform.parent = modeButtons.transform;
                bookmarkFishCaught.transform.localPosition = new Vector3(bookmarkPos.Pop(), bookmarkReceipts.transform.localPosition[1], bookmarkReceipts.transform.localPosition[2]);
                bookmarkFishCaught.transform.localRotation = bookmarkReceipts.transform.localRotation;
                bookmarkFishCaught.transform.localScale = bookmarkReceipts.transform.localScale;
                bookmarkFishCaught.name = "bookmark fish caught";
                bookmarkFishCaught.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "fish caught";
                GPButtonLogMode gPButtonLogMode = bookmarkFishCaught.GetComponent<GPButtonLogMode>();
                Traverse.Create(gPButtonLogMode).Field("mode").SetValue(fishCaught);
                Object.Destroy(bookmarkFishCaught.GetComponent<cakeslice.Outline>());

                fishCaughtUI = GameObject.Instantiate(repUI);
                Object.Destroy(fishCaughtUI.GetComponent<ReputationUI>());
                fishCaughtUI.transform.parent = repUI.transform.parent;
                fishCaughtUI.transform.localPosition = repUI.transform.localPosition;
                fishCaughtUI.transform.localRotation = repUI.transform.localRotation;
                fishCaughtUI.transform.localScale = repUI.transform.localScale;
                fishCaughtUI.name = "fish caught ui";
                Object.Destroy(fishCaughtUI.transform.GetChild(2).gameObject);
                Object.Destroy(fishCaughtUI.transform.GetChild(1).gameObject);
                fishCaughtUI.AddComponent<FishCaughtUI>();

                GameObject fishCaughtTextGO = fishCaughtUI.transform.GetChild(0).gameObject;
                fishCaughtTextGO.GetComponent<TextMesh>().font = fishCaughtTextGO.transform.GetChild(1).GetComponent<TextMesh>().font;
                fishCaughtTextGO.GetComponent<MeshRenderer>().material = fishCaughtTextGO.transform.GetChild(1).GetComponent<MeshRenderer>().material;
                fishCaughtTextGO.transform.GetChild(1).gameObject.name = "caught count";
                fishCaughtTextGO.transform.GetChild(1).localPosition = new Vector3(55f, 0f, fishCaughtTextGO.transform.GetChild(1).localPosition[2]);
                fishCaughtTextGO.transform.GetChild(1).GetComponent<TextMesh>().fontSize = 50;
                TextMesh[] caughtCountTexts = new TextMesh[Names.fishNames.Length + 1];
                TextMesh[] fishnameTexts = new TextMesh[Names.fishNames.Length + 1];

                for (int i = 0; i < Names.fishNames.Length; i++)
                {
                    GameObject newFishCaughtTextGO = GameObject.Instantiate(fishCaughtTextGO);
                    Object.Destroy(newFishCaughtTextGO.transform.GetChild(4).gameObject);
                    Object.Destroy(newFishCaughtTextGO.transform.GetChild(3).gameObject);
                    Object.Destroy(newFishCaughtTextGO.transform.GetChild(2).gameObject);
                    Object.Destroy(newFishCaughtTextGO.transform.GetChild(0).gameObject);
                    newFishCaughtTextGO.GetComponent<TextMesh>().fontSize = 50;
                    newFishCaughtTextGO.name = Names.fishNames[i];
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
                        FishCaughtUI.instance.fishBadgeGOs.Add(badgeName, badge);
                    }
                }

                GameObject totalCaughtTextGO = GameObject.Instantiate(fishCaughtTextGO);
                Object.Destroy(totalCaughtTextGO.transform.GetChild(4).gameObject);
                Object.Destroy(totalCaughtTextGO.transform.GetChild(3).gameObject);
                Object.Destroy(totalCaughtTextGO.transform.GetChild(2).gameObject);
                Object.Destroy(totalCaughtTextGO.transform.GetChild(0).gameObject);
                totalCaughtTextGO.name = "totalCaught";
                totalCaughtTextGO.transform.parent = fishCaughtTextGO.transform.parent;
                totalCaughtTextGO.transform.localPosition = new Vector3(0.8f, -0.21f, fishCaughtTextGO.transform.localPosition[2]);
                totalCaughtTextGO.transform.localRotation = fishCaughtTextGO.transform.localRotation;
                totalCaughtTextGO.transform.localScale = fishCaughtTextGO.transform.localScale;
                fishnameTexts[Names.fishNames.Length] = totalCaughtTextGO.GetComponent<TextMesh>();
                caughtCountTexts[Names.fishNames.Length] = totalCaughtTextGO.transform.GetChild(1).GetComponent<TextMesh>();

                string[] totalCaughtBadges = { "caught50", "caught250", "caught500", "caughtAll" };

                for (int j = 0; j < totalCaughtBadges.Length; j++)
                {
                    var badgeName = totalCaughtBadges[j];
                    var badge = CreateBadgeObject(badgeName, totalCaughtTextGO.transform, new Vector3(15f, 15f, 1f), new Vector3(15f + j * 20f, -13f, 0f));
                    FishCaughtUI.instance.fishBadgeGOs.Add(badgeName, badge);
                }

                Object.Destroy(fishCaughtTextGO);
                FishCaughtUI.instance.fishNameTMs = fishnameTexts;
                FishCaughtUI.instance.caughtCountTMs = caughtCountTexts;

                Plugin.logger.LogInfo("Loaded fish caught UI");
            }

            private static void MakePortsVisitedUI(GameObject modeButtons, GameObject repUI)
            {
                GameObject bookmarkReceipts = modeButtons.transform.GetChild(9).gameObject;
                GameObject bookmarkPortsVisited = GameObject.Instantiate(bookmarkReceipts);
                bookmarkPortsVisited.transform.parent = modeButtons.transform;
                bookmarkPortsVisited.transform.localPosition = new Vector3(bookmarkPos.Pop(), 0.0028f, bookmarkReceipts.transform.localPosition[2]);
                bookmarkPortsVisited.transform.localRotation = bookmarkReceipts.transform.localRotation;
                bookmarkPortsVisited.transform.localScale = bookmarkReceipts.transform.localScale;
                bookmarkPortsVisited.name = "bookmark ports visited";
                bookmarkPortsVisited.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "ports visited";
                GPButtonLogMode gPButtonLogMode = bookmarkPortsVisited.GetComponent<GPButtonLogMode>();
                Traverse.Create(gPButtonLogMode).Field("mode").SetValue(portsVisited);
                Object.Destroy(bookmarkPortsVisited.GetComponent<cakeslice.Outline>());

                portsVisitedUI = GameObject.Instantiate(repUI);
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

                GameObject lagoon = GameObject.Instantiate(portsVisitedUI.transform.GetChild(1).gameObject);
                lagoon.transform.parent = portsVisitedUI.transform;
                lagoon.transform.localPosition = new Vector3(0.02f, -0.065f, portsVisitedUI.transform.GetChild(1).localPosition[2]);
                lagoon.transform.localRotation = portsVisitedUI.transform.GetChild(1).localRotation;
                lagoon.transform.localScale = portsVisitedUI.transform.GetChild(1).localScale;
                lagoon.name = "lagoon";
                lagoon.GetComponent<TextMesh>().text = "Fire Fish Lagoon";

                TextMesh[] portNameTMs = new TextMesh[Names.portNames.Length];
                TextMesh[] portVisitedTMs = new TextMesh[Names.portNames.Length];
                int portVisitedIndex = 0;
                int[] numPorts = { 7, 6, 10, 4 };

                for (int r = 0; r < 4; r++)
                {
                    Transform portsVisitedGO = portsVisitedUI.transform.GetChild(r);
                    Object.Destroy(portsVisitedGO.GetChild(1).gameObject);
                    Object.Destroy(portsVisitedGO.GetChild(2).gameObject);
                    Object.Destroy(portsVisitedGO.GetChild(4).gameObject);

                    GameObject portNameTMTemplate = portsVisitedGO.GetChild(0).gameObject;
                    GameObject portVisitedTMTemplate = portsVisitedGO.GetChild(3).gameObject;
                    portNameTMTemplate.name = "port1";
                    portNameTMTemplate.transform.localPosition = new Vector3(5f, -8f, -0.1f);
                    portVisitedTMTemplate.name = "visited port1";
                    portVisitedTMTemplate.transform.localPosition = new Vector3(70f, -9f, 0f);
                    portNameTMs[portVisitedIndex] = portNameTMTemplate.GetComponent<TextMesh>();
                    portVisitedTMs[portVisitedIndex] = portVisitedTMTemplate.GetComponent<TextMesh>();
                    portVisitedIndex++;

                    for (int i = 1; i < numPorts[r]; i++)
                    {
                        GameObject portNameTM = GameObject.Instantiate(portNameTMTemplate);
                        portNameTM.transform.parent = portNameTMTemplate.transform.parent;
                        portNameTM.transform.localPosition = new Vector3(portNameTMTemplate.transform.localPosition[0], portNameTMTemplate.transform.localPosition[1] - 7.5f * i, portNameTMTemplate.transform.localPosition[2]);
                        portNameTM.transform.localRotation = portNameTMTemplate.transform.localRotation;
                        portNameTM.transform.localScale = portNameTMTemplate.transform.localScale;
                        portNameTM.name = "port" + (i + 1);

                        GameObject portVisitedTM = GameObject.Instantiate(portVisitedTMTemplate);
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
                    PortsVisitedUI.instance.portBadgeGOs.Add(badgeName, badge);
                }

                var allPortsBN = "allPortsBadge";
                var allPortsBadge = CreateBadgeObject(allPortsBN, portsVisitedUI.transform, new Vector3(0.1f, 0.0675f, 1f), new Vector3(0.55f, -0.22f, -0.007f));
                allPortsBadge.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                PortsVisitedUI.instance.portBadgeGOs.Add(allPortsBN, allPortsBadge);

                PortsVisitedUI.instance.portNameTMs = portNameTMs;
                PortsVisitedUI.instance.portVisitedTMs = portVisitedTMs;

                Plugin.logger.LogInfo("Loaded ports visited UI");
            }

            private static void MakeStatsUI(GameObject modeButtons, GameObject repUI)
            {
                GameObject bookmarkReceipts = modeButtons.transform.GetChild(9).gameObject;
                GameObject bookmark = GameObject.Instantiate(bookmarkReceipts);
                bookmark.transform.parent = modeButtons.transform;
                bookmark.transform.localPosition = new Vector3(bookmarkPos.Pop(), -0.0032f, -0.4566f);
                bookmark.transform.localRotation = bookmarkReceipts.transform.localRotation;
                bookmark.transform.localScale = bookmarkReceipts.transform.localScale;
                bookmark.name = "bookmark stats";
                bookmark.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text = "stats & transit";
                GPButtonLogMode gPButtonLogMode = bookmark.GetComponent<GPButtonLogMode>();
                Traverse.Create(gPButtonLogMode).Field("mode").SetValue(stats);
                Object.Destroy(bookmark.GetComponent<cakeslice.Outline>());

                statsUI = GameObject.Instantiate(repUI);
                Object.Destroy(statsUI.GetComponent<ReputationUI>());
                statsUI.transform.parent = repUI.transform.parent;
                statsUI.transform.localPosition = repUI.transform.localPosition;
                statsUI.transform.localRotation = repUI.transform.localRotation;
                statsUI.transform.localScale = repUI.transform.localScale;
                statsUI.name = "stats ui";
                Object.Destroy(statsUI.transform.GetChild(2).gameObject);
                Object.Destroy(statsUI.transform.GetChild(1).gameObject);
                statsUI.AddComponent<StatsUI>();

                GameObject statsTextGO = statsUI.transform.GetChild(0).gameObject;
                statsTextGO.GetComponent<TextMesh>().font = statsTextGO.transform.GetChild(1).GetComponent<TextMesh>().font;
                statsTextGO.GetComponent<TextMesh>().fontSize = 45;
                statsTextGO.GetComponent<MeshRenderer>().material = statsTextGO.transform.GetChild(1).GetComponent<MeshRenderer>().material;
                statsTextGO.transform.GetChild(0).gameObject.name = "current value";
                statsTextGO.transform.GetChild(0).localPosition = new Vector3(38f, 0f, 0f);
                statsTextGO.transform.GetChild(0).GetComponent<TextMesh>().font = statsTextGO.transform.GetChild(1).GetComponent<TextMesh>().font;
                statsTextGO.transform.GetChild(0).GetComponent<TextMesh>().fontSize = 45;                
                statsTextGO.transform.GetChild(0).GetComponent<TextMesh>().anchor = TextAnchor.MiddleLeft;
                statsTextGO.transform.GetChild(0).GetComponent<MeshRenderer>().material = statsTextGO.transform.GetChild(1).GetComponent<MeshRenderer>().material;
                statsTextGO.transform.GetChild(1).gameObject.name = "record value";
                statsTextGO.transform.GetChild(1).localPosition = new Vector3(80f, 0f, 0f);
                statsTextGO.transform.GetChild(1).GetComponent<TextMesh>().fontSize = 45;
                statsTextGO.transform.GetChild(1).GetComponent<TextMesh>().anchor = TextAnchor.MiddleLeft;
                statsTextGO.transform.GetChild(1).GetComponent<TextMesh>().fontStyle = FontStyle.Normal;
                Dictionary<string, TextMesh> statTMs = new Dictionary<string, TextMesh>();

                AddTrackedStat(statsTextGO, "CargoMass", 0.215f, statTMs);
                AddTrackedStat(statsTextGO, "TotalMass", 0.215f - 0.03f, statTMs);
                AddTrackedStat(statsTextGO, "UnderwayTime", 0.215f - 0.03f * 2, statTMs);

                AddTrackedStat(statsTextGO, "MilesSailed", 0.08f, statTMs, true);
                int j = 0;
                foreach (string ltStat in Names.intStatNames)
                {
                    if (ltStat == "UnderwayDay") continue;
                    if (ltStat == "FlotsamEncounters" && (RandomEncounters.pluginInstance == null || !RandomEncounters.flotsamEnabled)) continue;
                    if (ltStat == "DenseFogEncounters" && (RandomEncounters.pluginInstance == null || !RandomEncounters.denseFogEnabled)) continue;
                    if (ltStat == "SeaLifeEncounters" && (RandomEncounters.pluginInstance == null || !RandomEncounters.isSeaLifeEnabled)) continue;
                    AddTrackedStat(statsTextGO, ltStat, 0.05f - 0.03f * j, statTMs, true);
                    j++;
                }

                int i = 0;
                foreach (string transit in Names.regionTransitNames)
                {
                    AddTrackedStat(statsTextGO, transit, 0.215f - 0.03f * i, statTMs, false, true);
                    i++;
                }

                StatsUI.instance.statTMs = statTMs;

                statsTextGO.name = "stats header";
                statsTextGO.GetComponent<TextMesh>().text = "Stat                        Current                     Record";
                statsTextGO.transform.localPosition = new Vector3(0.82f, 0.24f, -0.007f);
                statsTextGO.GetComponent<TextMesh>().fontSize = 50;
                statsTextGO.GetComponent<TextMesh>().fontStyle = FontStyle.Bold;
                Object.Destroy(statsTextGO.transform.GetChild(4).gameObject);
                Object.Destroy(statsTextGO.transform.GetChild(3).gameObject);
                Object.Destroy(statsTextGO.transform.GetChild(2).gameObject);
                Object.Destroy(statsTextGO.transform.GetChild(1).gameObject);
                Object.Destroy(statsTextGO.transform.GetChild(0).gameObject);

                GameObject ltStatsTextGO = GameObject.Instantiate(statsTextGO, statsTextGO.transform.parent);
                ltStatsTextGO.name = "lifetime stats header";
                ltStatsTextGO.GetComponent<TextMesh>().text = "Lifetime Stats";
                ltStatsTextGO.transform.localPosition = new Vector3(0.82f, 0.105f, -0.007f);
                Object.Destroy(ltStatsTextGO.transform.GetChild(4).gameObject);
                Object.Destroy(ltStatsTextGO.transform.GetChild(3).gameObject);
                Object.Destroy(ltStatsTextGO.transform.GetChild(2).gameObject);
                Object.Destroy(ltStatsTextGO.transform.GetChild(1).gameObject);
                Object.Destroy(ltStatsTextGO.transform.GetChild(0).gameObject);

                GameObject transitTextGO = GameObject.Instantiate(statsTextGO, statsTextGO.transform.parent);
                transitTextGO.name = "transit times header";
                transitTextGO.GetComponent<TextMesh>().text = "Transit Times          Last                           Record";
                transitTextGO.transform.localPosition = new Vector3(0.12f, 0.24f, -0.007f);
                Object.Destroy(transitTextGO.transform.GetChild(4).gameObject);
                Object.Destroy(transitTextGO.transform.GetChild(3).gameObject);
                Object.Destroy(transitTextGO.transform.GetChild(2).gameObject);
                Object.Destroy(transitTextGO.transform.GetChild(1).gameObject);
                Object.Destroy(transitTextGO.transform.GetChild(0).gameObject);
                
                Plugin.logger.LogInfo("Loaded stats & transit UI");
            }

            private static void AddTrackedStat(GameObject templateGO, string name, float yPos, Dictionary<string, TextMesh> statTMs, bool ltime = false, bool transit = false)
            {
                var xPos = transit ? 0.12f : 0.82f;
                var preText = transit ? "last" : "current";
                GameObject newTextGO = GameObject.Instantiate(templateGO);
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
            }

            private static GameObject CreateBadgeObject(string name, Transform parent, Vector3 scale, Vector3 position)
            {
                GameObject badge = GameObject.CreatePrimitive(PrimitiveType.Quad);
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

        [HarmonyPatch(typeof(NotificationUi))]
        private class NotificationUiPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Start")]
            public static void StartPatch(NotificationUi ___instance)
            {
                if (Plugin.notificationsEnabled.Value)
                    ___instance.gameObject.AddComponent<NotificationUiQueue>();
            }
        }

        //// Cheats for testing
        //[HarmonyPatch(typeof(OceanFishes))]
        //private class OceanFishesPatches
        //{
        //    [HarmonyPostfix]
        //    [HarmonyPatch("Update")]
        //    public static void UpdatePatch(OceanFishes __instance)
        //    {
        //        if (Input.GetKeyDown("p"))
        //        {

                    //        //    //StatsUI.instance.RegisterMoored("island 9 E (dragon cliffs)");
                    //        //    Debug.Log("Distance to land: " + GameState.distanceToLand);
                    //        //    Debug.Log("DebugFishCatch: " + __instance.GetFish(Utilities.PlayerTransform).name);
                    //for (int i = 0; i < 5; i++)
                    //    FishCaughtUI.instance.RegisterCatch(__instance.GetFish(Camera.main.transform));

                    //        //    string[] ports = { "Gold Rock City",
                    //        //                            "Al'Nilem",
                    //        //                            //"Neverdin",
                    //        //                            "Albacore Town",
                    //        //                            "Alchemist's Island",
                    //        //                            "Al'Ankh Academy",
                    //        //                            "Oasis"
                    //        //                            };
                    //        //    foreach (var port in ports)
                    //        //    {
                    //        //        Debug.Log("Debug visit: " + port);
                    //        //        PortsVisitedUI.instance.RegisterVisit(port);
                    //        //    }
                    //        //}
                    //        //if (Input.GetKeyDown("l"))
                    //        //{
                    //        //    StatsUI.instance.RegisterMoored("island 15 M (Fort)");
                    //        //}
                    //        if (Input.GetKeyDown("u"))
                    //        {
                    //            Plugin.logger.LogInfo($"CurrentBoat: {GameState.currentBoat} CurrentBoatPosition: {GameState.currentBoat?.position}");
                    //            //Plugin.logger.LogInfo($"LastBoat: {GameState.lastBoat} CurrentBoat: {GameState.currentBoat?.parent}");
                    //            //StatsUI.instance.RegisterMoored("island 27 Lagoon Shipyard");
                    //        }
                    //        //if (Input.GetKeyDown("i"))
                    //        //{
                    //        //    //StatsUI.instance.RegisterUnderway("island 1 A (gold rock)");

                    //        //    Debug.Log($"Current Boat: {GameState.currentBoat} Last Boat: {GameState.lastBoat}");
                    //        //    string[] portNames = {
                    //        //        //Emerald Archipelago
                    //        //        "Dragon Cliffs",
                    //        //        "Sanctuary",
                    //        //        "Crab Beach",
                    //        //        "New Port",
                    //        //        "Sage Hills",
                    //        //        "Serpent Isle",
                    //        //        //Aestrin(medi)
                    //        //        "Fort Aestrin",
                    //        //        "Sunspire",
                    //        //        "Mount Malefic",
                    //        //        "Siren Song",
                    //        //        "Eastwind",
                    //        //        "Happy Bay",
                    //        //        "Chronos",
                    //        //        //Fire Fish Lagoon
                    //        //        "Kicia Bay",
                    //        //        "Fire Fish Town",
                    //        //        "On'na",
                    //        //        "Sen'na"
                    //        //    };

                    //        //    foreach (var port in portNames)
                    //        //    {
                    //        //        Debug.Log("Debug visit: " + port);
                    //        //        PortsVisitedUI.instance.RegisterVisit(port);
                    //        //    }
                    //        }
                    //    }
                    //}
                }
            }
