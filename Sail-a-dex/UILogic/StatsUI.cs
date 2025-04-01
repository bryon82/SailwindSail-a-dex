using BepInEx;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace sailadex
{
    public class StatsUI : MonoBehaviour
    {
        public static StatsUI instance;
        public Dictionary<string, float> floatStats;
        public Dictionary<string, int> intStats;
        public Dictionary<string, bool[]> boolArrayStats;
        public Dictionary<string, TextMesh> statTMs;
        private Vector3 lastPosition;
        private string lastPortVisited;
        private int trackerTimer;
        private string lastStorm;

        private void Awake()
        {
            instance = this;
            floatStats = new Dictionary<string, float>();
            intStats = new Dictionary<string, int>();
            boolArrayStats = new Dictionary<string, bool[]>();
            statTMs = new Dictionary<string, TextMesh>();
            lastPosition = new Vector3();
            lastPortVisited = "";
            trackerTimer = 1000;
            lastStorm = "";

            foreach (string stat in Names.floatStatNames)
            {
                floatStats.Add(stat, 0f);
                floatStats.Add("current" + stat, 0f);
                floatStats.Add("record" + stat, 0f);
            }
            
            foreach (string stat in Names.intStatNames)
            {
                intStats.Add(stat, 0);
                intStats.Add("current" + stat, 0);
                intStats.Add("record" + stat, 0);
            }            

            foreach (string transit in Names.regionTransitNames)
            {
                floatStats.Add("last" + transit + "TransitTime", 0f);
                intStats.Add("last" + transit + "TransitDay", 0);
                floatStats.Add("record" + transit + "TransitTime", 0f);
                intStats.Add("record" + transit + "TransitDay", 0);
            }

            foreach (string region in Names.regions)
            {
                floatStats.Add(region + "UnderwayTime", 0f);
                intStats.Add(region + "UnderwayDay", 0);
                boolArrayStats.Add(region + "Transit", new bool[4]);
            }
        }

        public void RegisterCurrentMass()
        {
            if (GameState.currentBoat?.parent == null && GameState.lastBoat == null) return;

            var boatGameObject = GameState.currentBoat != null ? GameState.currentBoat.parent.gameObject : GameState.lastBoat.gameObject;  
            floatStats["currentCargoMass"] = boatGameObject
                .GetComponent<BoatMass>()
                .GetPrivateField<List<ItemRigidbody>>("itemsOnBoat")
                .Where(item => item.GetShipItem().GetComponent<Good>() != null
                    && (item.GetShipItem().GetComponent<Good>().sizeDescription.Contains("crate")
                    || item.GetShipItem().GetComponent<Good>().sizeDescription.Contains("package")
                    || item.GetShipItem().GetComponent<Good>().sizeDescription.Contains("barrel")
                    || item.GetShipItem().GetComponent<Good>().sizeDescription.Contains("bundle")))
                .Sum(item => item.GetBody().mass);            
        }

        public void RegisterTotalMass(float totalMass)
        {
            floatStats["currentTotalMass"] = totalMass;
        }

        public void RegisterUnderway(string islandName)
        {
            if (islandName == null || islandName == "") return;

            if (floatStats["recordCargoMass"] < floatStats["currentCargoMass"])
                floatStats["recordCargoMass"] = floatStats["currentCargoMass"];
           
            if (floatStats["recordTotalMass"] < floatStats["currentTotalMass"])
                floatStats["recordTotalMass"] = floatStats["currentTotalMass"];

            floatStats["UnderwayTime"] = Sun.sun.globalTime;
            intStats["UnderwayDay"] = GameState.day;

            //fastest transit
            if (islandName == "island 18 M (HappyBay)") return;

            if (Names.alankhIslands.Contains(islandName)) {
                floatStats["AaUnderwayTime"] = Sun.sun.globalTime;
                intStats["AaUnderwayDay"] = GameState.day;
                for (int i = 0; i < 4; i++)
                {
                    boolArrayStats["AaTransit"][i] = false;
                }                
                return;
            }
            if (Names.emeraldIslands.Contains(islandName))
            {
                floatStats["EaUnderwayTime"] = Sun.sun.globalTime;
                intStats["EaUnderwayDay"] = GameState.day;
                for (int i = 0; i < 4; i++)
                {
                    boolArrayStats["EaTransit"][i] = false;
                }
                return;
            }
            if (Names.mediIslands.Contains(islandName))
            {
                floatStats["AeUnderwayTime"] = Sun.sun.globalTime;
                intStats["AeUnderwayDay"] = GameState.day;
                for (int i = 0; i < 4; i++)
                {
                    boolArrayStats["AeTransit"][i] = false;
                }
                return;
            }
            if (Names.lagoonIslands.Contains(islandName))
            {
                floatStats["FflUnderwayTime"] = Sun.sun.globalTime;
                intStats["FflUnderwayDay"] = GameState.day;
                for (int i = 0; i < 4; i++)
                {
                    boolArrayStats["FflTransit"][i] = false;
                }
            }
        }

        public void RegisterMoored(string islandName)
        {
            if (islandName == null || islandName == "")
                return;

            UpdateStats();

            if (intStats["currentUnderwayDay"] > intStats["recordUnderwayDay"] 
                || (intStats["currentUnderwayDay"] == intStats["recordUnderwayDay"]
                && floatStats["currentUnderwayTime"] > floatStats["recordUnderwayTime"]))
            {
                intStats["recordUnderwayDay"] = intStats["currentUnderwayDay"];
                floatStats["recordUnderwayTime"] = floatStats["currentUnderwayTime"];
            }

            floatStats["UnderwayTime"] = 0f;
            intStats["UnderwayDay"] = 0;

            // fastest transit
            if (islandName == "island 18 M (HappyBay)") return;

            if (Names.alankhIslands.Contains(islandName))
            {               
                if (!boolArrayStats["EaTransit"][0] && (floatStats["EaUnderwayTime"] > 0f || intStats["EaUnderwayDay"] > 0))
                    CheckTransitTime("Ea", "EaAa", 0);
                if (!boolArrayStats["AeTransit"][0] && (floatStats["AeUnderwayTime"] > 0f || intStats["AeUnderwayDay"] > 0))
                    CheckTransitTime("Ae", "AeAa", 0);
                if (!boolArrayStats["FflTransit"][0] && (floatStats["FflUnderwayTime"] > 0f || intStats["FflUnderwayDay"] > 0))
                    CheckTransitTime("Ffl", "FflAa", 0);
                return;
            }
            if (Names.emeraldIslands.Contains(islandName))
            {   
                if (!boolArrayStats["AaTransit"][1] && (floatStats["AaUnderwayTime"] > 0f || intStats["AaUnderwayDay"] > 0))
                    CheckTransitTime("Aa", "AaEa", 1);
                if (!boolArrayStats["AeTransit"][1] && (floatStats["AeUnderwayTime"] > 0f || intStats["AeUnderwayDay"] > 0))
                    CheckTransitTime("Ae", "AeEa", 1);
                if (!boolArrayStats["FflTransit"][1] && (floatStats["FflUnderwayTime"] > 0f || intStats["FflUnderwayDay"] > 0))
                    CheckTransitTime("Ffl", "FflEa", 1);
                return;
            }
            if (Names.mediIslands.Contains(islandName))
            {
                if (!boolArrayStats["AaTransit"][2] && (floatStats["AaUnderwayTime"] > 0f || intStats["AaUnderwayDay"] > 0))
                    CheckTransitTime("Aa", "AaAe", 2);
                if (!boolArrayStats["EaTransit"][2] && (floatStats["EaUnderwayTime"] > 0f || intStats["EaUnderwayDay"] > 0))
                    CheckTransitTime("Ea", "EaAe", 2);
                if (!boolArrayStats["FflTransit"][2] && (floatStats["FflUnderwayTime"] > 0f || intStats["FflUnderwayDay"] > 0))
                    CheckTransitTime("Ffl", "FflAe", 2);
                return;
            }
            if (Names.lagoonIslands.Contains(islandName))
            {
                if (!boolArrayStats["AaTransit"][3] && (floatStats["AaUnderwayTime"] > 0f || intStats["AaUnderwayDay"] > 0))
                    CheckTransitTime("Aa", "AaFfl", 3);
                if (!boolArrayStats["EaTransit"][3] && (floatStats["EaUnderwayTime"] > 0f || intStats["EaUnderwayDay"] > 0))
                    CheckTransitTime("Ea", "EaFfl", 3);
                if (!boolArrayStats["AeTransit"][3] && (floatStats["AeUnderwayTime"] > 0f || intStats["AeUnderwayDay"] > 0))
                    CheckTransitTime("Ae", "AeFfl", 3);
            }            
        }

        public void CheckTransitTime(string underwayKey, string transitCode, int destInt)
        {
            var transitDay = GameState.day - intStats[underwayKey + "UnderwayDay"];
            var transitTime = Sun.sun.globalTime - floatStats[underwayKey + "UnderwayTime"];
            
            if (transitTime < 0f)
            {
                transitTime += 24f;
                transitDay--;
            }

            intStats["last" + transitCode + "TransitDay"] = transitDay;
            floatStats["last" + transitCode + "TransitTime"] = transitTime;

            if ((intStats["record" + transitCode + "TransitDay"] == 0
                && floatStats["record" + transitCode + "TransitTime"] == 0f)
                || intStats["record" + transitCode + "TransitDay"] > transitDay
                || (intStats["record" + transitCode + "TransitDay"] == transitDay
                && floatStats["record" + transitCode + "TransitTime"] > transitTime))
            {
                intStats["record" + transitCode + "TransitDay"] = transitDay;
                floatStats["record" + transitCode + "TransitTime"] = transitTime;
                if (Plugin.notificationsEnabled.Value)
                    NotificationUiQueue.instance.QueueNotification($"Fastest {AddTo(transitCode)} time");
            }
            boolArrayStats[underwayKey + "Transit"][destInt] = true;
        }

        public void IncrementIntStat(string statName)
        {
            intStats["current" + statName]++;
        }

        public void UpdatePage()
        {
            UpdateStats();
            UpdateTexts();
            //UpdateBadges();
        }

        private void UpdateStats()
        {
            if (intStats["UnderwayDay"] > 0 || floatStats["UnderwayTime"] > 0f)
            {
                intStats["currentUnderwayDay"] = GameState.day - intStats["UnderwayDay"];
                floatStats["currentUnderwayTime"] = Sun.sun.globalTime - floatStats["UnderwayTime"];
            }
            if (floatStats["currentUnderwayTime"] < 0f)
            {
                floatStats["currentUnderwayTime"] += 24f;
                intStats["currentUnderwayDay"]--;
            }
        }

        private void UpdateTexts()
        {
            foreach (string stat in Names.floatStatNames)
            {                
                switch (stat)
                {
                    case "UnderwayTime":
                        statTMs[stat].text = AddSpace(stat);
                        statTMs["currentUnderwayTime"].text = UnderwayText(intStats["currentUnderwayDay"], floatStats["currentUnderwayTime"]);
                        statTMs["recordUnderwayTime"].text = UnderwayText(intStats["recordUnderwayDay"], floatStats["recordUnderwayTime"]);
                        break;
                    case "CargoMass":
                        statTMs[stat].text = AddSpace(stat);
                        statTMs["currentCargoMass"].text = floatStats["currentCargoMass"] == 0f ? "-" : $"{floatStats["currentCargoMass"]:#,##0.#} lbs";
                        statTMs["recordCargoMass"].text = floatStats["recordCargoMass"] == 0f ? "-" : $"{floatStats["recordCargoMass"]:#,##0.#} lbs";
                        break;
                    case "TotalMass":
                        statTMs[stat].text = AddSpace(stat);
                        statTMs["currentTotalMass"].text = floatStats["currentTotalMass"] == 0f ? "-" : $"{floatStats["currentTotalMass"]:#,##0.#} lbs";
                        statTMs["recordTotalMass"].text = floatStats["recordTotalMass"] == 0f ? "-" : $"{floatStats["recordTotalMass"]:#,##0.#} lbs";
                        break;
                    case "MilesSailed":
                        statTMs[stat].text = AddSpace(stat);
                        if (Plugin.updateMilesSailed.Value == "realtime")
                            statTMs["currentMilesSailed"].text = $"{floatStats["currentMilesSailed"]:#,##0.#}";
                        else
                            statTMs["currentMilesSailed"].text = $"{floatStats["MilesSailed"]:#,##0.#}";
                        break;
                    default:
                        statTMs[stat].text = AddSpace(stat);
                        statTMs["current" + stat].text = floatStats["current" + stat].ToString();
                        statTMs["record" + stat].text = floatStats["record" + stat].ToString();
                        break;
                }
            }

            foreach (string stat in Names.intStatNames)
            {
                if (stat == "UnderwayDay") continue;
                if (stat == "FlotsamEncounters" && (RandomEncounters.pluginInstance == null || !RandomEncounters.flotsamEnabled)) continue;
                if (stat == "DenseFogEncounters" && (RandomEncounters.pluginInstance == null || !RandomEncounters.denseFogEnabled)) continue;
                if (stat == "SeaLifeEncounters" && (RandomEncounters.pluginInstance == null || !RandomEncounters.isSeaLifeEnabled)) continue;

                statTMs[stat].text = AddSpace(stat);
                statTMs["current" + stat].text = $"{intStats["current" + stat]:#,##0}";                
            }

            foreach (string transit in Names.regionTransitNames)
            {
                statTMs[transit].text = AddTo(transit);
                statTMs["last" + transit].text = UnderwayText(intStats["last" + transit + "TransitDay"], floatStats["last" + transit + "TransitTime"]);
                statTMs["record" + transit].text = UnderwayText(intStats["record" + transit + "TransitDay"], floatStats["record" + transit + "TransitTime"]);
            }
        }

        private string UnderwayText(int underwayDay, float underwayTime)
        {
            if (underwayDay == 0 && underwayTime == 0f)
                return "-";
            if (underwayDay > 0)
            {
                var dayText = underwayDay == 1 ? "Day" : "Days";
                return $"{underwayDay} {dayText} {underwayTime:0.0} Hours";
            }
            else
            {
                return $"{underwayTime:0.0} Hours";
            }
        }

        private string AddSpace(string name)
        {
            return Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
        }

        private string AddTo(string name)
        {
            var temp = AddSpace(name);
            return temp.ToUpper().Insert(temp.IndexOf(' '), " to");
        }

        public void PlayerTeleported()
        {
            Plugin.logger.LogInfo("Player teleported, resetting current transits");
            foreach (string region in Names.regions)
            {
                int j = 0;
                for (int i = 0; i < 4; i++) 
                {
                    if (i != j)
                        boolArrayStats[region + "Transit"][i] = true;
                    j++;
                }
            }
        }

        public void TrackDistance()
        {                     
            var globePosition = FloatingOriginManager.instance.GetGlobeCoords(GameState.currentBoat);
            var currentPosition = new Vector3(globePosition.x, 0f, globePosition.z);
            if (lastPosition == Vector3.zero)
            {
                lastPosition = currentPosition;
                return;
            }
            if (trackerTimer > 1)
            {
                trackerTimer -= 1;
                return;
            }

            floatStats["currentMilesSailed"] += Vector3.Distance(lastPosition, currentPosition) * 61;
            lastPosition = currentPosition;
            trackerTimer = 1000;
        }

        public void UpdateMilesText()
        {
            floatStats["MilesSailed"] = floatStats["currentMilesSailed"]; 
        }

        public void IncrementPortVisited(string port)
        {
            if (lastPortVisited == port) return;
            IncrementIntStat("PortsVisited");
            lastPortVisited = port;
        }

        public void IncrementStormsWeathered()
        {
            var currentStorm = WeatherStorms.instance.GetCurrentStorm().name;
            if (lastStorm == currentStorm)
                return;

            Plugin.logger.LogDebug($"Weathering {currentStorm}");
            IncrementIntStat("StormsWeathered");
            lastStorm = currentStorm;
            
        }

        public void ClearLastStorm()
        {
            if (lastStorm.IsNullOrWhiteSpace())
                return;

            Plugin.logger.LogDebug($"Storm cleared");
            lastStorm = "";
        }

        //Testing
        //public void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.P))
        //    {
        //        Plugin.logger.LogDebug($"LastBoat: {GameState.lastBoat} CurrentBoat: {GameState.currentBoat?.parent}");
        //        var stormDistance = (WeatherStorms.currentStormDistance - WeatherStorms.instance.GetPrivateField<WanderingStorm>("currentStorm").GetRadius()) / WeatherStorms.instance.GetPrivateField<float>("currentStormRange");
        //        Plugin.logger.LogDebug($"Storm distance: {stormDistance}");
        //    }
        //}
    }
}