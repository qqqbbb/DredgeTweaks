using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinemachine;
using FluffyUnderware.DevTools.Extensions;
using UnityEngine.ResourceManagement.AsyncOperations;
using HarmonyLib;
using InControl;
using UnityEngine;
using UnityEngine.Localization.Settings;
using static UnityEngine.ParticleSystem.PlaybackState;
using CommandTerminal;
using Yarn.Unity;
using Yarn;
using static Yarn.VirtualMachine;
using System.Reflection;
using System.Diagnostics.Eventing.Reader;
using UnityEngine.EventSystems;
using static TooltipUI;
using UnityEngine.Localization.Tables;
using System.Collections;
using DG.Tweening;
using UnityEngine.Localization;
using UnityEngine.UIElements;
using FluffyUnderware.Curvy.ThirdParty.LibTessDotNet;

namespace Tweaks
{
    public class Testing
    {
        //List<SpatialItemInstance> allItemsOfType = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.ROD);
        //GameManager.Instance.ItemManager.SetItemSeen(currentlyFocusedObject.SpatialItemInstance);

        //flag = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.ROD).Any<SpatialItemInstance>((Func<SpatialItemInstance, bool>) (r => !r.GetIsOnDamagedCell() && ((IEnumerable<HarvestableType>) r.GetItemData<RodItemData>().harvestableTypes).Contains<HarvestableType>(this.itemDataToHarvest.harvestableType)));

        //List<ResearchableItemData> list = allItems.Where<ResearchableItemData>((Func<ResearchableItemData, bool>)(i => i.researchBenefitType == benefitType)).ToList<ResearchableItemData>();

    //    int matchingItemCount = 0;
    //    SpatialItemData itemData;
    //    grid.spatialItems.ForEach((Action<SpatialItemInstance>) (i =>
    //{
    //  itemData = i.GetItemData<SpatialItemData>();
    //  if (!(itemData is FishItemData) || !(itemData as FishItemData).IsAberration)
    //    return;
    //  ++matchingItemCount;
    //}));
    //return matchingItemCount >= this.targetItemCount;


        //[HarmonyPatch(typeof(HarvestMinigameView), "Show")]
        public class HarvestMinigameView_Show_Patch
        {
            public static void Prefix(HarvestMinigameView __instance, HarvestPOI harvestPOI)
            {
                bool aber = harvestPOI.HarvestPOIData.MainItemsHaveAberrations();
                string aber_ = " ";
                if (aber)
                    aber_ = " aber ";

                string nightAber = "";
                if (harvestPOI.HarvestPOIData.usesTimeSpecificStock && harvestPOI.HarvestPOIData.NighttimeItemsHaveAberrations())
                {
                    nightAber = "night aber ";
                }
                //Util.Message(harvestPOI.name + aber_ + nightAber);
                //bool randomFishSpot = harvestPOI.GetComponent<RandomFishSpot>();
                //if (Config.debugMode.Value)
                //    Util.Message(harvestPOI.name + " randomFishSpot " + randomFishSpot);
                //else
                //    Util.Log(harvestPOI.name + " Show randomFishSpot " + randomFishSpot);

                string stock = "Stock " + (int)harvestPOI.Stock + "/" + harvestPOI.harvestable.GetMaxStock();
                float currentDepth = GameManager.Instance.WaveController.SampleWaterDepthAtPosition(GameManager.Instance.WaveController.GetSamplePositionByWorldPosition(harvestPOI.transform.position));
                string formattedDepthString = GameManager.Instance.ItemManager.GetFormattedDepthString(currentDepth);
                //Util.Log("HarvestMinigameView show " + stock + " depth " + formattedDepthString);
                Util.Message("Stock " + stock + " Restocks " + harvestPOI.harvestable.GetDoesRestock());
                //Util.Message(harvestPOI.name + " IsHarvestable " + harvestPOI.HarvestPOIData.IsHarvestable() );
            }
        }

