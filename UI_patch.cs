using DG.Tweening;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Tweaks
{
    internal class UI_patch
    {
        [HarmonyPatch(typeof(MapWindow), "RefreshPlayerMarkerPosition")]
        class MapWindow_RefreshPlayerMarkerPosition_PostfixPatch
        {
            public static void Prefix(MapWindow __instance)
            {
                //Util.Log(" RefreshPlayerMarkerPosition  ");
                __instance.youAreHereMarkerTransform.gameObject.SetActive(Config.showPlayerMarkerOnMap.Value);
            }
        }

        [HarmonyPatch(typeof(SpyglassUI), "Update")]
        public class SpyglassUI_Update_Patch
        {
            public static bool Prefix(SpyglassUI __instance)
            {
                return Config.spyGlassShowsFishingSpots.Value;
            }
        }

        [HarmonyPatch(typeof(InteractPointUI), "Show")]
        public class UIController_ShowDestination_Patch
        {
            public static void Prefix(InteractPointUI __instance)
            {
                InteractPointUI.showInteractIcon = Config.showPOIicon.Value;
                //Util.Message("InteractPointUI Show");
            }
        }

        //[HarmonyPatch(typeof(PlayerFundsUI), "OnPlayerFundsChanged")]
        public class PlayerFundsUI_OnPlayerFundsChanged_Patch
        {
            public static bool Prefix(PlayerFundsUI __instance, Decimal newTotal, Decimal changeAmount)
            { // money in top right corner
                //int total = (int)newTotal;
                //Util.Message("OnPlayerFundsChanged " + newTotal);
                //Util.Log("OnPlayerFundsChanged " + newTotal);
                __instance._textField.text = "$" + newTotal.ToString("n0", LocalizationSettings.SelectedLocale.Formatter);
                return false;
            }
        }

        //[HarmonyPatch(typeof(EncyclopediaPage), "PopulatePage")]
        public class EncyclopediaPage_Patch
        {
            public static bool Prefix(EncyclopediaPage __instance, FishItemData itemData, int index)
            {
                if (itemData == null)
                {
                    __instance.pageContainer.SetActive(false);
                    return false;
                }
                __instance.pageContainer.SetActive(true);
                Color color1 = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL);
                Color color2 = __instance.harvestTypeTagConfig.textColorLookup[itemData.harvestableType];
                int caughtCountById = GameManager.Instance.SaveData.GetCaughtCountById(itemData.id);
                bool flag = caughtCountById > 0;
                string str = flag ? itemData.itemNameKey.GetLocalizedString() : "???";
                __instance.itemNameText.text = string.Format("#{0} {1}", (object)(index + 1), (object)str);
                __instance.itemImage.sprite = itemData.sprite;
                __instance.itemImage.color = flag ? __instance.itemImageColorIdentified : __instance.itemImageColorUnidentified;
                __instance.itemImageGrid.sizeDelta = new Vector2((float)itemData.GetWidth() * 128f, (float)itemData.GetHeight() * 128f);
                __instance.undiscoveredItemImage.sprite = itemData.IsAberration ? __instance.undiscoveredAberrationSprite : __instance.undiscoveredRegularSprite;
                __instance.undiscoveredItemImage.color = itemData.IsAberration ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
                __instance.undiscoveredItemImage.gameObject.SetActive(!flag);
                if (__instance.harvestTypeTagConfig.colorLookup.ContainsKey(itemData.harvestableType))
                    color1 = __instance.harvestTypeTagConfig.colorLookup[itemData.harvestableType];
                else
                    Debug.LogError((object)string.Format("[EncyclopediaPage] RefreshUI({0}) couldn't find color in config.", (object)itemData.harvestableType));
                __instance.harvestTypeBackplate.color = color1;
                if (itemData.harvestableType == HarvestableType.NONE || itemData.harvestableType == HarvestableType.CRAB)
                {
                    string depthString = itemData.GetDepthString();
                    __instance.harvestTypeLocalizedText.enabled = false;
                    __instance.harvestTypeLocalizedText.StringReference.SetReference((TableReference)LanguageManager.STRING_TABLE, (TableEntryReference)"encyclopedia.depth");
                    __instance.harvestTypeLocalizedText.StringReference.Arguments = new object[1] { depthString };
                    __instance.harvestTypeLocalizedText.enabled = true;
                }
                else if (__instance.harvestTypeTagConfig.stringLookup.ContainsKey(itemData.harvestableType))
                    __instance.harvestTypeLocalizedText.StringReference.SetReference((TableReference)LanguageManager.STRING_TABLE, (TableEntryReference)__instance.harvestTypeTagConfig.stringLookup[itemData.harvestableType]);
                else
                    Debug.LogError((object)string.Format("[EncyclopediaPage] RefreshUI({0}) couldn't find string in config.", (object)itemData.harvestableType));
                __instance.harvestTypeText.color = color2;
                if (caughtCountById > 0)
                {
                    __instance.caughtCounterLocalizedText.enabled = false;
                    __instance.caughtCounterLocalizedText.StringReference.SetReference((TableReference)LanguageManager.STRING_TABLE, (TableEntryReference)"encyclopedia.caught-some");
                    __instance.caughtCounterLocalizedText.StringReference.Arguments = new object[1] { caughtCountById };
                    __instance.caughtCounterLocalizedText.enabled = true;
                    __instance.caughtCounterText.color = Color.black;
                    __instance.caughtCounterDiamond.color = Color.black;
                }
                else
                {
                    __instance.caughtCounterLocalizedText.enabled = false;
                    __instance.caughtCounterLocalizedText.StringReference.SetReference((TableReference)LanguageManager.STRING_TABLE, (TableEntryReference)"encyclopedia.caught-none");
                    __instance.caughtCounterLocalizedText.enabled = true;
                    __instance.caughtCounterText.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
                    __instance.caughtCounterDiamond.color = GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE);
                }
                __instance.descriptionLocalizedText.text = flag ? itemData.itemDescriptionKey.GetLocalizedString() : "???";
                __instance.descriptionLocalizedText.color = color2;
                __instance.descriptionBackplate.color = color1;
                LocalizedString zoneString = __instance.encyclopediaConfig.zoneStrings[itemData.zonesFoundIn];
                Sprite zoneIconSprite = __instance.encyclopediaConfig.zoneIconSprites[itemData.zonesFoundIn];
                if (itemData.LocationHiddenUntilCaught && caughtCountById == 0)
                {
                    zoneString = __instance.encyclopediaConfig.zoneStrings[ZoneEnum.NONE];
                    zoneIconSprite = __instance.encyclopediaConfig.zoneIconSprites[ZoneEnum.NONE];
                }
                __instance.zoneImage.sprite = zoneIconSprite;
                __instance.localizedZoneText.StringReference = zoneString;
                __instance.zoneText.HighlightedBackplateColor = color1;
                __instance.zoneText.HighlightedTextColor = color2;
                __instance.zoneText.SetHighlighted(true);
                foreach (KeyValuePair<ItemSubtype, ToggleableTextWithBackplate> catchTypeText in __instance.catchTypeTexts)
                {
                    catchTypeText.Value.HighlightedTextColor = color2;
                    catchTypeText.Value.HighlightedBackplateColor = color1;
                    if (catchTypeText.Key == ItemSubtype.ROD)
                        catchTypeText.Value.SetHighlighted(itemData.canBeCaughtByRod);
                    if (catchTypeText.Key == ItemSubtype.POT)
                        catchTypeText.Value.SetHighlighted(itemData.canBeCaughtByPot);
                    if (catchTypeText.Key == ItemSubtype.NET)
                        catchTypeText.Value.SetHighlighted(itemData.canBeCaughtByNet);
                }
                if (flag)
                {
                    float largestFishRecordById = GameManager.Instance.SaveData.GetLargestFishRecordById(itemData.id);
                    __instance.largestText.text = GameManager.Instance.ItemManager.GetFormattedFishSizeString(largestFishRecordById, itemData);
                    __instance.trophyIcon.color = (double)largestFishRecordById > (double)GameManager.Instance.GameConfigData.TrophyMaxSize ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.VALUABLE) * __instance.trophyIconColorMultiplier : Color.black;
                    __instance.trophyIcon.color = new Color(__instance.trophyIcon.color.r, __instance.trophyIcon.color.g, __instance.trophyIcon.color.b, 1f);
                }
                else
                {
                    __instance.largestText.text = "-";
                    __instance.trophyIcon.color = Color.black;
                }
                __instance.valueText.text = GameManager.Instance.DialogueRunner.GetNumItemsSoldById(itemData.id) > 0 ? itemData.value.ToString("n0", LocalizationSettings.SelectedLocale.Formatter) : "???";
                __instance.dayToggler.HighlightedBackplateColor = color1;
                __instance.dayToggler.HighlightedImageColor = color2;
                __instance.nightToggler.HighlightedBackplateColor = color1;
                __instance.nightToggler.HighlightedImageColor = color2;
                __instance.dayToggler.SetHighlighted(itemData.Day);
                __instance.nightToggler.SetHighlighted(itemData.Night);
                if (itemData.IsAberration)
                {
                    FishItemData aberrationParent = itemData.NonAberrationParent;
                    __instance.aberrationInfos[0].BasicButtonWrapper.OnSubmitAction = (Action)(() =>
                    {
                        Action<FishItemData> pageLinkRequest = __instance.PageLinkRequest;
                        if (pageLinkRequest == null)
                            return;
                        pageLinkRequest(itemData.NonAberrationParent);
                    });
                    __instance.aberrationInfos[0].SetData(itemData.NonAberrationParent);
                    __instance.aberrationInfos[0].gameObject.SetActive(true);
                    __instance.aberrationInfos[1].gameObject.SetActive(false);
                    __instance.aberrationInfos[2].gameObject.SetActive(false);
                    __instance.aberrationsContainer.SetActive(true);
                    __instance.aberrationsHeaderText.StringReference = __instance.aberrationOfLocalizedString;
                }
                else if (itemData.Aberrations.Count > 0)
                {
                    for (int index1 = 0; index1 < __instance.aberrationInfos.Count; ++index1)
                    {
                        if (itemData.Aberrations.Count > index1)
                        {
                            FishItemData aberrationData = itemData.Aberrations[index1];
                            __instance.aberrationInfos[index1].BasicButtonWrapper.OnSubmitAction = (Action)(() =>
                            {
                                Action<FishItemData> pageLinkRequest = __instance.PageLinkRequest;
                                if (pageLinkRequest == null)
                                    return;
                                pageLinkRequest(aberrationData);
                            });
                            __instance.aberrationInfos[index1].SetData(aberrationData);
                            __instance.aberrationInfos[index1].gameObject.SetActive(true);
                        }
                        else
                            __instance.aberrationInfos[index1].gameObject.SetActive(false);
                    }
                    __instance.aberrationsHeaderText.StringReference = __instance.aberrationsLocalizedString;
                    __instance.aberrationsContainer.SetActive(true);
                }
                else
                    __instance.aberrationsContainer.SetActive(false);
                __instance.fadeTween = (Tweener)__instance.canvasGroup.DOFade(1f, __instance.fadeDurationSec);
                __instance.fadeTween.SetUpdate<Tweener>(true);
                __instance.fadeTween.OnComplete<Tweener>(() => __instance.fadeTween = null);

                return false;
            }
        }

        //[HarmonyPatch(typeof(UpgradeGridPanel), "OnPlayerFundsChanged")]
        public class UpgradeGridPanel_Patch
        {
            public static bool Prefix(UpgradeGridPanel __instance, Decimal total, Decimal change)
            {
                string str1 = "___!!!___" + __instance.upgradeData.MonetaryCost.ToString("n0", LocalizationSettings.SelectedLocale.Formatter);
                //string str1 = "$" + __instance.upgradeData.MonetaryCost.ToString("n0", LocalizationSettings.SelectedLocale.Formatter);
                string str2;
                if (total >= __instance.upgradeData.MonetaryCost)
                    str2 = str1;
                else
                    str2 = "<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.NEGATIVE) + ">" + str1 + "</color>";
                __instance.bottomButton.LocalizedString.StringReference.Arguments = new string[1] { str2 };
                __instance.bottomButton.LocalizedString.StringReference.RefreshString();
                //string str1 = "$" + __instance.upgradeData.MonetaryCost.ToString("n0", LocalizationSettings.SelectedLocale.Formatter);
                //string str2;
                //if (total >= __instance.upgradeData.MonetaryCost)
                //    str2 = str1;
                //else
                //    str2 = "<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.NEGATIVE) + ">" + str1 + "</color>";
                //__instance.bottomButton.LocalizedString.StringReference.Arguments = (IList<object>)new string[1] { str2 };
                //__instance.bottomButton.LocalizedString.StringReference.RefreshString();
                return false;
            }
        }

        //[HarmonyPatch(typeof(UIController), "PrepareItemNameForSellNotification")]
        public class UIController_Patch
        {
            public static bool Prefix(UIController __instance, NotificationType notificationType, LocalizedString itemNameKey, Decimal incomeAmount, Decimal debtRepaymentAmount, bool isRefund)
            {
                LocalizationSettings.StringDatabase.GetLocalizedStringAsync(itemNameKey.TableReference, itemNameKey.TableEntryReference, null, FallbackBehavior.UseProjectSettings).Completed += (Action<AsyncOperationHandle<string>>)(op =>
                {
                    if (op.Status != AsyncOperationStatus.Succeeded)
                        return;
                    if (debtRepaymentAmount > 0M)
                        GameManager.Instance.UI.ShowNotification(notificationType, "notification.sell-fish-debt", new object[3]
                        {
          op.Result,
          ("<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE) + ">+$" + incomeAmount.ToString("n0", LocalizationSettings.SelectedLocale.Formatter) + "</color>"),
          ("<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.NEGATIVE) + ">-$" + debtRepaymentAmount.ToString("n0", LocalizationSettings.SelectedLocale.Formatter) + "</color>")
                        });
                    else if (isRefund)
                        GameManager.Instance.UI.ShowNotification(notificationType, "notification.refund-item", new object[2]
                        {
          op.Result,
          ("<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE) + ">+$" + incomeAmount.ToString("n0", LocalizationSettings.SelectedLocale.Formatter) + "</color>")
                        });
                    else
                        GameManager.Instance.UI.ShowNotification(notificationType, "notification.sell-item", new object[2]
                        {
          op.Result,
          ("<color=#" + GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.POSITIVE) + ">+$" + incomeAmount.ToString("n0", LocalizationSettings.SelectedLocale.Formatter) + "</color>")
                        });
                });
                return false;
            }
        }


        //[HarmonyPatch(typeof(SellModeActionHandler))]
        public class SellModeActionHandler_Patch
        {
            //[HarmonyPrefix]
            //[HarmonyPatch("OnFocusedItemChanged")]
            public static bool OnFocusedItemChangedPrefix(SellModeActionHandler __instance, GridObject gridObject)
            {
                GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[2] { __instance.sellAction, __instance.sellHoldAction }, ActionLayer.UI_WINDOW);
                if (!gridObject)
                {
                    gridObject = GameManager.Instance.GridManager.GetCurrentlyFocusedObject();
                }
                if (((bool)GameManager.Instance.GridManager.CurrentlyHeldObject && gridObject != GameManager.Instance.GridManager.CurrentlyHeldObject) || !gridObject || !__instance.DoesStoreAcceptThisItem(gridObject.ItemData, bulkMode: false))
                {
                    return false;
                }
                decimal num = default(decimal);
                if (gridObject.state == GridObjectState.IN_INVENTORY || gridObject.state == GridObjectState.IN_STORAGE || gridObject.state == GridObjectState.BEING_HARVESTED)
                {
                    num = GameManager.Instance.ItemManager.GetItemValue(gridObject.SpatialItemInstance, ItemManager.BuySellMode.SELL, __instance.sellValueModifier);
                    __instance.sellAction.promptString = "prompt.sell";
                    __instance.sellHoldAction.promptString = "prompt.sell";
                }
                else
                {
                    if (gridObject.state != GridObjectState.JUST_PURCHASED)
                    {
                        if (gridObject.state != GridObjectState.IN_SHOP)
                        {
                            Debug.LogWarning($"[SellModeActionHandler] OnFocusedItemChanged({gridObject}) has a weird state: {gridObject.state}");
                        }
                        return false;
                    }
                    num = GameManager.Instance.ItemManager.GetItemValue(gridObject.SpatialItemInstance, ItemManager.BuySellMode.BUY);
                    __instance.sellAction.promptString = "prompt.refund";
                    __instance.sellHoldAction.promptString = "prompt.refund";
                }
                string text = num.ToString("n0", LocalizationSettings.SelectedLocale.Formatter);
                string text2 = ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.POSITIVE));
                __instance._SellPrice = "<color=#" + text2 + ">$" + text + "</color>";
                object[] localizationArguments = new object[1] { __instance._SellPrice };
                __instance.sellAction.LocalizationArguments = localizationArguments;
                __instance.sellHoldAction.LocalizationArguments = localizationArguments;
                __instance.sellAction.TriggerPromptArgumentsChanged();
                __instance.sellHoldAction.TriggerPromptArgumentsChanged();
                GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[1] { __instance.GetSellAction(gridObject) }, ActionLayer.UI_WINDOW);
                return false;
            }

            //[HarmonyPrefix]
            //[HarmonyPatch("CheckSellAllValidity")]
            public static bool CheckSellAllValidityPrefix(SellModeActionHandler __instance)
            {
                bool flag = true;
                if (GameManager.Instance.GridManager.CurrentlyHeldObject != null && GameManager.Instance.GridManager.CurrentlyHeldObject.state == GridObjectState.JUST_PURCHASED)
                    flag = false;

                if (flag)
                {
                    List<SpatialItemInstance> sellableItemInstances = __instance.GetBulkSellableItemInstances();
                    Decimal sellAllValue = 0M;
                    sellableItemInstances.ForEach((Action<SpatialItemInstance>)(itemInstance => sellAllValue += GameManager.Instance.ItemManager.GetItemValue(itemInstance, ItemManager.BuySellMode.SELL, __instance.sellValueModifier)));
                    __instance.sellAllValueString = "$" + sellAllValue.ToString("n0", LocalizationSettings.SelectedLocale.Formatter);
                    __instance.sellAllAction.LocalizationArguments = (object[])new string[1]
                    { __instance.sellAllValueString };
                    __instance.sellAllAction.TriggerPromptArgumentsChanged();
                    flag = flag && sellableItemInstances.Count > 0;
                    if (sellableItemInstances.Count > 0)
                        flag = true;
                }
                if (flag)
                    __instance.sellAllAction.Enable();
                else
                    __instance.sellAllAction.Disable(true);

                return false;
            }

            //[HarmonyPrefix] !
            //[HarmonyPatch(MethodType.Constructor)]
            public static bool SellModeActionHandlerPrefix(SellModeActionHandler __instance)
            {
                Util.Log("SellModeActionHandler constructor " + __instance.sellAllValueString);
                __instance.sellAllValueString = "$0";
                __instance.sellAction = new DredgePlayerActionPress("prompt.sell", GameManager.Instance.Input.Controls.SellItem);
                __instance.sellAction.showInTooltip = true;
                DredgePlayerActionPress dredgePlayerActionPress = __instance.sellAction;
                dredgePlayerActionPress.OnPressEnd = (Action)Delegate.Combine(dredgePlayerActionPress.OnPressEnd, new Action(__instance.SellFocusedItem));
                __instance.sellAction.priority = 4;
                __instance.sellHoldAction = new DredgePlayerActionHold("prompt.sell", GameManager.Instance.Input.Controls.SellItem, 0.75f);
                __instance.sellHoldAction.showInTooltip = true;
                DredgePlayerActionHold dredgePlayerActionHold = __instance.sellHoldAction;
                dredgePlayerActionHold.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold.OnPressComplete, new Action(__instance.SellFocusedItem));
                __instance.sellHoldAction.priority = 4;
                __instance.sellAllAction = new DredgePlayerActionHold("prompt.sell-all", GameManager.Instance.Input.Controls.SellItem, 0.5f);
                __instance.sellAllAction.LocalizationArguments = (object[])new string[1] { __instance.sellAllValueString };
                __instance.sellAllAction.showInControlArea = true;
                DredgePlayerActionHold dredgePlayerActionHold2 = __instance.sellAllAction;
                dredgePlayerActionHold2.OnPressComplete = (Action)Delegate.Combine(dredgePlayerActionHold2.OnPressComplete, new Action(__instance.OnSellAllPressed));
                __instance.sellAllAction.priority = 5;
                GameEvents.Instance.OnItemRemovedFromCursor += __instance.OnItemRemovedFromCursor;
                Util.Log("SellModeActionHandler constructor 1 " + __instance.sellAllValueString);
                return false;
            }
        }

        //[HarmonyPatch(typeof(RepairActionHandler), "GetFormattedCostString")]
        public class RepairActionHandler_Patch
        {
            public static bool Prefix(RepairActionHandler __instance, Decimal cost, ref string __result)
            {
                string str = cost.ToString("n0", LocalizationSettings.SelectedLocale.Formatter);
                __result = "<color=#" + (!(GameManager.Instance.SaveData.Funds >= cost) ? ColorUtility.ToHtmlStringRGB(GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE)) : ColorUtility.ToHtmlStringRGB(Color.white)) + ">$" + str + "</color>";
                return false;
            }
        }

        //[HarmonyPatch(typeof(BuyModeActionHandler), "OnItemHoveredChanged")]
        public class BuyModeActionHandler_Patch
        {
            public static bool Prefix(BuyModeActionHandler __instance, GridObject gridObject)
            {
                //base.OnItemHoveredChanged(gridObject);
                bool flag1 = false;
                if (!GameManager.Instance.GridManager.CurrentlyHeldObject && gridObject && gridObject.state == GridObjectState.IN_SHOP)
                    flag1 = true;
                if (flag1)
                {
                    GameManager.Instance.Input.AddActionListener(new DredgePlayerActionBase[1]
                    {  __instance.buyAction }, ActionLayer.UI_WINDOW);
                    Decimal itemValue = GameManager.Instance.ItemManager.GetItemValue(gridObject.SpatialItemInstance, ItemManager.BuySellMode.BUY);
                    bool flag2 = GameManager.Instance.SaveData.Funds >= itemValue;
                    string str = itemValue.ToString("n0", LocalizationSettings.SelectedLocale.Formatter);
                    __instance._BuyPrice = "<color=#" + ColorUtility.ToHtmlStringRGB(flag2 ? GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEUTRAL) : GameManager.Instance.LanguageManager.GetColor(DredgeColorTypeEnum.NEGATIVE)) + ">$" + str + "</color>";
                    __instance.buyAction.LocalizationArguments = new object[1] { __instance._BuyPrice };
                    __instance.buyAction.TriggerPromptArgumentsChanged();
                    if (flag2)
                        __instance.buyAction.Enable();
                    else
                        __instance.buyAction.Disable(false);
                }
                else
                    GameManager.Instance.Input.RemoveActionListener(new DredgePlayerActionBase[1]
                    { __instance.buyAction }, ActionLayer.UI_WINDOW);

                return false;
            }
        }





    }
}
