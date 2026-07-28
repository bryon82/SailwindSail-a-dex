using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static sailadex.SAD_Plugin;
using static sailadex.Configs;

namespace sailadex
{
    public class PortsVisitedUI : MonoBehaviour
    {
        public static PortsVisitedUI Instance { get; private set; }

        private TextMesh[] _portNameTMs;
        private TextMesh[] _portVisitedTMs;
        private Dictionary<string, GameObject> _portBadgeGOs;
        private Dictionary<string, bool> _visitedPorts;
        private Dictionary<string, bool> _portBadges;

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
                LogWarning($"Attempted to register visit to unknown port: {portName}");
                return;
            }

            if (!_visitedPorts[portName])
            {
                _visitedPorts[portName] = true;
                CheckBadges();
            }

            LogDebug($"Visited: {portName}");
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
                    LogWarning("Not enough TextMesh objects to display all ports");
                    break;
                }

                _portNameTMs[i].text = GetPortDisplayName(port.Key, port.Value);
                _portVisitedTMs[i].text = port.Value ? "✓" : "✗";
                i++;
            }
        }

        private string GetPortDisplayName(string portName, bool visited)
        {
            return portNamesHidden.Value && !visited ? "???" : portName;
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
            if (!notificationsEnabled.Value)
                return;

            string message = region != "all"
                ? $"Visited all\n{region} ports"
                : $"Visited all ports";

            NotificationUiQueue.Instance.QueueNotification(message);
        }

        public void UpdateBadges()
        {
            foreach (KeyValuePair<string, bool> badge in _portBadges)
            {
                if (!_portBadgeGOs.ContainsKey(badge.Key))
                {
                    LogWarning($"Missing GameObject for badge: {badge.Key}");
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

        public void LoadPortsVisitedUI()
        {
            ModData.GetDictEntry($"{PLUGIN_NAME}.VisitedPorts", _visitedPorts);
            ModData.GetDictEntry($"{PLUGIN_NAME}.PortBadges", _portBadges);
        }

        public void SavePortsVisitedUI()
        {
            ModData.AddDictEntry($"{PLUGIN_NAME}.VisitedPorts", _visitedPorts);
            ModData.AddDictEntry($"{PLUGIN_NAME}.PortBadges", _portBadges);
        }
    }
}
