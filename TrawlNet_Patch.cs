using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tweaks
{
    internal class TrawlNet_Patch
    {
        [HarmonyPatch(typeof(TrawlNetAbility))]
        public class TrawlNetAbility_OnEnable_Patch
        {
            static readonly string[] materials = { "scrap", "metal", "cloth", "lumber" };

            [HarmonyPrefix]
            [HarmonyPatch("Update")]
            public static bool UpdatePrefix(TrawlNetAbility __instance)
            {
                if (Config.netCatchChance.Value == 1f && Config.netBreakes.Value)
                    return true;

                if (!__instance.isActive || __instance.trawlNetItemInstance == null || __instance.trawlNetItemInstance.durability <= 0 || !GameManager.Instance.Player.Controller.IsMoving)
                    return false;

                __instance.change = GameManager.Instance.Time.GetTimeChangeThisFrame();
                if (Config.netBreakes.Value)
                {
                    //Util.Log("net durability " + __instance.trawlNetItemInstance.durability);
                    __instance.modifiedChange = __instance.change * (Decimal)(1f - GameManager.Instance.PlayerStats.ResearchedEquipmentMaintenanceModifier);
                    __instance.trawlNetItemInstance.ChangeDurability(-(float)__instance.modifiedChange);
                    //Util.Message("net ChangeDurability " + __instance.modifiedChange);
                    //Util.Log("net ChangeDurability " + __instance.modifiedChange);
                }
                if (__instance.trawlNetItemInstance.durability > 0)
                {
                    __instance.timeUntilNextCatchRoll -= __instance.change;
                    if (__instance.timeUntilNextCatchRoll > 0M)
                        return false;

                    //if (UnityEngine.Random.value < __instance.trawlNetItemData.CatchRate)
                    if (Config.netCatchChance.Value >= UnityEngine.Random.value)
                        __instance.AddTrawlItem();

                    __instance.RefreshTimeUntilNextCatchRoll();
                }
                else
                {
                    __instance.Deactivate();
                    GameEvents.Instance.TriggerItemInventoryChanged(__instance.trawlNetItemInstance.GetItemData<DeployableItemData>());
                }
                return false;
            }

            [HarmonyPostfix]
            [HarmonyPatch("RefreshTimeUntilNextCatchRoll")]
            public static void RefreshTimeUntilNextCatchRollPostfix(TrawlNetAbility __instance)
            {
                //Util.Message(" RefreshTimeUntilNextCatchRoll timeUntilNextCatchRoll " + __instance.timeUntilNextCatchRoll);
                //Util.Log(__instance.trawlNetItemInstance.id + " RefreshTimeUntilNextCatchRoll maxDurabilityDays " + __instance.timeUntilNextCatchRoll);
                __instance.timeUntilNextCatchRoll /= (decimal)Config.netCatchRateMult.Value;
                //Util.Log(__instance.trawlNetItemInstance.id + " RefreshTimeUntilNextCatchRoll maxDurabilityDays my " + __instance.timeUntilNextCatchRoll);
            }

            [HarmonyPrefix]
            [HarmonyPatch("AddTrawlItem", new Type[] { })]
            public static bool AddTrawlItemPrefix(TrawlNetAbility __instance)
            {
                //Util.Message(" RefreshTimeUntilNextCatchRoll timeUntilNextCatchRoll " +    float currentDepth = GameManager.Instance.WaveController.SampleWaterDepthAtPlayerPosition();
                if (Config.netCatchSound.Value && Config.netCatchMaterialChance.Value == 0f)
                    return true;

                float currentDepth = GameManager.Instance.WaveController.SampleWaterDepthAtPlayerPosition();
                List<string> harvestableItemIds = GameManager.Instance.Player.HarvestZoneDetector.GetHarvestableItemIds(new Func<HarvestableItemData, bool>(__instance.CheckCanBeCaughtByThisNet), currentDepth, GameManager.Instance.Time.IsDaytime);
                bool catchMaterial = Config.netCatchMaterialChance.Value >= UnityEngine.Random.value;
                if (harvestableItemIds.Count == 0 && !catchMaterial)
                {
                    //Main.log.LogInfo("[TrawlNetAbility] AddTrawlItem() no nettable items in current zone(s)");
                    return false;
                }
                string id = "";
                if (catchMaterial)
                {
                    int randomIndex = UnityEngine.Random.Range(0, materials.Length);
                    id = materials[randomIndex];
                    foreach (GridCellData gcd in GameManager.Instance.SaveData.TrawlNet.grid)
                        gcd.acceptedItemSubtype |= ItemSubtype.MATERIAL;
                }
                else
                {
                    float[] weights = new float[harvestableItemIds.Count];
                    for (int index = 0; index < harvestableItemIds.Count; ++index)
                        weights[index] = GameManager.Instance.ItemManager.GetItemDataById<HarvestableItemData>(harvestableItemIds[index]).harvestItemWeight;

                    int randomWeightedIndex = MathUtil.GetRandomWeightedIndex(weights);
                    id = harvestableItemIds[randomWeightedIndex];
                }
                HarvestableItemData itemDataById = GameManager.Instance.ItemManager.GetItemDataById<HarvestableItemData>(id);
                if (itemDataById == null)
                    return false;

                Vector3Int foundPosition = new Vector3Int();
                if (!GameManager.Instance.SaveData.TrawlNet.FindPositionForObject(itemDataById, out foundPosition))
                    return false;

                if (catchMaterial)
                {
                    SpatialItemInstance spatialItemInstance = GameManager.Instance.ItemManager.CreateItem<SpatialItemInstance>(id);
                    GameManager.Instance.SaveData.TrawlNet.AddObjectToGridData(spatialItemInstance, foundPosition, true);
                    GameManager.Instance.ItemManager.SetItemSeen(spatialItemInstance);
                }
                else
                {
                    FishSizeGenerationMode sizeGenerationMode = FishSizeGenerationMode.NO_BIG_TROPHY;
                    float aberrationBonusMultiplier = 1f;
                    if (!itemDataById.canBeCaughtByPot && !itemDataById.canBeCaughtByRod)
                    {
                        sizeGenerationMode = FishSizeGenerationMode.ANY;
                        aberrationBonusMultiplier = 2f;
                    }
                    FishItemInstance fishItem = GameManager.Instance.ItemManager.CreateFishItem(id, FishAberrationGenerationMode.RANDOM_CHANCE, false, sizeGenerationMode, aberrationBonusMultiplier);
                    GameManager.Instance.SaveData.TrawlNet.AddObjectToGridData(fishItem, foundPosition, true);
                    GameManager.Instance.ItemManager.SetItemSeen(fishItem);
                }
                //Util.Log(" net Item size " + itemDataById.GetSize());
                //if (Config.netDurabilityLossPerCatch.Value > 0)
                //{
                //    Util.Log("durability " + __instance.trawlNetItemInstance.durability);
                //    float durChange = Config.netDurabilityLossPerCatch.Value * 1f;
                //    __instance.trawlNetItemInstance.ChangeDurability(-durChange);
                //    Util.Log("durability after " + __instance.trawlNetItemInstance.durability);
                //}
                ++GameManager.Instance.SaveData.NetFishCaught;
                GameEvents.Instance.TriggerFishCaught();
                if (Config.netCatchSound.Value)
                    GameManager.Instance.AudioPlayer.PlaySFX(__instance.catchSFX, AudioLayer.SFX_PLAYER, __instance.catchSFXVolume);
                
                return false;
            }
        }

        [HarmonyPatch(typeof(NetTabUI), "OnEnable")]
        public class NetTabUI_OnEnable_Patch
        {
            public static void Postfix(NetTabUI __instance)
            {
                if (!Config.showNetCatchCount.Value)
                    __instance.itemCounterUI.gameObject.SetActive(false);

            }
        }

        [HarmonyPatch(typeof(TrawlActiveTab))]
        class TrawlActiveTab_xPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("Toggle")]
            public static bool TogglePrefix(TrawlActiveTab __instance)
            {
                //Util.Message("TrawlActiveTab Toggle");
                return Config.showNetCatchCount.Value;
            }
            [HarmonyPostfix]
            [HarmonyPatch("OnEnable")]
            public static void OnEnablePrefix(TrawlActiveTab __instance)
            {
                //Util.Log("TrawlActiveTab OnEnable");
                if (!Config.showNetCatchCount.Value)
                    __instance.container.gameObject.SetActive(false);
            }
        }




    }
}