        //[HarmonyPatch(typeof(DeployableItemData), "CatchRate", MethodType.Getter)]
        class DeployableItemData_CatchRate_Patch
        {
            public static void Postfix(DeployableItemData __instance, ref float __result)
            {
                Util.Log("DeployableItemData CatchRate " + __result);
                //ListHoodedQuests(__instance);
                //ListQuests(__instance);
            }
        }


        //[HarmonyPatch(typeof(GridManager), "AddActiveGrid")]
        class GridManager_AddActiveGrid_PrefixPatch
        {
            public static void Prefix(GridManager __instance, GridUI gridUI)
            {
                Util.Log("AddActiveGrid " + gridUI.name);
                //gridUI.
                //return false;
            }
        }

        //[HarmonyPatch(typeof(GridManager), "ObjectPickedUp")]
        class GridManager_ObjectPickedUp_PrefixPatch
        {
            public static void Prefix(GridManager __instance, GridObject o)
            {
                Util.Message("ObjectPickedUp " + o.name + " id: " + o.ItemData.id);
                Util.Log("ObjectPickedUp " + o.name + " id: " + o.ItemData.id);
                //o.ItemData.canBeSoldByPlayer
                //return false;
            }
        }


        //[HarmonyPatch(typeof(QuestManager), "ListQuests")]
        class QuestManager_ListQuestSteps_Patch
        {
            public static void Postfix(QuestManager __instance)
            {
                //ListHoodedQuests(__instance);
                //ListQuests(__instance);
            }
        }

        //[HarmonyPatch(typeof(BanishAbility), "Activate")]
        class BanishAbility_Patch
        {
            public static void Prefix(BanishAbility __instance)
            {
                //ListHoodedQuests(__instance);
                //ListQuests(__instance);
                Util.Log(" BanishAbility cooldown " + __instance.abilityData.cooldown);
                //__instance.abilityData.cooldown = 0;
                Util.Log(" BanishAbility cooldown a " + __instance.abilityData.cooldown);
                //Util.Message(" BanishAbility cooldown " + __instance.abilityData.cooldown);
            }
        }


        //[HarmonyPatch(typeof(CinemachineFreeLookInputProvider), "GetAxisCustom")]
        class CinemachineFreeLookInputProvider_GetAxisCustom_PostfixPatch
        {
            private static float mouseX;
            public static bool Prefix(CinemachineFreeLookInputProvider __instance, string axisName, ref float __result)
            {
                if (!GameManager.Instance.IsPaused && __instance.canMoveCamera && !__instance.playerCamera.IsRecentering && !GameManager.Instance.UI.IsShowingRadialMenu && (GameManager.Instance.Input.IsUsingController || __instance.freelook || __instance.spyglassEnabled || GameManager.Instance.Input.Controls.CameraMoveButton.IsPressed))
                {
                    if (axisName.Equals("Mouse X"))
                    {
                        mouseX = Input.GetAxisRaw("Mouse X") ;
                        //mouseX = Mathf.Clamp(mouseX, -9f, 9f);
                        if (Input.GetKeyDown(KeyCode.LeftShift))
                            Util.Message("GetAxisRaw " + Input.GetAxisRaw("Mouse X") + " GetAxis " + Input.GetAxis("Mouse X"));
                        //__result = __instance.cameraMoveAction.Value.x;
                        //__result =  Input.GetAxisRaw("Mouse X");

                        __result = mouseX;
                        return false;
                    }
                    else if (axisName.Equals("Mouse Y"))
                    {
                        if (Input.GetKeyDown(KeyCode.LeftShift))
                            Util.Message(" Mouse Y ");
                        __result =  Input.GetAxisRaw("Mouse Y");
                        //__result = __instance.cameraMoveAction.Value.y;
                        return false;
                    }
                }
                __result = 0f;
                return false;
            }
        }

