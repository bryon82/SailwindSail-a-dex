using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace sailadex
{
    internal class EventPatches
    {
        [HarmonyPatch(typeof(FishingRodFish))]
        private class FishingRodFishPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("CollectFish")]
            public static void CollectFishPatch(FishingRodFish __instance, GameObject ___currentFish)
            {
                if (Plugin.fishCaughtUIEnabled.Value)
                    FishCaughtUI.instance.RegisterCatch(___currentFish);
            }
        }

        [HarmonyPatch(typeof(IslandMarketWarehouseArea))]
        private class IslandMarketWarehouseAreaPatches
        {

            [HarmonyPostfix]
            [HarmonyPatch("OnTriggerEnter")]
            public static void OnTriggerEnterPatch(IslandMarketWarehouseArea __instance, IslandMarket ___market, Collider other)
            {
                if (Plugin.portsVisitedUIEnabled.Value && other.CompareTag("Player"))
                    PortsVisitedUI.instance.RegisterVisit(___market.GetPortName());
                if (Plugin.statsUIEnabled.Value && other.CompareTag("Player"))
                    StatsUI.instance.IncrementPortVisited(___market.GetPortName());
                
                    
            }
        }

        //[HarmonyPatch(typeof(PlayerEmbarkDisembarkTrigger))]
        //private class PlayerEmbarkDisembarkTriggerPatches
        //{
        //    [HarmonyPostfix]
        //    [HarmonyPatch("EnterBoat")]
        //    public static void EnterBoatPatch()
        //    {
        //        //Debug.Log("Embarking boat");
        //    }
        //}

        [HarmonyPatch(typeof(ShipItem))]
        private class ShipItemPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("EnterBoat")]
            public static void EnterBoatPatch()
            {
                if (Plugin.statsUIEnabled.Value && GameState.playing)
                    StatsUI.instance.RegisterCurrentMass();
            }

            [HarmonyPostfix]
            [HarmonyPatch("ExitBoat")]
            public static void ExitBoatPatch()
            {
                if (Plugin.statsUIEnabled.Value && GameState.playing)
                    StatsUI.instance.RegisterCurrentMass();
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
                if (Plugin.statsUIEnabled.Value 
                    && !GameState.currentlyLoading 
                    && GameState.playing
                    && !___boatRigidbody.gameObject.GetComponent<BoatMooringRopes>().AnyRopeMoored())
                {
                    Plugin.logger.LogDebug($"Unmoor from {__state} Day: {GameState.day} Time: {Sun.sun.globalTime}");
                    StatsUI.instance.RegisterUnderway(__state);
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("MoorTo")]
            public static void MoorToPatch(Rigidbody ___boatRigidbody)
            {
                if (Plugin.statsUIEnabled.Value 
                    && !GameState.currentlyLoading 
                    && GameState.playing
                    && (___boatRigidbody.transform == GameState.lastBoat || ___boatRigidbody.transform == GameState.currentBoat?.parent)
                    && ___boatRigidbody.gameObject.GetComponent<BoatMooringRopes>().AnyRopeMoored())
                {
                    var islandName = ___boatRigidbody.gameObject.GetComponent<BoatMooringRopes>().ropes
                        .Where(r => r.GetPrivateField<SpringJoint>("mooredToSpring") != null)
                        .Select(r => r.GetPrivateField<SpringJoint>("mooredToSpring").transform.parent.name)
                        .FirstOrDefault();
                    Plugin.logger.LogDebug($"Moored at: {islandName} Day: {GameState.day} Time: {Sun.sun.globalTime} ");
                    StatsUI.instance.RegisterMoored(islandName);
                    StatsUI.instance.UpdateMilesText();
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
                if (Plugin.statsUIEnabled.Value && ___health > 0)
                    StatsUI.instance.IncrementIntStat("DrinksTaken");
            }
        }

        [HarmonyPatch(typeof(ShipItemFood))]
        private class ShipItemFoodPatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("EatFood")]
            public static void EatFoodPatch()
            {
                if (Plugin.statsUIEnabled.Value && !(PlayerNeeds.instance.eatCooldown > 0f))
                    StatsUI.instance.IncrementIntStat("FoodsEaten");
            }
        }

        [HarmonyPatch(typeof(ShipItemPipe))]
        private class ShipItemPipePatches
        {
            [HarmonyPrefix]
            [HarmonyPatch("StopSmoking")]
            public static void StopSmokingPatch(float ___currentInhaleDuration)
            {
                if (Plugin.statsUIEnabled.Value && ___currentInhaleDuration > 0f)
                    StatsUI.instance.IncrementIntStat("TimesSmoked");
            }
        }

        [HarmonyPatch(typeof(Sleep))]
        private class SleepPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("EnterBed")]
            public static void EnterBedPatch()
            {
                if (Plugin.statsUIEnabled.Value)
                {
                    StatsUI.instance.IncrementIntStat("TimesSlept");

                    if (Plugin.updateMilesSailed.Value == "sleep")
                        StatsUI.instance.UpdateMilesText();
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
                if (Plugin.statsUIEnabled.Value)
                    StatsUI.instance.IncrementIntStat("MissionsCompleted");
            }
        }

        [HarmonyPatch(typeof(Port))]
        private class  PortPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("Update")]
            public static void UpdatePatch(bool ___teleportPlayer)
            {
                if (Plugin.statsUIEnabled.Value && ___teleportPlayer)
                    StatsUI.instance.PlayerTeleported();
            }
        }

        [HarmonyPatch(typeof(BoatMass))]
        private class BoatMassPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("FixedUpdate")]
            public static void FixedUpdatePatch()
            {
                if (Plugin.statsUIEnabled.Value && GameState.currentBoat != null)                
                    StatsUI.instance.TrackDistance();
                
            }

            [HarmonyPostfix]
            [HarmonyPatch("UpdateMass")]
            public static void UpdateMassPatch(Rigidbody ___body)
            {
                if (Plugin.statsUIEnabled.Value && GameState.currentBoat != null)
                    StatsUI.instance.RegisterTotalMass(___body.mass);
            }
        }

        [HarmonyPatch(typeof(WeatherStorms))]
        private class WeatherStormPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch("GetNormalizedDistance")]
            public static void GetNormalizedDistancePatch(float __result)
            {
                if (Plugin.statsUIEnabled.Value 
                    && GameState.currentBoat != null
                    && !GameState.currentBoat.parent.gameObject.GetComponent<BoatMooringRopes>().AnyRopeMoored()
                    && __result <= WeatherStorms.instance.GetPrivateField<float>("rainBorder"))
                {
                    StatsUI.instance.IncrementStormsWeathered();
                }

                if (__result > WeatherStorms.instance.GetPrivateField<float>("cloudyBorder"))
                    StatsUI.instance.ClearLastStorm();
            }
        }
    }
}
