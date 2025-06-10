using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tweaks
{
    internal class CrabPot_Patch
    {

        [HarmonyPatch(typeof(SerializedCrabPotPOIData))]
        public class SerializedCrabPotPOIData_Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch("Init", new Type[] { })]
            public static void InitPostfix(SerializedCrabPotPOIData __instance)
            {
                //Util.Log(__instance.deployableItemId + " Init crabPotCatchRateMult " + Config.crabPotCatchRateMult.Value);
                //Util.Log(__instance.deployableItemId + " Init timeUntilNextCatchRoll " + __instance.timeUntilNextCatchRoll);
                __instance.timeUntilNextCatchRoll *= Config.crabPotCatchRateMult.Value;
                //Util.Log(__instance.deployableItemId + " Init timeUntilNextCatchRoll my " + __instance.timeUntilNextCatchRoll);
            }

            [HarmonyPrefix]
            [HarmonyPatch("CalculateCatchRoll")]
            public static bool CalculateCatchRollPrefix(SerializedCrabPotPOIData __instance, ref float gameTimeElapsed, ref bool __result)
            {
                if (Config.crabPotCatchChance.Value == 1f && Config.crabPotCatchRateMult.Value == 1f)
                    return true;

                bool catchRoll = false;
                float num = Mathf.Min(gameTimeElapsed, __instance.deployableItemData.TimeBetweenCatchRolls * Config.crabPotCatchRateMult.Value);
                gameTimeElapsed -= num;
                //Util.Log(__instance.deployableItemId + " CalculateCatchRoll timeUntilNextCatchRoll 1 " + __instance.timeUntilNextCatchRoll);

                __instance.timeUntilNextCatchRoll -= num;
                if (__instance.timeUntilNextCatchRoll <= 0 && __instance.durability > 0)
                {
                    if (UnityEngine.Random.value < Config.crabPotCatchChance.Value)
                    {
                        MathUtil.GetRandomWeightedIndex(__instance.GetItemWeights());
                        HarvestableItemData harvestableItemData = __instance.GetRandomHarvestableItem();
                        if (harvestableItemData == null)
                            return false;

                        if (harvestableItemData.canBeReplacedWithResearchItem && UnityEngine.Random.value < GameManager.Instance.GameConfigData.ResearchItemCrabPotSpawnChance)
                            harvestableItemData = GameManager.Instance.ResearchHelper.ResearchItemData;
                        Vector3Int foundPosition;

                        if (__instance.grid.FindPositionForObject(harvestableItemData, out foundPosition))
                        {
                            //Util.Log(__instance.deployableItemId + " CalculateCatchRoll crabPotCatchRateMult " + Config.crabPotCatchRateMult.Value);
                            SpatialItemInstance spatialItemInstance1;
                            if (harvestableItemData.itemSubtype == ItemSubtype.FISH)
                            {
                                spatialItemInstance1 = GameManager.Instance.ItemManager.CreateFishItem(harvestableItemData.id, FishAberrationGenerationMode.RANDOM_CHANCE, false, FishSizeGenerationMode.ANY, 1f + __instance.deployableItemData.aberrationBonus);
                            }
                            else
                            {
                                SpatialItemInstance spatialItemInstance2 = new SpatialItemInstance();
                                spatialItemInstance2.id = harvestableItemData.id;
                                spatialItemInstance1 = spatialItemInstance2;
                            }
                            __instance.grid.AddObjectToGridData(spatialItemInstance1, foundPosition, false);
                            catchRoll = true;
                        }
                    }
                    __instance.timeUntilNextCatchRoll = __instance.deployableItemData.TimeBetweenCatchRolls * Config.crabPotCatchRateMult.Value;
                    //Util.Log(__instance.deployableItemId + " CalculateCatchRoll timeUntilNextCatchRoll 2 " + __instance.timeUntilNextCatchRoll);
                }
                if (gameTimeElapsed > 0)
                    catchRoll = __instance.CalculateCatchRoll(gameTimeElapsed) | catchRoll;

                __result = catchRoll;
                return false;
            }

            [HarmonyPrefix]
            [HarmonyPatch("AdjustDurability")]
            public static bool AdjustDurabilityPrefix(SerializedCrabPotPOIData __instance, ref bool __result, float newGameTime)
            {
                //Util.Log("SerializedCrabPotPOIData AdjustDurability " + __instance.deployableItemData.id);
                if (Config.crabPotDurabilityMultiplier.Value == 1)
                    return true;

                __instance.hadDurabilityRemaining = __instance.durability > 0.0;
                float num = newGameTime - __instance.lastUpdate;
                __instance.lastUpdate = newGameTime;
                __instance.durability -= num * (1f - GameManager.Instance.PlayerStats.ResearchedEquipmentMaintenanceModifier) / Config.crabPotDurabilityMultiplier.Value;
                __instance.durability = Mathf.Clamp(__instance.durability, 0f, __instance.deployableItemData.MaxDurabilityDays);
                __result = __instance.durability <= 0 && __instance.hadDurabilityRemaining;
                return false;
            }


        }


    }
}
