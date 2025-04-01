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
            List<AudioClip> audioClips = new List<AudioClip>();
            GetAudioClip("twoBells", audioClips);
            Debug.Log($"AudioClips length {audioClips.Count}");
            notificationSound = audioClips[0];
        }

        private static void GetAudioClip(string fileName, List<AudioClip> audioClips)
        {
            var clipPath = Path.Combine(Path.GetDirectoryName(Plugin.instance.Info.Location), "assets", "sounds", fileName + ".wav");
            var webRequest = UnityWebRequestMultimedia.GetAudioClip($"file://{clipPath}", AudioType.WAV);

            webRequest.SendWebRequest();

            while (!webRequest.isDone)
                _ = 0;

            if (webRequest.isNetworkError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(webRequest);
                clip.name = fileName;
                audioClips.Add(clip);
            }
        }

        private static void LoadFishBadges()
        {
            var fishBadgesPath = Path.Combine(Path.GetDirectoryName(Plugin.instance.Info.Location), "assets", "badges", "fish");
            int[] amountNums = { 25, 50, 100 };
            Texture2D tempTexture;
            string fishBadgeName;

            foreach (string fishName in Names.fishNames)
            {
                for (int i = 0; i < 3; i++)
                {
                    fishBadgeName = fishName + amountNums[i];
                    tempTexture = LoadTexture(Path.Combine(fishBadgesPath, fishBadgeName + ".png"));
                    textures.Add(fishBadgeName, tempTexture);
                    materials.Add(fishBadgeName, CreateMaterial(tempTexture));
                }
            }

            foreach (string caughtBadge in Names.totalFishBadgeNames)
            {                
                fishBadgeName = caughtBadge;
                tempTexture = LoadTexture(Path.Combine(fishBadgesPath, fishBadgeName + ".png"));
                textures.Add(fishBadgeName, tempTexture);
                materials.Add(fishBadgeName, CreateMaterial(tempTexture));                
            }

            Plugin.logger.LogInfo("Fishing badges loaded.");
        }

        private static void LoadPortBadges()
        {
            var portBadgesPath = Path.Combine(Path.GetDirectoryName(Plugin.instance.Info.Location), "assets", "badges", "ports");
            Texture2D tempTexture;

            foreach (string pbName in Names.portBadgeNames)
            {
                tempTexture = LoadTexture(Path.Combine(portBadgesPath, pbName + ".png"));
                textures.Add(pbName, tempTexture);
                materials.Add(pbName, CreateMaterial(tempTexture));                
            }
            Plugin.logger.LogInfo("Port badges loaded.");
        }

        private static Texture2D LoadTexture(string path)
        {
            byte[] array = File.Exists(path) ? File.ReadAllBytes(path) : null;
            Texture2D texture2D = new Texture2D(1, 1);
            if (array == null)
            {
                Plugin.logger.LogError("Failed to load " + path);
                return texture2D;                
            }
            ImageConversion.LoadImage(texture2D, array);
            return texture2D;
        }

        private static Material CreateMaterial(Texture2D tex)
        {
            Material material = new Material(Shader.Find("Standard"))
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