        //[HarmonyPatch(typeof(PlayerStats), "CalculateRodStats")]
        class PlayerStats_CalculateRodStats_Patch
        {
            public static bool Prefix(PlayerStats __instance)
            {
                Util.Log("CalculateRodStats  ");
                //if (__instance.HarvestableTypes == null)
                //    __instance.HarvestableTypes = new HashSet<HarvestableType>();
                //else
                //    __instance.HarvestableTypes.Clear();

                //List<SpatialItemInstance> rodItems = GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.ROD);
                //float totalFishingSpeedModifier = GameManager.Instance.GameConfigData.BaseFishingSpeedModifier;
                //float aberrationCatchBonus = 0.0f;
                //foreach (SpatialItemInstance rod in rodItems)
                //{
                //    RodItemData rodItemData = rod.GetItemData<RodItemData>();
                //    for (int j = 0; j < rodItemData.harvestableTypes.Length; j++)
                //    {
                //        __instance.HarvestableTypes.Add(rodItemData.harvestableTypes[j]);
                //    }
                //    Util.Log("rod " + rod.id + " aber " + rodItemData.aberrationCatchBonus);
                //    if (!rod.GetIsOnDamagedCell())
                //    {
                //        totalFishingSpeedModifier += rodItemData.fishingSpeedModifier;
                //        aberrationCatchBonus += rodItemData.aberrationCatchBonus * 2f;
                //    }
                //}
                //__instance.EquipmentFishingSpeedModifier = totalFishingSpeedModifier;
                //__instance.AberrationCatchBonus = aberrationCatchBonus;
                //GameManager.Instance.SaveData.Inventory.GetAllItemsOfType<SpatialItemInstance>(ItemType.EQUIPMENT, ItemSubtype.DREDGE).ForEach(delegate (SpatialItemInstance d)
                //{
                //    HarvesterItemData itemData = d.GetItemData<HarvesterItemData>();
                //    for (int i = 0; i < itemData.harvestableTypes.Length; i++)
                //    {
                //        //Util.Log("DREDGE harvestableTypes " + itemData.harvestableTypes[i]);
                //        __instance.HarvestableTypes.Add(itemData.harvestableTypes[i]);
                //    }
                //});

                return false;
            }
        }



