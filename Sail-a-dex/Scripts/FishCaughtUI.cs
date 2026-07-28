using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static sailadex.SAD_Plugin;
using static sailadex.Configs;

namespace sailadex
{
    public class FishCaughtUI : MonoBehaviour
    {
        public static FishCaughtUI Instance { get; private set; }

        public TextMesh[] _fishNameTMs;
        public TextMesh[] _caughtCountTMs;
        public Dictionary<string, GameObject> _fishBadgeGOs;

        public Dictionary<string, int> _caughtFish;
        public Dictionary<string, bool> _fishBadges;

        private static readonly int[] FISH_BADGE_AMOUNTS = { 25, 50, 100 };
        private static readonly int[] TOTAL_FISH_BADGE_AMOUNTS = { 50, 250, 500 };

        public static readonly string[] FishNames =
        {
            "templefish",
            "sunspot fish",
            "tuna",
            "blue shimmertail",
            "salmon",
            "eel",
            "blackfin hunter",
            "trout",
            "north fish",
            "swamp snapper",
            "blue bubbler",
            "fire fish"
        };

        public static readonly string[] TotalFishBadgeNames =
        {
            "caught50",
            "caught250",
            "caught500",
            "caughtAll"
        };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _caughtFish = new Dictionary<string, int>();
            _fishBadges = new Dictionary<string, bool>();
            _fishBadgeGOs = new Dictionary<string, GameObject>();

            foreach (string fishName in FishNames)
            {
                _caughtFish.Add(fishName, 0);

                foreach (int amt in FISH_BADGE_AMOUNTS)
                {
                    _fishBadges.Add($"{fishName}{amt}", false);
                }
            }

            foreach (string badgeName in TotalFishBadgeNames) 
            {
                _fishBadges.Add(badgeName, false);
            }
        }

        public void RegisterCatch(GameObject fish)
        {
            var fishName = fish.GetComponent<ShipItemFood>().name;
            if (!_caughtFish.ContainsKey(fishName))
            {
                LogWarning($"Fish caught {fishName} is not in fish caught log");
                return;
            }
            _caughtFish[fishName]++;
            CheckBadges(fishName);
            LogDebug($"Caught: {fishName}");
        }

        public void UpdatePage()
        {
            UpdateTexts();
            UpdateBadges();
        }

        private void UpdateTexts()
        {
            int i = 0;
            int catchSum = 0;
            foreach (KeyValuePair<string, int> fish in _caughtFish)
            {
                if (fishNamesHidden.Value)
                    _fishNameTMs[i].text = fish.Value > 0 ? fish.Key : "???";
                else
                    _fishNameTMs[i].text = fish.Key;
                _caughtCountTMs[i].text = fish.Value.ToString();
                catchSum += fish.Value;
                i++;
            }
            _fishNameTMs[_fishNameTMs.Length - 1].text = "Total";
            _caughtCountTMs[_caughtCountTMs.Length - 1].text = $"{catchSum}";
        }

        public void CheckBadges(string fishName)
        {
            CheckIndividualFishBadges(fishName);
            CheckAllFishBadges();
        }

        public void CheckIndividualFishBadges(string fishName)
        {
            foreach (int amt in FISH_BADGE_AMOUNTS)
            {
                if (!_fishBadges[$"{fishName}{amt}"] && _caughtFish[fishName] >= amt)
                {
                    if (notificationsEnabled.Value)
                        NotificationUiQueue.Instance.QueueNotification($"Caught {amt} {fishName}");
                    _fishBadges[$"{fishName}{amt}"] = true;
                }
            }
        }

        public void CheckAllFishBadges()
        {
            var catchSum = _caughtFish.Values.Sum();
            for (int i = 0; i < TOTAL_FISH_BADGE_AMOUNTS.Length; i++) 
            {
                if (!_fishBadges[TotalFishBadgeNames[i]] && catchSum >= TOTAL_FISH_BADGE_AMOUNTS[i])
                {
                    if (notificationsEnabled.Value)
                        NotificationUiQueue.Instance.QueueNotification($"Caught {TOTAL_FISH_BADGE_AMOUNTS[i]} fish");
                    _fishBadges[TotalFishBadgeNames[i]] = true;
                }
            }

            if (!_fishBadges[TotalFishBadgeNames[3]] && !_caughtFish.Values.Any(v => v.Equals(0)))
            {
                if (notificationsEnabled.Value)
                    NotificationUiQueue.Instance.QueueNotification($"Caught all fish");
                _fishBadges[TotalFishBadgeNames[3]] = true;
            }
        }

        private void UpdateBadges()
        {
            foreach (KeyValuePair<string, bool> badge in _fishBadges)
            {
                _fishBadgeGOs[badge.Key].SetActive(badge.Value);
            }
        }

        public void SetUIElems(TextMesh[] fishNameTMs, TextMesh[] caughtCountTMs, Dictionary<string, GameObject> fishBadgeGOs)
        {
            _fishNameTMs = fishNameTMs;
            _caughtCountTMs = caughtCountTMs;
            _fishBadgeGOs = fishBadgeGOs;
        }

        public void LoadFishCaughtUI()
        {
            ModData.GetDictEntry($"{PLUGIN_NAME}.CaughtFish", _caughtFish);
            ModData.GetDictEntry($"{PLUGIN_NAME}.FishBadges", _fishBadges);
        }

        public void SaveFishCaughtUI()
        {
            ModData.AddDictEntry($"{PLUGIN_NAME}.CaughtFish", _caughtFish);
            ModData.AddDictEntry($"{PLUGIN_NAME}.FishBadges", _fishBadges);
        }
    }
}
