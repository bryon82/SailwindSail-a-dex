using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;

namespace sailadex
{
    internal class PassageDude
    {
        public static void PatchMod()
        {
            Type ferryTravelClass = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(type => type.IsClass && type.Name == "FerryTravel")
                .Single();
            MethodInfo original = AccessTools.Method(ferryTravelClass, "TeleportPlayer");
            MethodInfo patch = AccessTools.Method(typeof(FerryTravelPatches), "TeleportPlayerPatch");
            SAD_Plugin.harmony.Patch(original, new HarmonyMethod(patch));
        } 

        public class FerryTravelPatches
        {
            [HarmonyPostfix]
            public static void TeleportPlayerPatch()
            {
                if (SAD_Plugin.statsUIEnabled.Value)
                    StatsUI.Instance.PlayerTeleported();
            }
        }
    }
}
