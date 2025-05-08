using HarmonyLib;
using static sailadex.SAD_Plugin;
using static sailadex.Configs;

namespace sailadex
{
    internal class PassageDude
    {
        public static void PatchMod()
        {
            var ferryTravelClass = AccessTools.TypeByName("FerryTravel");
            var original = AccessTools.Method(ferryTravelClass, "TeleportPlayer");
            var patch = AccessTools.Method(typeof(FerryTravelPatches), "TeleportPlayerPatch");
            HarmonyInstance.Patch(original, new HarmonyMethod(patch));
        } 

        public class FerryTravelPatches
        {
            [HarmonyPostfix]
            public static void TeleportPlayerPatch()
            {
                if (statsUIEnabled.Value)
                    StatsUI.Instance.PlayerTeleported();
            }
        }
    }
}
