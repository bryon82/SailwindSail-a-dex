using BepInEx.Logging;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace sailadex
{
    internal class AssetsLoader
    {
        public static AudioClip notificationSound;
        public static Dictionary<string, Material> materials;
        public static Dictionary<string, Texture2D> textures;

        static readonly ManualLogSource logger = SAD_Plugin.logger;

        public static void Start()
        {
            materials = new Dictionary<string, Material>();
            textures = new Dictionary<string, Texture2D>();

            LoadAudio();
            LoadFishBadges();
            LoadPortBadges();
        }

        private static void LoadAudio()
        {
            var clipPath = Path.Combine(Path.GetDirectoryName(SAD_Plugin.Instance.Info.Location), "assets", "sounds", "twoBells.wav");
            var webRequest = UnityWebRequestMultimedia.GetAudioClip($"file://{clipPath}", AudioType.WAV);

            webRequest.SendWebRequest();

            while (!webRequest.isDone)
                _ = 0;

            if (webRequest.isNetworkError)
            {
                logger.LogError(webRequest.error);
                return;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(webRequest);
            clip.name = "twoBells";
            notificationSound = clip;           

            logger.LogInfo("Audio loaded.");
        }

        private static void LoadFishBadges()
        {
            var fishBadgesPath = Path.Combine(Path.GetDirectoryName(SAD_Plugin.Instance.Info.Location), "assets", "badges", "fish");
            int[] amountNums = { 25, 50, 100 };

            foreach (string fishName in FishCaughtUI.FishNames)
            {
                for (int i = 0; i < 3; i++)
                {
                    var fishBadgeName = fishName + amountNums[i];
                    var texture = LoadTexture(Path.Combine(fishBadgesPath, fishBadgeName + ".png"));
                    textures.Add(fishBadgeName, texture);
                    materials.Add(fishBadgeName, CreateMaterial(texture));
                }
            }

            foreach (string caughtBadge in FishCaughtUI.TotalFishBadgeNames)
            {
                var fishBadgeName = caughtBadge;
                var texture = LoadTexture(Path.Combine(fishBadgesPath, fishBadgeName + ".png"));
                textures.Add(fishBadgeName, texture);
                materials.Add(fishBadgeName, CreateMaterial(texture));                
            }

            logger.LogInfo("Fishing badges loaded.");
        }

        private static void LoadPortBadges()
        {
            var portBadgesPath = Path.Combine(Path.GetDirectoryName(SAD_Plugin.Instance.Info.Location), "assets", "badges", "ports");

            foreach (string pbName in Region.AllBadgeNames)
            {
                var texture = LoadTexture(Path.Combine(portBadgesPath, pbName + ".png"));
                textures.Add(pbName, texture);
                materials.Add(pbName, CreateMaterial(texture));
            }

            logger.LogInfo("Port badges loaded.");
        }

        private static Texture2D LoadTexture(string path)
        {
            var array = File.Exists(path) ? File.ReadAllBytes(path) : null;
            var texture2D = new Texture2D(1, 1);
            if (array == null)
            {
                logger.LogError($"Failed to load {path}");
                return texture2D;
            }
            ImageConversion.LoadImage(texture2D, array);
            return texture2D;
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
