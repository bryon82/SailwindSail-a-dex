using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using static sailadex.SAD_Plugin;

namespace sailadex
{
    internal class AssetsLoader
    {
        public static bool BadgesLoaded => _fishBadgesLoaded && _portBadgesLoaded;
        public static AudioClip NotificationSound { get; private set; }
        public static Dictionary<string, Material> Materials { get; private set; }

        private static Dictionary<string, Texture2D> _textures;
        private static Dictionary<string, Texture2D> _fishTextures;
        private static Dictionary<int, Texture2D> _badgeRings;
        private static bool _fishBadgesLoaded;
        private static bool _portBadgesLoaded;

        private static readonly List<string> assetPaths = new List<string>() {
            Path.Combine(Path.GetDirectoryName(Instance.Info.Location), "Assets"),
            Path.Combine(Path.GetDirectoryName(Instance.Info.Location))
        };

        public static string FindAssetPath(string fileName)
        {
            foreach (string basePath in assetPaths)
            {
                string fullPath = Path.Combine(basePath, fileName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            return null;
        }

        public static void Start()
        {
            Materials = new Dictionary<string, Material>();
            _textures = new Dictionary<string, Texture2D>();
            _fishTextures = new Dictionary<string, Texture2D>();
            _badgeRings = new Dictionary<int, Texture2D>();

            Instance.StartCoroutine(LoadAssetsAsync());
        }

        private static IEnumerator LoadAssetsAsync()
        {
            var audioCoroutine = Instance.StartCoroutine(LoadAudio());
            var fishBadgeCoroutine = Instance.StartCoroutine(LoadFishBadges());
            var portBadgeCoroutine = Instance.StartCoroutine(LoadPortBadges());

            yield return audioCoroutine;
            yield return fishBadgeCoroutine;
            yield return portBadgeCoroutine;

            LogInfo("Assets loaded");
        }

        private static IEnumerator LoadAudio()
        {
            var clipPath = FindAssetPath("twoBells.wav");
            using (var webRequest = UnityWebRequestMultimedia.GetAudioClip($"file://{clipPath}", AudioType.WAV))
            {
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    LogError(webRequest.error);
                    yield break;
                }

                AudioClip clip = DownloadHandlerAudioClip.GetContent(webRequest);
                clip.name = "twoBells";
                NotificationSound = clip;

                LogDebug("Audio loaded.");
            }
        }

        private static IEnumerator LoadFishBadges()
        {
            yield return Instance.StartCoroutine(LoadBadgeRings());
            yield return Instance.StartCoroutine(LoadFishTextures());
            yield return Instance.StartCoroutine(GenerateFishBadges());

            _fishBadgesLoaded = true;
            LogDebug("Fish badges loaded.");
        }

        private static IEnumerator LoadBadgeRings()
        {
            int[] amountNums = { 25, 50, 100 };
            List<Coroutine> ringCoroutines = new List<Coroutine>();

            foreach (int amount in amountNums)
            {
                var ringName = amount + "Fish";
                var path = FindAssetPath(ringName + ".png");
                ringCoroutines.Add(Instance.StartCoroutine(LoadBadgeRing(path, amount)));
            }

            foreach (var coroutine in ringCoroutines)
            {
                yield return coroutine;
            }
        }

        private static IEnumerator LoadBadgeRing(string path, int amount)
        {
            if (!File.Exists(path))
            {
                LogError($"Badge ring file not found: {path}");
                yield break;
            }

            var texture2D = new Texture2D(1, 1);
            byte[] fileData = null;

            yield return Instance.StartCoroutine(LoadFile(path, result => fileData = result));

            if (fileData != null && fileData.Length > 0)
            {
                bool success = ImageConversion.LoadImage(texture2D, fileData);

                if (!success)
                {
                    LogError($"Failed to load badge ring from bytes: {path}");
                    yield break;
                }

                _badgeRings[amount] = texture2D;
            }
        }

        private static IEnumerator LoadFishTextures()
        {
            List<Coroutine> fishCoroutines = new List<Coroutine>();

            foreach (string fishName in FishCaughtUI.FishNames)
            {
                var path = FindAssetPath(fishName + ".png");
                fishCoroutines.Add(Instance.StartCoroutine(LoadFishTexture(path, fishName)));
            }

            foreach (var coroutine in fishCoroutines)
            {
                yield return coroutine;
            }
        }

        private static IEnumerator LoadFishTexture(string path, string fishName)
        {
            if (!File.Exists(path))
            {
                LogError($"Fish texture file not found: {path}");
                yield break;
            }

            var texture2D = new Texture2D(1, 1);
            byte[] fileData = null;

            yield return Instance.StartCoroutine(LoadFile(path, result => fileData = result));

            if (fileData != null && fileData.Length > 0)
            {
                bool success = ImageConversion.LoadImage(texture2D, fileData);

                if (!success)
                {
                    LogError($"Failed to load fish texture from bytes: {path}");
                    yield break;
                }

                _fishTextures[fishName] = texture2D;
            }
        }

        private static IEnumerator GenerateFishBadges()
        {
            int[] amountNums = { 25, 50, 100 };

            foreach (string fishName in FishCaughtUI.FishNames)
            {
                if (!_fishTextures.ContainsKey(fishName))
                {
                    LogError($"Fish texture not found for: {fishName}");
                    continue;
                }

                var fishTexture = _fishTextures[fishName];
                var scaledFishTextures = new Dictionary<string, Texture2D>();

                for (int i = 0; i < amountNums.Length; i++)
                {
                    int amount = amountNums[i];
                    if (!_badgeRings.ContainsKey(amount))
                    {
                        LogError($"Badge ring not found for amount: {amount}");
                        continue;
                    }

                    var ringTexture = _badgeRings[amount];
                    var badgeName = fishName + amount;

                    var sizeKey = $"{ringTexture.width}x{ringTexture.height}";
                    if (!scaledFishTextures.TryGetValue(sizeKey, out var scaledFishTexture))
                    {
                        scaledFishTexture = ScaleTexture(fishTexture, ringTexture.width, ringTexture.height);
                        scaledFishTextures.Add(sizeKey, scaledFishTexture);
                    }

                    var combinedTexture = CombineTextures(scaledFishTexture, ringTexture);
                    combinedTexture.name = badgeName;

                    _textures[badgeName] = combinedTexture;
                    Materials[badgeName] = CreateMaterial(combinedTexture);
                }

                foreach (var scaledTexture in scaledFishTextures.Values)
                {
                    UnityEngine.Object.DestroyImmediate(scaledTexture);
                }

                yield return null;
            }

            foreach (string caughtBadge in FishCaughtUI.TotalFishBadgeNames)
            {
                var path = FindAssetPath(caughtBadge + ".png");
                yield return Instance.StartCoroutine(LoadTexture(path, caughtBadge));
            }
        }

        private static Texture2D CombineTextures(Texture2D scaledFishTexture, Texture2D ringTexture)
        {
            var width = ringTexture.width;
            var height = ringTexture.height;

            Texture2D combinedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

            Color[] ringPixels = ringTexture.GetPixels();
            combinedTexture.SetPixels(ringPixels);
            
            Color[] fishPixels = scaledFishTexture.GetPixels();

            // Blend the scaled fish onto the ring
            for (int i = 0; i < ringPixels.Length; i++)
            {
                Color fishColor = fishPixels[i];
                Color ringColor = ringPixels[i];

                if (fishColor.a > 0)
                {
                    ringPixels[i] = Color.Lerp(ringColor, fishColor, fishColor.a);
                }
            }

            combinedTexture.SetPixels(ringPixels);
            combinedTexture.Apply();

            return combinedTexture;
        }

        private static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            var sourceAspect = (float)source.width / source.height;
            var targetAspect = (float)targetWidth / targetHeight;

            int scaledWidth, scaledHeight;

            if (sourceAspect > targetAspect)
            {
                scaledWidth = targetWidth;
                scaledHeight = Mathf.RoundToInt(targetWidth / sourceAspect);
            }
            else
            {
                scaledHeight = targetHeight;
                scaledWidth = Mathf.RoundToInt(targetHeight * sourceAspect);
            }

            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

            Color[] resultPixels = new Color[targetWidth * targetHeight];
            for (int i = 0; i < resultPixels.Length; i++)
            {
                resultPixels[i] = Color.clear;
            }

            var offsetX = (targetWidth - scaledWidth) / 2;
            var offsetY = (targetHeight - scaledHeight) / 2;

            Color[] sourcePixels = source.GetPixels();

            for (int y = 0; y < scaledHeight; y++)
            {
                for (int x = 0; x < scaledWidth; x++)
                {
                    var sourceX = (float)x / scaledWidth * source.width;
                    var sourceY = (float)y / scaledHeight * source.height;

                    var sourceXInt = Mathf.FloorToInt(sourceX);
                    var sourceYInt = Mathf.FloorToInt(sourceY);

                    sourceXInt = Mathf.Clamp(sourceXInt, 0, source.width - 1);
                    sourceYInt = Mathf.Clamp(sourceYInt, 0, source.height - 1);

                    var sourceIndex = sourceYInt * source.width + sourceXInt;
                    var resultIndex = (y + offsetY) * targetWidth + (x + offsetX);

                    if (resultIndex >= 0 && resultIndex < resultPixels.Length)
                    {
                        resultPixels[resultIndex] = sourcePixels[sourceIndex];
                    }
                }
            }

            result.SetPixels(resultPixels);
            result.Apply();

            return result;
        }

