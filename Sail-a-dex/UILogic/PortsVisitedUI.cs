using BepInEx.Logging;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace sailadex
{
    public class PortsVisitedUI : MonoBehaviour
    {
        public static PortsVisitedUI Instance { get; private set; }
        private static readonly ManualLogSource logger = SAD_Plugin.logger;

        private TextMesh[] _portNameTMs;
        private TextMesh[] _portVisitedTMs;
        private Dictionary<string, GameObject> _portBadgeGOs;
        private Dictionary<string, bool> _visitedPorts;
        private Dictionary<string, bool> _portBadges;

        public IReadOnlyDictionary<string, bool> VisitedPorts => _visitedPorts;
        public IReadOnlyDictionary<string, bool> PortBadges => _portBadges;        
                
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _visitedPorts = new Dictionary<string, bool>();
            foreach (string port in Region.AllPorts)
            {
                _visitedPorts[port] = false;
            }

            _portBadges = new Dictionary<string, bool>();
            foreach (string portBadge in Region.AllBadgeNames)
            {
                _portBadges[portBadge] = false;
            }
        }

        public void RegisterVisit(string portName)
        {
            if (!_visitedPorts.ContainsKey(portName))
            {
                logger.LogWarning($"Attempted to register visit to unknown port: {portName}");
                return;
            }

            if (!_visitedPorts[portName])
            {
                _visitedPorts[portName] = true;
                CheckBadges();
            }

            logger.LogDebug($"Visited: {portName}");
        }

        public void UpdatePage()
        {
            UpdateTexts();
            UpdateBadges();
        }

        private void UpdateTexts()
        {
            int i = 0;
            foreach (KeyValuePair<string, bool> port in _visitedPorts)
            {
                if (i >= _portNameTMs.Length || i >= _portVisitedTMs.Length)
                {
                    logger.LogWarning("Not enough TextMesh objects to display all ports");
                    break;
                }

                _portNameTMs[i].text = GetPortDisplayName(port.Key, port.Value);
                _portVisitedTMs[i].text = port.Value ? "✓" : "✗";
                i++;
            }
        }

        private string GetPortDisplayName(string portName, bool visited)
        {
            return SAD_Plugin.portNamesHidden.Value && !visited ? "???" : portName;
        }

        public void CheckBadges()
        {
            foreach (var region in Region.AllRegions)
            {
                if (!_portBadges[region.BadgeName] && region.Ports.All(p => _visitedPorts[p]))
                {
                    _portBadges[region.BadgeName] = true;
                    ShowBadgeNotification(region.Name);
                }
            }

            if (!_portBadges["allPortsBadge"] && Region.AllPorts.All(p => _visitedPorts[p]))
            {
                _portBadges["allPortsBadge"] = true;
                ShowBadgeNotification("all");
            }
        }

        private void ShowBadgeNotification(string region)
        {
            if (!SAD_Plugin.notificationsEnabled.Value)
                return;

            string message = name != "all"
                ? $"Visited all {region} ports"
                : $"Visited all ports";

            NotificationUiQueue.Instance.QueueNotification(message);
        }

        public void UpdateBadges()
        {
            foreach (KeyValuePair<string, bool> badge in _portBadges)
            {
                if (!_portBadgeGOs.ContainsKey(badge.Key))
                {
                    logger.LogWarning($"Missing GameObject for badge: {badge.Key}");
                    continue;
                }

                _portBadgeGOs[badge.Key].SetActive(badge.Value);
            }
        }
        
        public void SetUIElems(TextMesh[] portNameTMs, TextMesh[] portVisitedTMs, Dictionary<string, GameObject> portBadgeGOs)
        {
            _portNameTMs = portNameTMs;
            _portVisitedTMs = portVisitedTMs;
            _portBadgeGOs = portBadgeGOs;
        }

        public void LoadVisitedPorts(Dictionary<string, bool> visitedPorts)
        {
            SaveLoadPatches.LoadDictionary(visitedPorts, _visitedPorts);
        }

        public void LoadPortBadges(Dictionary<string, bool> portBadges)
        {
            SaveLoadPatches.LoadDictionary(portBadges, _portBadges);
        }        
    }
}
