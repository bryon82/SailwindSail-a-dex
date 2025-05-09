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
        private static bool _fishBadgesLoaded;
        private static bool _portBadgesLoaded;

        public static void Start()
        {
            Materials = new Dictionary<string, Material>();
            _textures = new Dictionary<string, Texture2D>();

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

            LogInfo("All assets loaded.");
        }

        private static IEnumerator LoadAudio()
        {
            var clipPath = Path.Combine(Path.GetDirectoryName(Instance.Info.Location), "assets", "sounds", "twoBells.wav");
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

                LogInfo("Audio loaded.");
            }
        }

        private static IEnumerator LoadFishBadges()
        {
            var fishBadgesPath = Path.Combine(Path.GetDirectoryName(Instance.Info.Location), "assets", "badges", "fish");
            int[] amountNums = { 25, 50, 100 };

            List<Coroutine> textureCoroutines = new List<Coroutine>();

            foreach (string fishName in FishCaughtUI.FishNames)
            {
                for (int i = 0; i < 3; i++)
                {
                    var fishBadgeName = fishName + amountNums[i];
                    var path = Path.Combine(fishBadgesPath, fishBadgeName + ".png");
                    textureCoroutines.Add(Instance.StartCoroutine(LoadTexture(path, fishBadgeName)));
                }
            }

            foreach (string caughtBadge in FishCaughtUI.TotalFishBadgeNames)
            {
                var fishBadgeName = caughtBadge;
                var path = Path.Combine(fishBadgesPath, fishBadgeName + ".png");
                textureCoroutines.Add(Instance.StartCoroutine(LoadTexture(path, fishBadgeName)));
            }

            foreach (var coroutine in textureCoroutines)
            {
                yield return coroutine;
            }

            _fishBadgesLoaded = true;
            LogInfo("Fishing badges loaded.");
        }

        private static IEnumerator LoadPortBadges()
        {
            var portBadgesPath = Path.Combine(Path.GetDirectoryName(Instance.Info.Location), "assets", "badges", "ports");

            List<Coroutine> textureCoroutines = new List<Coroutine>();

            foreach (string pbName in Region.AllBadgeNames)
            {
                var path = Path.Combine(portBadgesPath, pbName + ".png");
                textureCoroutines.Add(Instance.StartCoroutine(LoadTexture(path, pbName)));
            }

            foreach (var coroutine in textureCoroutines)
            {
                yield return coroutine;
            }

            _portBadgesLoaded = true;
            LogInfo("Port badges loaded.");
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

        private static IEnumerator LoadFile(string path, Action<byte[]> onComplete )
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