        //[HarmonyPatch(typeof(TooltipUI), "ConstructSpatialItemTooltip")]
        class TooltipUI_Patch
        {
            public static bool Prefix(TooltipUI __instance, SpatialItemInstance itemInstance, ItemData itemData, TooltipMode tooltipMode)
            {
                //Util.Message("ConstructSpatialItemTooltip");
                __instance.PrepareForTooltipShow();
                __instance.activeTooltipSections.Add((ILayoutable)__instance.itemHeaderWithIcon);
                __instance.itemHeaderWithIcon.Init<ItemData>(itemData, tooltipMode);
                __instance.itemHeaderWithIcon.SetObscured(tooltipMode == TooltipMode.MYSTERY);
                if (tooltipMode == TooltipUI.TooltipMode.HINT && itemData.itemSubtype == ItemSubtype.FISH)
                {
                    __instance.activeTooltipSections.Add((ILayoutable)__instance.fishHarvestDetails);
                    __instance.fishHarvestDetails.Init<FishItemData>(itemData as FishItemData, tooltipMode);
                }
                if (tooltipMode == TooltipMode.HOVER)
                {
                    if (itemData.itemSubtype == ItemSubtype.FISH)
                    {
                        __instance.activeTooltipSections.Add((ILayoutable)__instance.fishDetails);
                        __instance.fishDetails.Init<FishItemInstance>(itemInstance as FishItemInstance, tooltipMode);
                    }
                    if (itemData.itemType == ItemType.EQUIPMENT && itemData.itemSubtype != ItemSubtype.POT)
                    {
                        Util.Message("EQUIPMENT tooltip " + itemInstance.id);
                        __instance.activeTooltipSections.Add((ILayoutable)__instance.equipmentDetails);
                        __instance.equipmentDetails.Init<SpatialItemInstance>(itemInstance, tooltipMode);
                    }
                }
                if (tooltipMode == TooltipMode.HOVER || tooltipMode == TooltipMode.RESEARCH_PREVIEW || tooltipMode == TooltipMode.MYSTERY)
                {
                    if (itemData.itemSubtype == ItemSubtype.ROD)
                    {
                        __instance.activeTooltipSections.Add((ILayoutable)__instance.rodDetails);
                        __instance.rodDetails.Init<RodItemData>(itemData as RodItemData, itemInstance, tooltipMode);
                    }
                    if (itemData.itemSubtype == ItemSubtype.DREDGE)
                    {
                        __instance.activeTooltipSections.Add((ILayoutable)__instance.dredgeDetails);
                        __instance.dredgeDetails.Init<DredgeItemData>(itemData as DredgeItemData, tooltipMode);
                    }
                    if (itemData.itemSubtype == ItemSubtype.ENGINE && tooltipMode != TooltipMode.MYSTERY)
                    {
                        __instance.activeTooltipSections.Add((ILayoutable)__instance.engineDetails);
                        __instance.engineDetails.Init<EngineItemData>(itemData as EngineItemData, itemInstance, tooltipMode);
                    }
                    if (itemData.itemSubtype == ItemSubtype.LIGHT)
                    {
                        __instance.activeTooltipSections.Add((ILayoutable)__instance.lightDetails);
                        __instance.lightDetails.Init<LightItemData>(itemData as LightItemData, itemInstance, tooltipMode);
                    }
                    if (itemData.itemSubtype == ItemSubtype.POT && tooltipMode != TooltipMode.MYSTERY)
                    {
                        __instance.activeTooltipSections.Add((ILayoutable)__instance.deployableDetails);
                        __instance.deployableDetails.Init<DeployableItemData>(itemData as DeployableItemData, itemInstance, tooltipMode);
                    }
                    if (itemData.itemSubtype == ItemSubtype.NET)
                    {
                        __instance.activeTooltipSections.Add((ILayoutable)__instance.deployableDetails);
                        __instance.deployableDetails.Init<DeployableItemData>(itemData as DeployableItemData, itemInstance, tooltipMode);
                    }
                    if (tooltipMode != TooltipMode.MYSTERY)
                    {
                        __instance.activeTooltipSections.Add((ILayoutable)__instance.description);
                        __instance.description.Init<ItemData>(itemData, tooltipMode);
                    }
                }
                if (tooltipMode != TooltipMode.HINT)
                {
                    __instance.activeTooltipSections.Add((ILayoutable)__instance.controlPrompts);
                    __instance.controlPrompts.Init();
                }
                if (__instance.layoutCoroutine != null)
                    __instance.StopCoroutine(__instance.layoutCoroutine);

                __instance.layoutCoroutine = __instance.StartCoroutine(__instance.DoUpdateLayoutGroups());
                return false;
            }
        }

        //[HarmonyPatch(typeof(TooltipSectionEquipmentDetails), "RefreshUI")]
        class TooltipSectionEquipmentDetails_Patch
        {
            public static bool Prefix(TooltipSectionEquipmentDetails __instance)
            {
                //Util.Message("ConstructSpatialItemTooltip");
                __instance.isLayedOut = false;
                //string str = GameManager.Instance.ItemManager.GetInstallTimeForItem(__instance.spatialItemInstance.GetItemData<SpatialItemData>()).ToString(".#");
                //__instance.installTimeTextField.text = "<color=#" + ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL)) + ">" + str + "h</color>";
                //bool flag = false;
                //if (__instance.spatialItemInstance.GetItemData<SpatialItemData>().damageMode == DamageMode.DURABILITY)
                //    flag = __instance.spatialItemInstance.durability <= 0.0;
                //else if (__instance.spatialItemInstance.GetItemData<SpatialItemData>().damageMode == DamageMode.OPERATION)
                //    flag = __instance.spatialItemInstance.GetIsOnDamagedCell();

                //__instance.operationalStatusLocalizedString.StringReference.SetReference(LanguageManager.STRING_TABLE, (TableEntryReference)(flag ? "equipment-status.damaged" : "equipment-status.operational"));
                //__instance.operationalStatusTextField.color = flag ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
                __instance.isLayedOut = true;
                return false;
            }
        }

