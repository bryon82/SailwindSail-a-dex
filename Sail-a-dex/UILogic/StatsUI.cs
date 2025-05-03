using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace sailadex
{
    public class StatsUI : MonoBehaviour
    {
        public static StatsUI Instance { get; private set; }
        private static readonly ManualLogSource logger = SAD_Plugin.logger;

        private Dictionary<string, TextMesh> _statTMs;
        private Dictionary<string, float> _floatStats;
        private Dictionary<string, int> _intStats;
        private Dictionary<string, bool[]> _boolArrayStats;        

        private Vector3 lastPosition;
        private string lastPortVisited;
        private int trackerTimer;
        private string lastStorm;

        private const int TRACKER_TIMER_VALUE = 1000;
        private const float MILES_CONVERSION_FACTOR = 61f;

        public static readonly string[] FloatStatNames =
        {
            "CargoMass",
            "TotalMass",
            "UnderwayTime",
            "MilesSailed"
        };

        public static readonly string[] IntStatNames =
        {
            "UnderwayDay",
            "PortsVisited",
            "MissionsCompleted",
            "DrinksTaken",
            "FoodsEaten",
            "TimesSmoked",
            "TimesSlept",
            "StormsWeathered",
            "FlotsamEncounters",
            "DenseFogEncounters",
            "SeaLifeEncounters",
            "FishingBonanzaEncounters",
            "IntenseStormEncounters",
        };

        private static readonly Dictionary<string, Func<bool>> _randomEncounterStats = new Dictionary<string, Func<bool>>
        {
            { "FlotsamEncounters", () => RandomEncounters.FlotsamEnabled },
            { "DenseFogEncounters", () => RandomEncounters.DenseFogEnabled },
            { "SeaLifeEncounters", () => RandomEncounters.IsSeaLifeEnabled },
            { "FishingBonanzaEncounters", () => RandomEncounters.FishingBonanzaEnabled },
            { "IntenseStormEncounters", () => RandomEncounters.IntenseStormEnabled }
        };
        
        public static IReadOnlyDictionary<string, Func<bool>> RandomEncounterStats => _randomEncounterStats;
        public static List<float> RandomEncounterStatYPos { get; set; }

        public IReadOnlyDictionary<string, float> FloatStats => _floatStats;
        public IReadOnlyDictionary<string, int> IntStats => _intStats;
        public IReadOnlyDictionary<string, bool[]> BoolArrayStats => _boolArrayStats;        

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;            
            
            _floatStats = new Dictionary<string, float>();
            _intStats = new Dictionary<string, int>();
            _boolArrayStats = new Dictionary<string, bool[]>();
            _statTMs = new Dictionary<string, TextMesh>();

            lastPosition = Vector3.zero;
            lastPortVisited = string.Empty;
            trackerTimer = TRACKER_TIMER_VALUE;
            lastStorm = string.Empty;

            foreach (string stat in FloatStatNames)
            {
                _floatStats.Add(stat, 0f);
                _floatStats.Add($"current{stat}", 0f);
                _floatStats.Add($"record{stat}", 0f);
            }

            foreach (string stat in IntStatNames)
            {
                _intStats.Add(stat, 0);
                _intStats.Add($"current{stat}", 0);
                _intStats.Add($"record{stat}", 0);
            }            

            foreach (string transit in Region.TransitCodes)
            {
                _floatStats.Add($"last{transit}TransitTime", 0f);
                _intStats.Add($"last{transit}TransitDay", 0);
                _floatStats.Add($"record{transit}TransitTime", 0f);
                _intStats.Add($"record{transit}TransitDay", 0);
            }

            foreach (Region region in Region.AllRegions)
            {
                _floatStats.Add($"{region.Code}UnderwayTime", 0f);
                _intStats.Add($"{region.Code}UnderwayDay", 0);
                _boolArrayStats.Add($"{region.Code}Transit", new bool[4]);
            }
        }

        #region mass

        public void RegisterCurrentMass()
        {
            if (GameState.currentBoat?.parent == null && GameState.lastBoat == null)
                return;

            var boatGameObject = GameState.currentBoat != null
                ? GameState.currentBoat.parent.gameObject
                : GameState.lastBoat.gameObject;

            _floatStats["currentCargoMass"] = boatGameObject
                .GetComponent<BoatMass>()
                .GetPrivateField<List<ItemRigidbody>>("itemsOnBoat")
                .Where(item => IsCargoItem(item))
                .Sum(item => item.GetBody().mass);
        }

        private bool IsCargoItem(ItemRigidbody item)
        {
            var good = item.GetShipItem().GetComponent<Good>();
            if (good == null) return false;

            string desc = good.sizeDescription;
            return desc.Contains("crate") ||
                   desc.Contains("package") ||
                   desc.Contains("barrel") ||
                   desc.Contains("bundle");
        }

        public void RegisterTotalMass(float totalMass)
        {
            _floatStats["currentTotalMass"] = totalMass;
        }

        private void UpdateMassRecords()
        {
            if (_floatStats["recordCargoMass"] < _floatStats["currentCargoMass"])
                _floatStats["recordCargoMass"] = _floatStats["currentCargoMass"];

            if (_floatStats["recordTotalMass"] < _floatStats["currentTotalMass"])
                _floatStats["recordTotalMass"] = _floatStats["currentTotalMass"];
        }

        #endregion

        #region transit

        public void RegisterUnderway(string islandName)
        {
            if (string.IsNullOrEmpty(islandName))
                return;

            UpdateMassRecords();
            ResetUnderwayTimers();

            if (islandName == "island 18 M (HappyBay)")
                return;

            foreach (Region region in Region.AllRegions)
            {
                if (region.Ports.Contains(islandName))
                {
                    TrackRegionUnderway(region.Code);
                    return;
                }
            }            
        }        

        private void ResetUnderwayTimers()
        {
            _floatStats["UnderwayTime"] = Sun.sun.globalTime;
            _intStats["UnderwayDay"] = GameState.day;
        }

        private void TrackRegionUnderway(string regionCode)
        {
            _floatStats[$"{regionCode}UnderwayTime"] = Sun.sun.globalTime;
            _intStats[$"{regionCode}UnderwayDay"] = GameState.day;

            for (int i = 0; i < Region.AllRegions.Count; i++)
            {
                _boolArrayStats[$"{regionCode}Transit"][i] = false;
            }
        }

        private void UpdateUnderwayRecords()
        {
            if (_intStats["currentUnderwayDay"] > _intStats["recordUnderwayDay"]
                || (_intStats["currentUnderwayDay"] == _intStats["recordUnderwayDay"]
                && _floatStats["currentUnderwayTime"] > _floatStats["recordUnderwayTime"]))
            {
                _intStats["recordUnderwayDay"] = _intStats["currentUnderwayDay"];
                _floatStats["recordUnderwayTime"] = _floatStats["currentUnderwayTime"];
            }

            _floatStats["UnderwayTime"] = 0f;
            _intStats["UnderwayDay"] = 0;
        }        

        public void RegisterMoored(string islandName)
        {
            if (islandName.IsNullOrWhiteSpace())
                return;

            UpdateStats();
            UpdateUnderwayRecords();            

            // fastest transit
            if (islandName.Equals("island 18 M (HappyBay)")) 
                return;

            ProcessRegionalArrival(islandName);                   
        }

        private void ProcessRegionalArrival(string islandName)
        {
            foreach (Region region in Region.AllRegions)
            {
                if (region.Ports.Contains(islandName))
                {
                    foreach (Region departureRegion in Region.AllRegions)
                    {
                        if (departureRegion.Code == region.Code)
                            continue;
                        CheckPossibleTransit(departureRegion.Code, region.Code, region.Index);
                    }
                    return;
                }
            }
        }

        private void CheckPossibleTransit(string departureRegion, string arrivalRegion, int arrivalIndex)
        {
            string transitKey = departureRegion + arrivalRegion;
            bool hasUnderwayTime = _floatStats[$"{departureRegion}UnderwayTime"] > 0f ||
                                  _intStats[$"{departureRegion}UnderwayDay"] > 0;

            bool alreadyTransited = _boolArrayStats[$"{departureRegion}Transit"][arrivalIndex];

            if (!alreadyTransited && hasUnderwayTime)
                CheckTransitTime(departureRegion, transitKey, arrivalIndex);
        }

        public void CheckTransitTime(string underwayKey, string transitCode, int destInt)
        {
            var transitDay = GameState.day - _intStats[$"{underwayKey}UnderwayDay"];
            var transitTime = Sun.sun.globalTime - _floatStats[$"{underwayKey}UnderwayTime"];

            if (transitTime < 0f)
            {
                transitTime += 24f;
                transitDay--;
            }

            _intStats[$"last{transitCode}TransitDay"] = transitDay;
            _floatStats[$"last{transitCode}TransitTime"] = transitTime;

            if ((_intStats[$"record{transitCode}TransitDay"] == 0 &&
                _floatStats[$"record{transitCode}TransitTime"] == 0f) ||
                _intStats[$"record{transitCode}TransitDay"] > transitDay ||
                (_intStats[$"record{transitCode}TransitDay"] == transitDay &&
                _floatStats[$"record{transitCode}TransitTime"] > transitTime))
            {
                _intStats[$"record{transitCode}TransitDay"] = transitDay;
                _floatStats[$"record{transitCode}TransitTime"] = transitTime;
                
                if (SAD_Plugin.notificationsEnabled.Value)
                    NotificationUiQueue.Instance.QueueNotification($"Fastest {FormatTransitName(transitCode)} time");
            }
            _boolArrayStats[$"{underwayKey}Transit"][destInt] = true;
        }

        public void PlayerTeleported()
        {
            logger.LogInfo("Player teleported, resetting current transits");
            foreach (Region region in Region.AllRegions)
            {
                int j = 0;
                for (int i = 0; i < Region.AllRegions.Count; i++)
                {
                    if (i != j)
                        _boolArrayStats[$"{region.Code}Transit"][i] = true;
                    j++;
                }
            }
        }

        #endregion
        
        #region distance

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

            _floatStats["currentMilesSailed"] += Vector3.Distance(lastPosition, currentPosition) * MILES_CONVERSION_FACTOR;
            lastPosition = currentPosition;
            trackerTimer = TRACKER_TIMER_VALUE;
        }

        public void UpdateMilesText()
        {
            _floatStats["MilesSailed"] = _floatStats["currentMilesSailed"]; 
        }

        #endregion

        #region increment stats

        public void IncrementIntStat(string statName)
        {
            _intStats[$"current{statName}"]++;
        }

        public void IncrementPortVisited(string port)
        {
            if (lastPortVisited == port) 
                return;

            IncrementIntStat("PortsVisited");
            lastPortVisited = port;
        }

        public void IncrementStormsWeathered()
        {
            var currentStorm = WeatherStorms.instance.GetCurrentStorm().name;
            if (lastStorm == currentStorm)
                return;

            logger.LogDebug($"Weathering {currentStorm}");
            IncrementIntStat("StormsWeathered");
            lastStorm = currentStorm;            
        }

        public void ClearLastStorm()
        {
            if (lastStorm.IsNullOrWhiteSpace())
                return;

            logger.LogDebug($"Storm cleared");
            lastStorm = string.Empty;
        }

        #endregion

        #region update page

        public void UpdatePage()
        {
            UpdateStats();
            UpdateTexts();
        }

        private void UpdateStats()
        {
            if (_intStats["UnderwayDay"] > 0 || _floatStats["UnderwayTime"] > 0f)
            {
                _intStats["currentUnderwayDay"] = GameState.day - _intStats["UnderwayDay"];
                _floatStats["currentUnderwayTime"] = Sun.sun.globalTime - _floatStats["UnderwayTime"];
            }

            if (_floatStats["currentUnderwayTime"] < 0f)
            {
                _floatStats["currentUnderwayTime"] += 24f;
                _intStats["currentUnderwayDay"]--;
            }
        }

        private void UpdateTexts()
        {
            UpdateFloatTexts();
            UpdateIntTexts();
            UpdateTransitTexts();
        }

        private void UpdateFloatTexts()
        {
            foreach (string stat in FloatStatNames)
            {
                switch (stat)
                {
                    case "UnderwayTime":
                        _statTMs[stat].text = FormatStatName(stat);
                        _statTMs["currentUnderwayTime"].text = FormatUnderwayTime(_intStats["currentUnderwayDay"], _floatStats["currentUnderwayTime"]);
                        _statTMs["recordUnderwayTime"].text = FormatUnderwayTime(_intStats["recordUnderwayDay"], _floatStats["recordUnderwayTime"]);
                        break;
                    case "CargoMass":
                        _statTMs[stat].text = FormatStatName(stat);
                        _statTMs["currentCargoMass"].text = _floatStats["currentCargoMass"] == 0f ? "-" : $"{_floatStats["currentCargoMass"]:#,##0.#} lbs";
                        _statTMs["recordCargoMass"].text = _floatStats["recordCargoMass"] == 0f ? "-" : $"{_floatStats["recordCargoMass"]:#,##0.#} lbs";
                        break;
                    case "TotalMass":
                        _statTMs[stat].text = FormatStatName(stat);
                        _statTMs["currentTotalMass"].text = _floatStats["currentTotalMass"] == 0f ? "-" : $"{_floatStats["currentTotalMass"]:#,##0.#} lbs";
                        _statTMs["recordTotalMass"].text = _floatStats["recordTotalMass"] == 0f ? "-" : $"{_floatStats["recordTotalMass"]:#,##0.#} lbs";
                        break;
                    case "MilesSailed":
                        _statTMs[stat].text = FormatStatName(stat);
                        if (SAD_Plugin.updateMilesSailed.Value == "realtime")
                            _statTMs["currentMilesSailed"].text = $"{_floatStats["currentMilesSailed"]:#,##0.#}";
                        else
                            _statTMs["currentMilesSailed"].text = $"{_floatStats["MilesSailed"]:#,##0.#}";
                        break;
                    default:
                        _statTMs[stat].text = FormatStatName(stat);
                        _statTMs[$"current{stat}"].text = _floatStats[$"current{stat}"].ToString();
                        _statTMs[$"record{stat}"].text = _floatStats[$"record{stat}"].ToString();
                        break;
                }
            }
        }

        private void UpdateIntTexts()
        {
            var posQueue = new Queue<float>(RandomEncounterStatYPos);
            foreach (string stat in IntStatNames)
            {
                if (stat == "UnderwayDay") 
                    continue;
                if (_randomEncounterStats.ContainsKey(stat))
                    PositionRandomEnounterStat(stat, posQueue);

                _statTMs[stat].text = FormatStatName(stat);
                _statTMs[$"current{stat}"].text = $"{_intStats[$"current{stat}"]:#,##0}";
            }
        }        

        private void UpdateTransitTexts()
        {
            foreach (string transit in Region.TransitCodes)
            {
                _statTMs[transit].text = FormatTransitName(transit);
                _statTMs[$"last{transit}"].text = FormatUnderwayTime(_intStats[$"last{transit}TransitDay"], _floatStats[$"last{transit}TransitTime"]);
                _statTMs[$"record{transit}"].text = FormatUnderwayTime(_intStats[$"record{transit}TransitDay"], _floatStats[$"record{transit}TransitTime"]);
            }
        }

        private string FormatUnderwayTime(int underwayDay, float underwayTime)
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

        private string FormatStatName(string name)
        {
            if (name.Contains("Encounters"))
                name = name.Replace("Encounters", "Encs");
            return Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");
        }

        private string FormatTransitName(string name)
        {
            var temp = FormatStatName(name);
            return temp.ToUpper().Insert(temp.IndexOf(' '), " to");
        }

        private void PositionRandomEnounterStat(string stat, Queue<float> posQueue)
        {
            if (RandomEncounters.pluginInstance == null || !_randomEncounterStats[stat]())
            {
                _statTMs[stat].gameObject.SetActive(false);
            }
            else if (RandomEncounters.pluginInstance != null && _randomEncounterStats[stat]())
            {
                _statTMs[stat].gameObject.SetActive(true);
                var pos = _statTMs[stat].transform.localPosition;
                _statTMs[stat].transform.localPosition = new Vector3(pos.x, posQueue.Dequeue(), pos.z);
            }
        }

        #endregion

        public void SetUIElems(Dictionary<string, TextMesh> statTMs)
        {
            _statTMs = statTMs;
        }

        public void LoadFloatStats(Dictionary<string, float> floatStats)
        {
            SaveLoadPatches.LoadDictionary(floatStats, _floatStats);
        }

        public void LoadIntStats(Dictionary<string, int> intStats)
        {
            SaveLoadPatches.LoadDictionary(intStats, _intStats);
        }

        public void LoadBoolArrayStats(Dictionary<string, bool[]> boolArrayStats)
        {            
            foreach (var stat in boolArrayStats)
            {
                if (!_boolArrayStats.ContainsKey(stat.Key))
                {
                    logger.LogWarning($"Attempted to set unknown boolArrayStat: {stat.Key}");
                    continue;
                }
                
                _boolArrayStats[stat.Key] = (bool[])stat.Value.Clone();
            }
            
        }

        //Testing
        //public void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.P))
        //    {
        //        logger.LogDebug($"LastBoat: {GameState.lastBoat} CurrentBoat: {GameState.currentBoat?.parent}");
        //        var stormDistance = (WeatherStorms.currentStormDistance - WeatherStorms.instance.GetPrivateField<WanderingStorm>("currentStorm").GetRadius()) / WeatherStorms.instance.GetPrivateField<float>("currentStormRange");
        //        logger.LogDebug($"Storm distance: {stormDistance}");
        //    }
        //}
    }
}