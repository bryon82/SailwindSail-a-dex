using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using static sailadex.SAD_Plugin;

namespace sailadex
{
    internal class ModData
    {
        public static void AddEntry<T>(string dataName, T data)
        {
            var dataString = data == null ? string.Empty : data.ToString();
            if (GameState.modData.ContainsKey(dataName))
                GameState.modData[dataName] = dataString;
            else
                GameState.modData.Add(dataName, dataString);
        }

        public static T GetEntry<T>(string dataName)
        {
            if (!GameState.modData.ContainsKey(dataName))
            {
                LogWarning($"GetEntry: {dataName} not found in modData");
                return default;
            }

            SaveLoadPatches.loadedNewData = true;
            var dataString = GameState.modData[dataName];
            return (T)Convert.ChangeType(dataString, typeof(T));
        }

        public static void AddDictEntry<K, T>(string dataName, Dictionary<K, T> data)
        {
            var sb = new StringBuilder();

            if (typeof(T) == typeof(bool[]))
                foreach (var kvp in data)
                    sb.AppendLine($"{kvp.Key}|{string.Join(",", (bool[])(object)kvp.Value)}");
            else if (typeof(T) == typeof(float))
                foreach (var kvp in data)
                    sb.AppendLine($"{kvp.Key}|{((float)(object)kvp.Value).ToString(CultureInfo.InvariantCulture)}");
            else
                foreach (var kvp in data)
                    sb.AppendLine($"{kvp.Key}|{kvp.Value}");

            var dataString = sb.ToString();

            if (GameState.modData.ContainsKey(dataName))
                GameState.modData[dataName] = dataString;
            else
                GameState.modData.Add(dataName, dataString);
        }

        public static void GetDictEntry<K, T>(string dataName, Dictionary<K, T> gameDict)
        {
            if (!GameState.modData.ContainsKey(dataName))
            {
                LogWarning($"LoadDictFromModData: {dataName} not found in modData");
                return;
            }

            var dataString = GameState.modData[dataName];
            var lines = dataString.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Trim().Split('|');
                if (parts.Length < 2)
                    continue;

                var key = (K)Convert.ChangeType(parts[0], typeof(K));

                if (!gameDict.ContainsKey(key))
                {
                    LogWarning($"LoadDictFromModData: {key} not found in game");
                    continue;
                }

                gameDict[key] = ParseValue<T>(parts[1]);
            }
        }

        private static T ParseValue<T>(string raw)
        {
            if (typeof(T) == typeof(bool[]))
            {
                var boolArray = Array.ConvertAll(raw.Split(','), bool.Parse);
                return (T)(object)boolArray;
            }

            if (typeof(T) == typeof(float))
                return (T)(object)float.Parse(raw, CultureInfo.InvariantCulture);

            return (T)Convert.ChangeType(raw, typeof(T));
        }
    }
}