        //[HarmonyPatch(typeof(TooltipUI), "DoUpdateLayoutGroups")]
        class TooltipUI_Patch_
        {
            public static bool Prefix(TooltipUI __instance, IEnumerator __result)
            {
                return false;
            }
            public static void Postfix(TooltipUI __instance)
            {
                //Util.Message("ConstructSpatialItemTooltip");
                GameManager.Instance.StartCoroutine(DoUpdateLayoutGroups(__instance));
                //return false;
            }
        }

        //[HarmonyPatch(typeof(GameEvents), "TriggerWorldPhaseChanged")]
        class GameEvents_Patch
        {
             public static void Postfix(GameEvents __instance, int worldPhase)
            {
                Util.Message("TriggerWorldPhaseChanged " + worldPhase);
                Util.Log("TriggerWorldPhaseChanged " + worldPhase);
                //GameManager.Instance.StartCoroutine(DoUpdateLayoutGroups(__instance));
                //return false;
            }
        }

        static IEnumerator DoUpdateLayoutGroups(TooltipUI tooltipUi)
        {
            do
            {
                yield return new WaitForEndOfFrame();
            }
            while (!tooltipUi.activeTooltipSections.TrueForAll((Predicate<ILayoutable>)(x => x.IsLayedOut)));
            //tooltipUi.activeTooltipSections.ForEach((Action<ILayoutable>)(x => x.GameObject.SetActive(true)));
            foreach (ILayoutable layoutable in tooltipUi.activeTooltipSections)
            {
                if (layoutable.GameObject.name == "TooltipSectionControlPrompts")
                    continue;

                Util.Log("layoutable type " + layoutable.GetType() + " GameObject " + layoutable.GameObject.name);
                layoutable.GameObject.SetActive(true);
            }
            tooltipUi.verticalLayoutGroup.enabled = false;
            yield return new WaitForEndOfFrame();
            tooltipUi.verticalLayoutGroup.enabled = true;
            yield return new WaitForEndOfFrame();
            tooltipUi.canvasGroupFadeTween = tooltipUi.canvasGroup.DOFade(1f, 0.35f);
            tooltipUi.canvasGroupFadeTween.SetUpdate<Tweener>(true);
            // ISSUE: reference to a compiler-generated method
            tooltipUi.canvasGroupFadeTween.OnComplete<Tweener>((TweenCallback)delegate
            {
                tooltipUi.canvasGroupFadeTween = null;
            });
            tooltipUi.layoutCoroutine = null;
        }

        //[HarmonyPatch(typeof(Debug))]
        class Debug_Log_Patch
        {
            //[HarmonyPrefix] [HarmonyPatch(nameof(Debug.Log), new Type[] { typeof(object) })]
            public static bool LogPrefix(Debug __instance, object message)
            {
                Util.Log(message.ToString());
                return false;
            }
            //[HarmonyPrefix] [HarmonyPatch(nameof(Debug.LogWarning), new Type[] { typeof(object) })]
            public static bool LogWarningPrefix(Debug __instance, object message)
            {
                Util.Log(message.ToString());
                return false;
            }
        }

        public static Dictionary<string, int> fishCount = new ();
        public static Dictionary<string, string> fishF = new ();
        public static HashSet<string> dayFishSpots = new ();
        public static HashSet<string> nightFishSpots = new ();