        private static IEnumerator LoadPortBadges()
        {
            var textureCoroutines = new List<Coroutine>();

            foreach (string pbName in Region.AllBadgeNames)
            {
                var path = FindAssetPath(pbName + ".png");
                textureCoroutines.Add(Instance.StartCoroutine(LoadTexture(path, pbName)));
            }

            foreach (var coroutine in textureCoroutines)
            {
                yield return coroutine;
            }

            _portBadgesLoaded = true;
            LogDebug("Port badges loaded.");
        }

        private static IEnumerator LoadTexture(string path, string textureName)
        {
            if (!File.Exists(path))
            {
                LogError($"File not found: {path}");
                yield break;
            }

            var texture2D = new Texture2D(1, 1);
            byte[] fileData = null;

            yield return Instance.StartCoroutine(LoadFile(path, result => fileData = result));

            if (fileData != null && fileData.Length > 0)
            {
                bool success = ImageConversion.LoadImage(texture2D, fileData);

                if (!success)
                {
                    LogError($"Failed to load texture from bytes: {path}");
                    yield break;
                }

                _textures[textureName] = texture2D;
                Materials[textureName] = CreateMaterial(texture2D);
            }
        }

        private static IEnumerator LoadFile(string path, Action<byte[]> onComplete)
        {
            byte[] result = null;

            yield return null;

            using (UnityWebRequest www = UnityWebRequest.Get($"file://{path}"))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    LogError(www.error);
                    yield break;
                }

                result = www.downloadHandler.data;
            }

            onComplete(result);
        }

        private static Material CreateMaterial(Texture2D tex)
        {
            var material = new Material(Shader.Find("Standard"))
            {
                renderQueue = 2001,
                mainTexture = tex
            };
            material.EnableKeyword("_ALPHATEST_ON");
            material.SetShaderPassEnabled("ShadowCaster", false);
            return material;
        }
    }
}