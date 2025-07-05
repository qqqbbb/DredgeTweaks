using FluffyUnderware.DevTools.Extensions;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tweaks
{
    public class HarvestPatch
    {
        //public static Dictionary<string, float> depthAv = new();
        //public static Dictionary<HarvestableType, float> depthMax = new();
        //public static SortedList<string, float> dredges = new();
        //public static List<float> dredges = new();

        [HarmonyPatch(typeof(HarvestMinigameView))]
        public class HarvestMinigameView_Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch("RefreshHarvestTarget")]
            public static void RefreshHarvestTargetPostfix(HarvestMinigameView __instance)
            {
                if (!Config.showFishSpotInfo.Value)
                {
                    __instance.hintImage.color = Color.black;
                    __instance.stockText.text = "";
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch("SpawnItem")]
            public static bool SpawnItemPrefix(HarvestMinigameView __instance, bool isTrophy)
            {
                //Util.Message(" SpawnItem " + __instance.itemDataToHarvest.id);
                //Main.log.LogDebug(" SpawnItem ResearchItemDredgeSpotSpawnChance " + GameManager.Instance.GameConfigData.ResearchItemDredgeSpotSpawnChance + " ");
                bool deductFromStock = true;
                SpatialItemInstance spatialItemInstance1 = null;
                if (__instance.itemDataToHarvest != null)
                {
                    if (__instance.itemDataToHarvest.itemSubtype == ItemSubtype.FISH)
                    {
                        FishAberrationGenerationMode aberrationGenerationMode = FishAberrationGenerationMode.RANDOM_CHANCE;
                        if (__instance.currentPOI.IsCurrentlySpecial && __instance.currentPOI.Stock < 2f)
                            aberrationGenerationMode = FishAberrationGenerationMode.FORCE;

                        spatialItemInstance1 = GameManager.Instance.ItemManager.CreateFishItem(__instance.itemDataToHarvest.id, aberrationGenerationMode, __instance.currentPOI.IsCurrentlySpecial, isTrophy ? FishSizeGenerationMode.FORCE_BIG_TROPHY : FishSizeGenerationMode.NO_BIG_TROPHY);
                        deductFromStock = !__instance.itemDataToHarvest.affectedByFishingSustain || UnityEngine.Random.value > GameManager.Instance.PlayerStats.ResearchedFishingSustainModifier;
                        if (spatialItemInstance1.GetItemData<FishItemData>().IsAberration)
                            __instance.currentPOI.SetIsCurrentlySpecial(false);

                        --GameManager.Instance.SaveData.FishUntilNextTrophyNotch;
                        ++GameManager.Instance.SaveData.RodFishCaught;
                    }
                    else if (__instance.itemDataToHarvest.canBeReplacedWithResearchItem && Config.chanceToCatchResearchPart.Value >= UnityEngine.Random.value)
                    {
                        SpatialItemInstance spatialItemInstance2 = new SpatialItemInstance();
                        spatialItemInstance2.id = GameManager.Instance.ResearchHelper.ResearchItemData.id;
                        spatialItemInstance1 = spatialItemInstance2;
                        deductFromStock = false;
                    }
                    else
                    {
                        SpatialItemInstance spatialItemInstance3 = new SpatialItemInstance();
                        spatialItemInstance3.id = __instance.itemDataToHarvest.id;
                        spatialItemInstance1 = spatialItemInstance3;
                    }
                }
                if (__instance.itemDataToHarvest.itemSubtype == ItemSubtype.FISH && Config.fishingSpots.Value == FishingSpots.NeverDeplete)
                    deductFromStock = false;

                if (__instance.currentPOI.IsDredgePOI && Config.dredgeSpots.Value == DredgeSpots.NeverDeplete)
                    deductFromStock = false;

                __instance.currentPOI.OnHarvested(deductFromStock);
                GameManager.Instance.GridManager.AddItemOfTypeToCursor(spatialItemInstance1, GridObjectState.BEING_HARVESTED);
                GameManager.Instance.ItemManager.SetItemSeen(spatialItemInstance1);
                GameEvents.Instance.TriggerFishCaught();
                __instance.itemDataToHarvest = null;
                return false;
            }

        }

        [HarmonyPatch(typeof(HarvestableParticles))]
        public class HarvestableParticles_Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnEnable")]
            public static void OnEnablePrefix(HarvestableParticles __instance)
            {
                if (!Config.waterRipplesOnFishingSpot.Value)
                {
                    Transform disturbedWaterParticles = __instance.transform.Find("DisturbedWaterParticles");
                    if (disturbedWaterParticles)
                    {
                        //Main.log.LogDebug("DisturbedWaterParticles");
                        disturbedWaterParticles.gameObject.SetActive(false);
                    }
                    PlacedHarvestPOI placedHarvestPOI = __instance.GetComponentInParent<PlacedHarvestPOI>();
                    if (placedHarvestPOI != null && placedHarvestPOI.IsCrabPotPOI)
                    {
                        __instance.gameObject.SetActive(false);
                        //Main.log.LogInfo(placedHarvestPOI.name + " GetHarvestableItemSubType " + placedHarvestPOI.harvestable.GetHarvestableItemSubType());
                        //Main.log.LogInfo(placedHarvestPOI.name + " GetHarvestType " + placedHarvestPOI.harvestable.GetHarvestType());
                    }
                }
                if (!Config.showOrangeParticlesOnPOI.Value)
                {
                    Transform particles = __instance.transform.Find("Embers");
                    if (particles)
                        particles.gameObject.SetActive(false);
                }
                if (!Config.showRelicParticles.Value && __instance.name == "RelicParticles(Clone)")
                {
                    Transform particles = __instance.transform.Find("Beam");
                    if (particles)
                        particles.gameObject.SetActive(false);
                }
            }
            [HarmonyPrefix]
            [HarmonyPatch("SetSpecialStatus")]
            public static void SetSpecialStatusPrefix(HarvestableParticles __instance, ref bool isSpecial)
            {
                if (isSpecial)
                    isSpecial = Config.specialFishingSpots.Value && Config.aberrationParticleFXonFishingSpot.Value;
            }
        }

        [HarmonyPatch(typeof(HarvestPOI))]
        public class HarvestPOI_Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnDayNightChanged")]
            public static void OnDayNightChangedPostfix(HarvestPOI __instance)
            {
                if (__instance.harvestable == null)
                    return;

                if (Config.randomizeFishStock.Value && Util.IsfishingSpot(__instance))
                {
                    RandomizeStock(__instance);
                }
                if (Config.specialFishingSpots.Value == false)
                {
                    __instance.shouldUpdateSpecialParticles = __instance.isCurrentlySpecial;
                    __instance.isCurrentlySpecial = false;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("OnEnable")]
            public static void OnEnablePostfix(HarvestPOI __instance)
            {
                if (!GameManager.Instance.IsPlaying)
                {
                    return;
                }
                //if (Config.randomDredgeSpotSpawnRate.Value > 0 && __instance.IsDredgePOI)
                //{
                //    RandomFishSpotManager.AddToDredgePool(__instance);
                //}
                if (!Util.IsfishingSpot(__instance))
                    return;

                //if (Config.randomFishSpotSpawnRate.Value > 0)
                {
                    //if (!__instance.gameObject.GetComponent<RandomFishSpot>())
                    {
                        //if (Input.GetKey(KeyCode.LeftShift))
                        //    Util.Message(__instance.name + " disable");
                        //__instance.gameObject.SetActive(false);
                    }
                    //RandomFishSpotManager.AddToFishPool(__instance);
                    //if (!RandomFishSpotManager.placingFishSpots)
                    //{
                    //    RandomFishSpotManager.placingFishSpots = true;
                    //    GameManager.Instance.StartCoroutine(RandomFishSpotManager.PlaceFishSpots());
                    //}
                    //string id = __instance.harvestPOIData.id;
                    //GameManager.Instance.SaveData.harvestSpotStocks[id] = 0;
                }
                //if (Config.randomFishSpotSpawnRate.Value == 0)
                //{
                //    RandomFishSpotManager.placingFishSpots = false;
                //    GameManager.Instance.StopCoroutine(RandomFishSpotManager.PlaceFishSpots());
                //}
                if (Config.fishingSpotDisableChance.Value > 0)
                    __instance.gameObject.EnsureComponent<HarvestPOIdisabler>();
                else
                {
                    HarvestPOIdisabler harvestPOIdisabler = __instance.GetComponent<HarvestPOIdisabler>();
                    if (harvestPOIdisabler)
                        UnityEngine.Object.Destroy(harvestPOIdisabler);
                }
            }

        }

        [HarmonyPatch(typeof(ItemManager), "CreateFishItem")]
        public class ItemManager_CreateFishItem_Patch
        {
            public static bool Prefix(ItemManager __instance, ref string itemId, FishAberrationGenerationMode aberrationGenerationMode, bool isSpecialSpot, FishSizeGenerationMode sizeGenerationMode, ref FishItemInstance __result, float aberrationBonusMultiplier = 1f)
            { // BaseAberrationSpawnChance 0.01 NightAberrationSpawnChance 0.03
              // TotalAberrationCatchModifier (gear)
              // MaxAberrationSpawnChance 0.35 SpecialSpotAberrationSpawnBonus 0.35
              //Util.Log("ItemManager CreateFishItem " + itemId + " aberrationGenerationMode " + aberrationGenerationMode);
                if (Config.aberrationCatchBonusCap.Value == .35f && Config.daytimeAberrationChance.Value == .01f && Config.nighttimeAberrationChance.Value == .03f && Config.sanityAberrationCatchBonus.Value == 0)
                {
                    return true;
                }
                if (aberrationGenerationMode == FishAberrationGenerationMode.RANDOM_CHANCE || aberrationGenerationMode == FishAberrationGenerationMode.FORCE)
                {
                    bool spawnedAber = false;
                    FishItemData itemDataById = __instance.GetItemDataById<FishItemData>(itemId);
                    if (itemDataById && GameManager.Instance.SaveData.CanCatchAberrations && itemDataById.Aberrations.Count > 0 && GameManager.Instance.SaveData.GetCaughtCountById(itemDataById.id) > 0)
                    {
                        float aberChance = 0f;
                        if (aberrationGenerationMode == FishAberrationGenerationMode.RANDOM_CHANCE)
                        {
                            float aberTimeOfDayChance = GameManager.Instance.Time.IsDaytime ? Config.daytimeAberrationChance.Value : Config.nighttimeAberrationChance.Value;
                            float aberrationCatchModifier = GameManager.Instance.PlayerStats.TotalAberrationCatchModifier;
                            float num3 = Mathf.Min(Config.aberrationCatchBonusCap.Value, aberTimeOfDayChance + (float)GameManager.Instance.SaveData.AberrationSpawnModifier + aberrationCatchModifier);
                            float specialSpotAberBonus = 0f;
                            if (isSpecialSpot)
                                specialSpotAberBonus = GameManager.Instance.SaveData.HasCaughtAberrationAtSpecialSpot ? GameManager.Instance.GameConfigData.SpecialSpotAberrationSpawnBonus : 1f;

                            float sanityBonus = 1 - GameManager.Instance.Player.Sanity.CurrentSanity;
                            sanityBonus *= Config.sanityAberrationCatchBonus.Value;
                            //Util.Log("Sanity " + GameManager.Instance.Player.Sanity.CurrentSanity + " sanityBonus " + sanityBonus);
                            aberChance = num3 * aberrationBonusMultiplier + specialSpotAberBonus;
                            aberChance += aberChance * sanityBonus;
                            Debug.Log(string.Format("[ItemManager] aberration spawn chance is {0} (time of day chance: {1} + player chance: {2} + bonus chance {3} + gear chance {4}. Modifier was {5}x)", aberChance, aberTimeOfDayChance, GameManager.Instance.SaveData.AberrationSpawnModifier, specialSpotAberBonus, aberrationCatchModifier, aberrationBonusMultiplier));
                        }
                        if (aberrationGenerationMode == FishAberrationGenerationMode.FORCE || UnityEngine.Random.value < aberChance)
                        {
                            int worldPhase = GameManager.Instance.SaveData.WorldPhase;
                            List<FishItemData> candidates = new List<FishItemData>();
                            itemDataById.Aberrations.ForEach(aberrationItemData =>
                            {
                                if (worldPhase >= aberrationItemData.MinWorldPhaseRequired && GameManager.Instance.SaveData.GetCaughtCountById(aberrationItemData.id) == 0)
                                {
                                    candidates.Add(aberrationItemData);
                                }
                            });
                            if (candidates.Count == 0)
                                itemDataById.Aberrations.ForEach(delegate (FishItemData aberrationItemData)
                                {
                                    if (worldPhase >= aberrationItemData.MinWorldPhaseRequired)
                                        candidates.Add(aberrationItemData);
                                });
                            if (candidates.Count > 0)
                            {
                                FishItemData fishItemData = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                                if (fishItemData)
                                {
                                    itemId = fishItemData.id;
                                    spawnedAber = true;
                                }
                            }
                        }
                    }
                    if (spawnedAber)
                    {
                        GameManager.Instance.SaveData.AberrationSpawnModifier = 0M;
                        if (isSpecialSpot)
                            GameManager.Instance.SaveData.HasCaughtAberrationAtSpecialSpot = true;

                        ++GameManager.Instance.SaveData.NumAberrationsCaught;
                    }
                    else
                        GameManager.Instance.SaveData.AberrationSpawnModifier += GameManager.Instance.GameConfigData.SpawnChanceIncreasePerNonAberrationCaught;
                }
                float num = 0.5f;
                if ((double)__instance.DebugNextFishSize == -1.0)
                {
                    switch (sizeGenerationMode)
                    {
                        case FishSizeGenerationMode.ANY:
                            num = MathUtil.GetRandomGaussian();
                            break;
                        case FishSizeGenerationMode.NO_BIG_TROPHY:
                            num = MathUtil.GetRandomGaussian(maxValue: GameManager.Instance.GameConfigData.TrophyMaxSize - 0.01f);
                            break;
                        case FishSizeGenerationMode.FORCE_BIG_TROPHY:
                            num = UnityEngine.Random.Range(GameManager.Instance.GameConfigData.TrophyMaxSize, 1f);
                            break;
                    }
                }
                else
                    num = __instance.DebugNextFishSize;

                FishItemInstance fishItem = new FishItemInstance();
                fishItem.id = itemId;
                fishItem.size = num;
                fishItem.freshness = GameManager.Instance.GameConfigData.MaxFreshness;
                __result = fishItem;
                return false;
            }
        }

        private static void RandomizeStock(HarvestPOI harvestPOI)
        { // does not work for dredge spot
            //Util.Log(" RandomizeStock " + harvestPOI.name);
            //Util.Message(" RandomizeStock " + harvestPOI.name);
            int maxStock = (int)harvestPOI.harvestable.GetMaxStock();
            if (maxStock == 1)
                return;

            string id = harvestPOI.harvestPOIData.id;
            int randomStock = UnityEngine.Random.Range(1, maxStock + 1);
            GameManager.Instance.SaveData.harvestSpotStocks[id] = randomStock;
            //harvestPOI.harvestPOIData.startStock = randomStock;
            //Util.Log("RandomizeStock " + harvestPOI.name + " maxStock " + maxStock + " randomStock " + randomStock);
        }

    }

    [HarmonyPatch(typeof(FishMinigame), "StartGame")]
    class FishMinigame_StartGame_Patch
    {
        public static void Postfix(FishMinigame __instance)
        {
            //Util.Message("StartGame");
            if (!Config.showMinigameAnimationFeedback.Value)
                __instance.feedbackAnimationController = null;
        }
    }

    [HarmonyPatch(typeof(BallCatcherMinigame), "StartGame")]
    class BallCatcherMinigame_StartGame_Patch
    {
        public static void Postfix(BallCatcherMinigame __instance)
        {
            if (!Config.showMinigameAnimationFeedback.Value)
                __instance.feedbackAnimationController = null;
        }
    }

    [HarmonyPatch(typeof(DredgeMinigame), "StartGame")]
    class DredgeMinigame_StartGame_Patch
    {
        public static void Postfix(DredgeMinigame __instance)
        {
            if (!Config.showMinigameAnimationFeedback.Value)
                __instance.feedbackAnimationController = null;
        }
    }

    [HarmonyPatch(typeof(PendulumMinigame), "StartGame")]
    class PendulumMinigame_StartGame_Patch
    {
        public static void Postfix(PendulumMinigame __instance)
        {
            if (!Config.showMinigameAnimationFeedback.Value)
                __instance.feedbackAnimationController = null;
        }
    }

    [HarmonyPatch(typeof(FreshnessCoroutine), "AdjustItemFreshness")]
    class FreshnessCoroutine_AdjustItemFreshness_Patch
    {
        public static bool Prefix(FreshnessCoroutine __instance)
        {
            return Config.fishDecays.Value;
        }
    }

    [HarmonyPatch(typeof(HarvestPOIDataModel))]
    public class HarvestPOIDataModel_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("GetDoesRestock")]
        public static void GetDoesRestockPostfix(HarvestPOIDataModel __instance, ref bool __result)
        {
            HarvestableType harvestType = __instance.GetHarvestType();
            if (Config.dredgeSpots.Value == DredgeSpots.NeverRestock && harvestType == HarvestableType.DREDGE)
            {
                __result = false;
            }
            else if (Config.fishingSpots.Value == FishingSpots.NeverRestock)
            {
                if (harvestType == HarvestableType.ABYSSAL || harvestType == HarvestableType.COASTAL || harvestType == HarvestableType.HADAL || harvestType == HarvestableType.ICE || harvestType == HarvestableType.MANGROVE || harvestType == HarvestableType.OCEANIC || harvestType == HarvestableType.SHALLOW || harvestType == HarvestableType.VOLCANIC)
                {
                    __result = false;
                }
            }
            //Util.Log(harvestType + " GetDoesRestock " + __result);
        }
        //[HarmonyPostfix]
        //[HarmonyPatch("GetNighttimeSpecialChance")]
        public static void GetNighttimeSpecialChancePostfix(HarvestPOIDataModel __instance, ref float __result)
        {
            if (Mathf.Approximately(__result, .1f))
                __result = Config.nighttimeAberrationChance.Value;
        }
        //[HarmonyPostfix]
        //[HarmonyPatch("GetDaytimeSpecialChance")]
        public static void GetDaytimeSpecialChancePostfix(HarvestPOIDataModel __instance, ref float __result)
        {
            if (Mathf.Approximately(__result, .025f))
                __result = Config.daytimeAberrationChance.Value;
        }

    }



}
