using BepInEx;
using HarmonyLib;
using System;
using UnityEngine;
using BepInEx.Configuration;
using UnityEngine.Localization.Pseudo;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Tweaks
{
    public static class Util
    {
        public static float MapTo01range(int value, int min, int max)
        {
            float fl;
            int oldRange = max - min;

            if (oldRange == 0)
                fl = 0f;
            else
                fl = ((float)value - (float)min) / (float)oldRange;

            return fl;
        }

        public static float MapTo01range(float value, float min, float max)
        {
            float fl;
            float oldRange = max - min;

            if (oldRange == 0)
                fl = 0f;
            else
                fl = ((float)value - (float)min) / (float)oldRange;

            return fl;
        }

        public static int MapToRange(int value, int oldMin, int oldMax, int newMin, int newMax)
        {
            int oldRange = oldMax - oldMin;
            int newValue;

            if (oldRange == 0)
                newValue = newMin;
            else
            {
                int newRange = newMax - newMin;
                newValue = ((value - oldMin) * newRange) / oldRange + newMin;
            }
            return newValue;
        }

        public static float MapToRange(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            float oldRange = oldMax - oldMin;
            float newValue;

            if (oldRange == 0)
                newValue = newMin;
            else
            {
                float newRange = newMax - newMin;
                newValue = ((value - oldMin) * newRange) / oldRange + newMin;
            }
            return newValue;
        }

        public static void Message(string s)
        {
            //Main.log.LogMessage("Message s " + s);
            if (GameEvents.Instance == null || s == null || s.Length == 0)
                return;

            //Main.log.LogMessage ("Message s " + s);
            //Main.log.LogMessage ("Message previous " + previous);
            GameEvents.Instance.TriggerNotification(NotificationType.ERROR, s);

        }

        public static void Log(string s)
        {
            if (s == null || s.Length == 0)
                return;

            Main.log.LogDebug(s);
        }

        public static void LogError(string s)
        {
            if (s == null || s.Length == 0)
                return;

            Main.log.LogError(s);
        }

        public static void PrintItems()
        {
            foreach (ItemData itemData in GameManager.Instance.ItemManager.allItems)
            {
                Log(itemData.id + ", " + itemData.itemNameKey.GetLocalizedString() + ", type " + itemData.itemType + ", subtype " + itemData.itemSubtype);
            }
        }

        public static void ShowGameTime()
        {
            Message(GameManager.Instance.Time.GetTimeFormatted(GameManager.Instance.Time.Time));
        }

        public static string GetGameTime()
        {
            double f = GameManager.Instance.Time.Time * 24.0;
            float num1 = Mathf.FloorToInt((float)f);
            int num2 = Mathf.FloorToInt((((float)f - num1) * 60f));
            string str1 = "";
            string str2 = "";
            string str3 = num2.ToString("00.##");
            if (GameManager.Instance.SettingsSaveData.clockStyle == 0)
            {
                if (num1 < 12.0)
                    str1 = " AM";
                if (num1 >= 12.0)
                {
                    num1 %= 12f;
                    str1 = " PM";
                }
                if (num1 == 0.0)
                    num1 = 12f;
                str2 = num1.ToString();
            }
            else if (GameManager.Instance.SettingsSaveData.clockStyle == 1)
                str2 = num1.ToString("00.##");

            return str2 + " " + str3 + " " + str1;
        }

        public static HarvestQueryEnum IsHarvestable(HarvestPOIDataModel harvestPOIDataModel)
        {
            bool noStock = GameManager.Instance.Time.IsDaytime && harvestPOIDataModel.items.Count == 0 || !GameManager.Instance.Time.IsDaytime && harvestPOIDataModel.nightItems.Count == 0;
            if (harvestPOIDataModel.usesTimeSpecificStock && noStock)
                return HarvestQueryEnum.INVALID_INCORRECT_TIME;

            return harvestPOIDataModel.GetStockCount(false) < 1.0 ? HarvestQueryEnum.INVALID_NO_STOCK : HarvestQueryEnum.VALID;
        }

        public static void PrintHarvestPOIData(HarvestPOI __instance)
        {
            StringBuilder fishSB = new StringBuilder();
            bool timeSpecificStock = __instance.harvestable.GetUsesTimeSpecificStock();
            if (timeSpecificStock)
            {
                List<ItemData> items = __instance.harvestable.GetItems();
                if (items.Count > 0)
                {
                    fishSB.Append(" day ");
                    fishSB.Append(items[0].id);
                }
                List<ItemData> nightItems = __instance.harvestable.GetNightItems();
                if (nightItems.Count > 0)
                {
                    fishSB.Append(" night ");
                    fishSB.Append(nightItems[0].id);
                }
            }
            else
            {
                List<ItemData> items = __instance.harvestable.GetItems();
                if (items.Count > 0)
                {
                    fishSB.Append(" 24H ");
                    fishSB.Append(items[0].id);
                }
            }
            Log(__instance.name + " " + GetGameTime() + fishSB.ToString());
        }

        public static bool IsfishingSpot(HarvestPOI harvestPOI)
        {
            return harvestPOI.harvestable.GetHarvestableItemSubType() == ItemSubtype.FISH && !harvestPOI.IsBaitPOI && !harvestPOI.IsCrabPotPOI;
        }

        public static bool IsQuestItem(ItemData itemData)
        {
            return itemData.id.StartsWith("quest");
        }

        public static T EnsureComponent<T>(this GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }
            return comp;
        }

        public static Vector3 getPos(Transform transform, float dist)
        {
            Vector3 originalPosition = transform.position;
            float angleInDegrees = transform.eulerAngles.z * -90f / 180f; // assuming you're using degrees for angles
            Quaternion forwardRotation = Quaternion.AngleAxis(-angleInDegrees, Vector3.forward); // rotation around up axis
            Vector3 offsetForward = (originalPosition + Vector3.up).normalized * dist; // calculating offset as normalized direction vector multiplied with distance
            Vector3 newPosition = transform.position;
            newPosition += forwardRotation * offsetForward;
            return newPosition;
        }

        public static void PrintPlayerPos()
        {
            int x = (int)GameManager.Instance.Player.transform.position.x;
            int y = (int)GameManager.Instance.Player.transform.position.y;
            int z = (int)GameManager.Instance.Player.transform.position.z;
            float playerDepth = GameManager.Instance.WaveController.SampleWaterDepthAtPlayerPosition();
            string depth = GameManager.Instance.ItemManager.GetFormattedDepthString(playerDepth);
            Message("" + x + " " + y + " " + z + " depth " + depth );
        }

        public static Dictionary<HarvestableType, float> typeAvDepth = new();

        public static void GetHarvestPOIdata(HarvestPOI __instance)
        {
            if (IsfishingSpot(__instance))
            {
                HarvestableType hType = __instance.harvestable.GetHarvestType();
                Vector2 v2Pos = GameManager.Instance.WaveController.GetSamplePositionByWorldPosition(__instance.transform.position);
                float depth = GameManager.Instance.WaveController.SampleWaterDepthAtPosition(v2Pos);
                //HarvestableItemData itemData = __instance.harvestPOIData.GetNextHarvestableItem();
                //string nightId = "";
                //HarvestableItemData itemData = __instance.harvestPOIData.GetActiveFirstHarvestableItem();
                //if (__instance.harvestPOIData.usesTimeSpecificStock && __instance.harvestPOIData.nightItems.Count > 0)
                //{
                //    nightId = __instance.harvestPOIData.nightItems[0].id;
                //    Util.Log(nightId + " usesTimeSpecificStock");
                //}
                //else
                //    Util.Log(itemData.id + " no TimeSpecificStock");
                //HarvestableType hType = __instance.harvestPOIData.GetHarvestType();
                if (typeAvDepth.ContainsKey(hType))
                {
                    float depAv = (typeAvDepth[hType] + depth) * .5f;
                    typeAvDepth[hType] = depAv;
                }
                else
                    typeAvDepth.Add(hType, depth);

                //if (nightId.Length > 0)
                //{
                //    if (depthAv.ContainsKey(nightId))
                //    {
                //        float depAv = (depthAv[nightId] + depth) * .5f;
                //        depthAv[nightId] = depAv;
                //    }
                //    else
                //        depthAv.Add(nightId, depth);
                //}
            }
        }

        public static void PrintPlayerTarget()
        {
            Vector3 playerPos = GameManager.Instance.Player.ColliderCenter.position;
            Vector3 dir = GameManager.Instance.Player.transform.forward;
            RaycastHit hit;
            int layer6 = 1 << 6;
            int layer15 = 1 << 15;
            //LayerMask layerMask = layer6 | layer15;
            LayerMask layerMask = layer6;
            Message("PrintPlayerTarget ");
            if (Physics.Raycast(playerPos, dir, out hit, 111, ~layerMask, QueryTriggerInteraction.Ignore))
            {
                Message("player target " + hit.collider.gameObject.name);
                if (hit.collider.transform.parent)
                {
                    Message("player target parent " + hit.collider.gameObject.transform.parent.name);
                    if (hit.collider.transform.parent.parent)
                        Message("player target parent parent " + hit.collider.gameObject.transform.parent.parent.name);
                }
            }
        }

        public static void SetBoatWeight()
        {
            //Log("SetBoatWeight start current " + GameManager.Instance.Player.Controller.rb.mass);
            int invTotalCells = GameManager.Instance.SaveData.Inventory.GetCountNonHiddenCells();
            if (Config.cargoBoatMassMult.Value > 0)
            {
                float mass = GameManager.Instance.Player.Controller.rb.mass;
                SerializableGrid grid = GameManager.Instance.SaveData.Inventory;
                int filledCells = grid.GetFilledCells(ItemType.ALL);
          
                float ratio = (float)filledCells / (float)invTotalCells;
                GameManager.Instance.Player.Controller.rb.mass = 1 + ratio;
                //Log("SetBoatWeight start inv " + GameManager.Instance.Player.Controller.rb.mass);
            }
            if (Config.NetBoatMassMult.Value > 0)
            {
                SerializableGrid grid = GameManager.Instance.SaveData.TrawlNet;
                int filledCells = grid.GetFilledCells(ItemType.ALL);
                int netTotalCells = grid.GetCountNonHiddenCells();
                float ratio = (float)filledCells / (float)netTotalCells;
                float netInvRatio = (float)netTotalCells / (float)invTotalCells;
                ratio *= netInvRatio;
                GameManager.Instance.Player.Controller.rb.mass += ratio;
                //Log("SetBoatWeight start net " + GameManager.Instance.Player.Controller.rb.mass);
            }
            //Log("SetBoatWeight start after " + GameManager.Instance.Player.Controller.rb.mass);
        }

        public static void SetBoatWeight(SerializableGrid grid)
        {
            //float mass = GameManager.Instance.Player.Controller.rb.mass;
            if (grid == GameManager.Instance.SaveData.Inventory || grid == GameManager.Instance.SaveData.TrawlNet)
            {
                if (Config.cargoBoatMassMult.Value > 0 || Config.NetBoatMassMult.Value > 0)
                {
                    //Log("SetBoatWeight current " + GameManager.Instance.Player.Controller.rb.mass);
                    grid = GameManager.Instance.SaveData.Inventory;
                    int filledCells = grid.GetFilledCells(ItemType.ALL);
                    int invTotalCells = grid.GetCountNonHiddenCells();
                    float ratio = (float)filledCells / (float)invTotalCells;
                    ratio *= Config.cargoBoatMassMult.Value;
                    //float weight = MapToRange(ratio, 0f, 1f, 1, 2f);
                    GameManager.Instance.Player.Controller.rb.mass = 1 + ratio;
                    //Log("SetBoatWeight inv " + GameManager.Instance.Player.Controller.rb.mass);

                    grid = GameManager.Instance.SaveData.TrawlNet;
                    filledCells = grid.GetFilledCells(ItemType.ALL);
                    int netTotalCells = grid.GetCountNonHiddenCells();
                    ratio = (float)filledCells / (float)netTotalCells;
                    ratio *= Config.NetBoatMassMult.Value;
                    float netInvRatio = (float)netTotalCells / (float)invTotalCells;
                    //Log("SetBoatWeight net ratio " + netInvRatio  + " ratio " + ratio);
                    ratio *= netInvRatio;
                    GameManager.Instance.Player.Controller.rb.mass += ratio;
                }
                //Log("SetBoatWeight after " + GameManager.Instance.Player.Controller.rb.mass);
            }
        }

        public static bool IsPlayerMoving()
        { // GameManager.Instance.Player.Controller.IsMoving is true when turning boat
            return GameManager.Instance.Player.Controller.rb.velocity.magnitude > .1f;
        }

        public static void PrintFishData()
        {
            List<SpatialItemData> allFish = GameManager.Instance.ItemManager.GetSpatialItemDataBySubtype(ItemSubtype.FISH);
            allFish.Sort((x, y) => x.id.CompareTo(y.id));
            foreach (SpatialItemData sid in allFish)
            {
                FishItemData fid = sid as FishItemData;
                if (fid != null)
                {
                    if (fid.IsAberration)
                        continue;

                    StringBuilder canBeCuaghtBy = new (",");
                    if (fid.canBeCaughtByRod)
                        canBeCuaghtBy.Append(" canBeCaughtByRod,");
                    if (fid.canBeCaughtByNet)
                        canBeCuaghtBy.Append(" canBeCaughtByNet,");
                    if (fid.canBeCaughtByPot)
                        canBeCuaghtBy.Append(" canBeCaughtByPot,");

                    StringBuilder depth = new();
                    if (fid.hasMinDepth)
                        depth.Append(" minDepth " + fid.minDepth + ",");
                    if (fid.hasMaxDepth)
                        depth.Append(" maxDepth " + fid.maxDepth + ",");

                    StringBuilder minWorldPhaseRequired = new();
                    if (fid.minWorldPhaseRequired > 0)
                        minWorldPhaseRequired.Append(" minWorldPhaseRequired " + fid.minWorldPhaseRequired + ",");

                    StringBuilder aberration = new();
                    if (fid.aberrations.Count == 1)
                        aberration.Append(" has aberration,");
                    else if (fid.aberrations.Count > 1)
                        aberration.Append(" has " + fid.aberrations.Count + " aberrations,");

                    StringBuilder locationHiddenUntilCaught = new();
                    if (fid.locationHiddenUntilCaught)
                        locationHiddenUntilCaught.Append(" locationHiddenUntilCaught,");

                    StringBuilder canAppearInBaitBalls = new();
                    if (fid.canAppearInBaitBalls)
                        canAppearInBaitBalls.Append(" canAppearInBaitBalls,");

                    StringBuilder harvestableType = new();
                    if (fid.harvestableType != HarvestableType.NONE)
                        harvestableType.Append(" harvestableType " + fid.harvestableType + ",");

                    StringBuilder harvestPOICategory = new();
                    if (fid.harvestPOICategory != HarvestPOICategory.NONE)
                        harvestPOICategory.Append(" harvestPOICategory " + fid.harvestPOICategory + ",");

                    //if (!fid.CanAppearInBaitBalls)
                    //    Log(fid.id + ": " + harvestableType + " " + harvestPOICategory);
                    
                    //Log(fid.id + ": " + harvestableType + harvestPOICategory + " harvestDifficulty " + fid.harvestDifficulty + canBeCuaghtBy + depth +
                        //aberration + minWorldPhaseRequired + locationHiddenUntilCaught + canAppearInBaitBalls + " harvestItemWeight " + fid.harvestItemWeight + Environment.NewLine);
                }
            }
        }



    }
}