        //[HarmonyPatch(typeof(HarvestPOI), "Awake")]
        class HarvestPOI_Patch
        {
            public static void Postfix(HarvestPOI __instance )
            {
                if (Util.IsfishingSpot( __instance))
                {
                    if (__instance.harvestPOIData == null)
                    {
                        Util.Log("HarvestPOI harvestPOIData == null");
                        return;
                    }
                    StringBuilder sb = new StringBuilder();
                    List<HarvestableItemData> dayFish = __instance.harvestPOIData.items;
                    List<HarvestableItemData> nightFish = __instance.harvestPOIData.nightItems;
                    if (dayFish != null && dayFish.Count > 0)
                    {
                        string id = dayFish[0].id;
                        if (fishCount.ContainsKey(id))
                            fishCount[id]++;
                        else
                            fishCount[id] = 1;
                    }
                    else if (nightFish != null && nightFish.Count > 0)
                    {
                        string id = nightFish[0].id;
                        if (fishCount.ContainsKey(id))
                            fishCount[id]++;
                        else
                            fishCount[id] = 1;
                    }
                    //fishFF.Add(sb.ToString());
                    //Util.Log("HarvestPOI Awake " + id);
                }
            }
        }


        public static bool ListQuests(QuestManager questManager)
        {
            StringBuilder sb = new("___ Quests begin ___");
            foreach (QuestData i in questManager.allQuests.Values.ToList())
            {
                sb.Append(i.name);
                sb.Append(", ");
            }
            sb.Append("___ Quests end ___");
            Util.Log(sb.ToString());
            return false;
        }

        public static bool ListHoodedQuests(QuestManager questManager)
        {
            StringBuilder sb = new("___ Quests begin ___");
            sb.Append(Environment.NewLine);
            foreach (QuestData questData in questManager.allQuests.Values.ToList())
            {
                if (questData.name.StartsWith("Quest_HoodedFigure"))
                {
                    sb.Append(questData.name);
                    sb.Append(Environment.NewLine);

                    foreach (QuestData subQuest in questData.subquests)
                    {
                        sb.Append(questData.name + " subquest " + subQuest.name);
                        sb.Append(Environment.NewLine);
                    }
                    if (questData.steps != null && questData.steps.Count > 0)
                    {
                        sb.Append(questData.name + " steps");
                        sb.Append(Environment.NewLine);
                    }
                    foreach (QuestStepData questStepData in questData.steps)
                    {
                        //sb.Append(questStepData.name + " can fail " + questStepData.canBeFailed);
                        if (questStepData.failureEvents != null && questStepData.failureEvents.Count > 0)
                        {
                            //sb.Append(questStepData.name + " failureEvents");
                            //sb.Append(Environment.NewLine);
                            //foreach (QuestStepEvent questStepEvent in questStepData.failureEvents)
                            //{
                            //}
                        }
                        sb.Append(Environment.NewLine);
                    }
                }
                //sb.Append(questData.name);
                //sb.Append(", ");
            }
            sb.Append(Environment.NewLine);
            sb.Append("___ Quests end ___");
            Util.Log(sb.ToString());
            return false;
        }

        public static bool ListFailableQuestSteps(QuestManager questManager)
        {
            StringBuilder sb = new("Failable quests");
            foreach (QuestStepData q in questManager.allQuestSteps.Values.ToList())
            {
                if (q.canBeFailed)
                {
                    sb.Append(q.name);
                    sb.Append(", ");
                }
            }
            Util.Log(sb.ToString());
            return false;
        }

        //[HarmonyPatch(typeof(Targeting), "GetTarget", new Type[] { typeof(float), typeof(GameObject), typeof(float), typeof(Targeting.FilterRaycast) }, new[] { ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal })]
        class Targeting_GetTarget_PostfixPatch
        {
            public static void Postfix(ref GameObject result)
            {
                //AddDebug(" Targeting GetTarget  " + result.name);
            }
        }

    }
}
