using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace sailadex
{
    internal class EventPatches
    {
        static readonly ManualLogSource logger = SAD_Plugin.logger;

        [HarmonyPatch(typeof(FishingRodFish))]
        private class FishingRodFishPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("CollectFish")]
            public static void CollectFishPatch(FishingRodFish __instance, GameObject ___currentFish)
            {
                if (SAD_Plugin.fishCaughtUIEnabled.Value)
                    FishCaughtUI.Instance.RegisterCatch(___currentFish);
            }
        }

        [HarmonyPatch(typeof(IslandMarketWarehouseArea))]
        private class IslandMarketWarehouseAreaPatches
        {

            [HarmonyPostfix]
            [HarmonyPatch("OnTriggerEnter")]
            public static void OnTriggerEnterPatch(IslandMarketWarehouseArea __instance, IslandMarket ___market, Collider other)
            {
                if (SAD_Plugin.portsVisitedUIEnabled.Value && other.CompareTag("Player"))
                    PortsVisitedUI.Instance.RegisterVisit(___market.GetPortName());
                if (SAD_Plugin.statsUIEnabled.Value && other.CompareTag("Player"))
                    StatsUI.Instance.IncrementPortVisited(___market.GetPortName());
                
                    
            }
        }

        [HarmonyPatch(typeof(ShipItem))]
        private class ShipItemPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("EnterBoat")]
            public static void EnterBoatPatch()
            {
                if (SAD_Plugin.statsUIEnabled.Value && GameState.playing)
                    StatsUI.Instance.RegisterCurrentMass();
            }

            [HarmonyPostfix]
            [HarmonyPatch("ExitBoat")]
            public static void ExitBoatPatch()
            {
                if (SAD_Plugin.statsUIEnabled.Value && GameState.playing)
                    StatsUI.Instance.RegisterCurrentMass();
            }
        }

        [HarmonyPatch(typeof(PickupableBoatMooringRope))]
        private class PickupableBoatMooringRopePatches
        {

            [HarmonyPrefix]
            [HarmonyPatch("OnPickup")]
            public static void OnPickupPrePatch(Rigidbody ___boatRigidbody, out string __state)
            {
                __state = ___boatRigidbody.gameObject.GetComponent<BoatMooringRopes>().ropes
                    .Where(r => r.GetPrivateField<SpringJoint>("mooredToSpring") != null)
                    .Select(r => r.GetPrivateField<SpringJoint>("mooredToSpring").transform.parent.name)
                    .FirstOrDefault();
            }

            [HarmonyPostfix]
            [HarmonyPatch("OnPickup")]
            public static void OnPickupPatch(Rigidbody ___boatRigidbody, string __state)
            {
                if (SAD_Plugin.statsUIEnabled.Value &&
                    !GameState.currentlyLoading &&
                    GameState.playing &&
                    !___boatRigidbody.gameObject.GetComponent<BoatMooringRopes>().AnyRopeMoored())
                {
                    logger.LogDebug($"Unmoor from {__state} Day: {GameState.day} Time: {Sun.sun.globalTime}");
                    StatsUI.Instance.RegisterUnderway(__state);
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("MoorTo")]
            public static void MoorToPatch(Rigidbody ___boatRigidbody)
            {
                if (SAD_Plugin.statsUIEnabled.Value &&
                    !GameState.currentlyLoading &&
                    GameState.playing &&
                    (___boatRigidbody.transform == GameState.lastBoat || ___boatRigidbody.transform == GameState.currentBoat?.parent) &&
                    ___boatRigidbody.gameObject.GetComponent<BoatMooringRopes>().AnyRopeMoored())
                {
                    var islandName = ___boatRigidbody.gameObject.GetComponent<BoatMooringRopes>().ropes
                        .Where(r => r.GetPrivateField<SpringJoint>("mooredToSpring") != null)
                        .Select(r => r.GetPrivateField<SpringJoint>("mooredToSpring").transform.parent.name)
                        .FirstOrDefault();
                    logger.LogDebug($"Moored at: {islandName} Day: {GameState.day} Time: {Sun.sun.globalTime} ");
                    StatsUI.Instance.RegisterMoored(islandName);
                    StatsUI.Instance.UpdateMilesText();
                }
            }
        }

        [HarmonyPatch(typeof(ShipItemBottle))]
        private class ShipItemBottlePatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Drink")]
            public static void DrinkPatch(float ___health)
            {
                if (SAD_Plugin.statsUIEnabled.Value && ___health > 0)
                    StatsUI.Instance.IncrementIntStat("DrinksTaken");
            }
        }

        [HarmonyPatch(typeof(ShipItemFood))]
        private class ShipItemFoodPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("EatFood")]
            public static void EatFoodPatch()
            {
                if (SAD_Plugin.statsUIEnabled.Value && !(PlayerNeeds.instance.eatCooldown > 0f))
                    StatsUI.Instance.IncrementIntStat("FoodsEaten");
            }
        }

        [HarmonyPatch(typeof(ShipItemPipe))]
        private class ShipItemPipePatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("StopSmoking")]
            public static void StopSmokingPatch(float ___currentInhaleDuration)
            {
                if (SAD_Plugin.statsUIEnabled.Value && ___currentInhaleDuration > 0f)
                    StatsUI.Instance.IncrementIntStat("TimesSmoked");
            }
        }

        [HarmonyPatch(typeof(Sleep))]
        private class SleepPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("EnterBed")]
            public static void EnterBedPatch()
            {
                if (SAD_Plugin.statsUIEnabled.Value)
                {
                    StatsUI.Instance.IncrementIntStat("TimesSlept");

                    if (SAD_Plugin.updateMilesSailed.Value == "sleep")
                        StatsUI.Instance.UpdateMilesText();
                }
            }
        }

        [HarmonyPatch(typeof(PlayerMissions))]
        private class PlayerMissionsPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("CompleteMission")]
            public static void CompleteMissionPatch()
            {
                if (SAD_Plugin.statsUIEnabled.Value)
                    StatsUI.Instance.IncrementIntStat("MissionsCompleted");
            }
        }

        [HarmonyPatch(typeof(Port))]
        private class  PortPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Update")]
            public static void UpdatePatch(bool ___teleportPlayer)
            {
                if (SAD_Plugin.statsUIEnabled.Value && ___teleportPlayer)
                    StatsUI.Instance.PlayerTeleported();
            }
        }

        [HarmonyPatch(typeof(BoatMass))]
        private class BoatMassPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("FixedUpdate")]
            public static void FixedUpdatePatch()
            {
                if (SAD_Plugin.statsUIEnabled.Value && GameState.currentBoat != null)                
                    StatsUI.Instance.TrackDistance();
                
            }

            [HarmonyPostfix]
            [HarmonyPatch("UpdateMass")]
            public static void UpdateMassPatch(Rigidbody ___body)
            {
                if (SAD_Plugin.statsUIEnabled.Value && GameState.currentBoat != null)
                    StatsUI.Instance.RegisterTotalMass(___body.mass);
            }
        }

        [HarmonyPatch(typeof(WeatherStorms))]
        private class WeatherStormPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("GetNormalizedDistance")]
            public static void GetNormalizedDistancePatch(float __result)
            {
                if (SAD_Plugin.statsUIEnabled.Value &&
                    GameState.currentBoat != null && 
                    !GameState.currentBoat.parent.gameObject.GetComponent<BoatMooringRopes>().AnyRopeMoored() &&
                    __result <= WeatherStorms.instance.GetPrivateField<float>("rainBorder"))
                {
                    StatsUI.Instance.IncrementStormsWeathered();
                }

                if (__result > WeatherStorms.instance.GetPrivateField<float>("cloudyBorder"))
                    StatsUI.Instance.ClearLastStorm();
            }
        }
    }
}
