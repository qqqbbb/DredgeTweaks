using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tweaks
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class Main : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "qqqbbb.dredge.tweaks";
        public const string PLUGIN_NAME = "Tweaks";
        public const string PLUGIN_VERSION = "1.6.0";

        public static ConfigFile config;
        public static ManualLogSource logger;
        public static CommandTerminal.Terminal terminal;

        private void Awake()
        {
            Harmony harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll();
            config = this.Config;
            Tweaks.Config.Bind();
            logger = Logger;
            Tweaks.Config.boatSpeedMult.SettingChanged += Boat_Patch.BoatMoveSpeed_SettingChanged;
            Tweaks.Config.boatTurnMult.SettingChanged += Boat_Patch.BoatTurnSpeed_SettingChanged;
            Tweaks.Config.boatTurnsOnlyWhenMoving.SettingChanged += Boat_Patch.BoatTurnSpeed_SettingChanged;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} {PLUGIN_VERSION} is loaded");
        }

        private void Start()
        {
            Logger.LogInfo("tweaks Start ");
            if (ApplicationEvents.Instance == null)
            {
                Logger.LogInfo("ApplicationEvents.Instance == null");
                return;
            }
            if (GameManager.Instance == null)
            {
                Logger.LogInfo("GameManager.Instance == null");
                return;
            }
            //GameManager.Instance._buildInfo.advancedMap = Tweaks.Config.advancedMap.Value;
            //ApplicationEvents.Instance.TriggerBuildInfoChanged();
            ApplicationEvents.Instance.OnGameUnloaded += new Action(this.OnGameUnloaded);
            ApplicationEvents.Instance.OnGameStartable += new Action(this.OnGameStarted);
            //EnableTerminal();
        }

        private void OnGameUnloaded()
        {
            Logger.LogInfo("OnGameUnloaded");
        }

        private void OnGameStarted()
        {
            Logger.LogInfo("OnGameStarted");
            //config.Remove(Tweaks.Config.advMapDef);
            Util.SetBoatWeight();
            //if (Tweaks.Config.randomFishSpotSpawnRate.Value > 0)
            {
                //if (!RandomFishSpotManager.placingFishSpots)
                {
                    //RandomFishSpotManager.placingFishSpots = true;
                    //GameManager.Instance.StartCoroutine(RandomFishSpotManager.PlaceFishSpots());
                }
                //string id = __instance.harvestPOIData.id;
                //GameManager.Instance.SaveData.harvestSpotStocks[id] = 0;
            }
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Z))
            {
                //int day = GameManager.Instance.Time._day;
                //float mass = GameManager.Instance.Player.Controller.rb.mass;
                //float sanity = GameManager.Instance.Player.Sanity._sanity;
                //bool timePassing = GameManager.Instance.Time.IsTimePassing();
                //int WorldPhase = GameManager.Instance.SaveData.WorldPhase;
                //ZoneEnum currentPlayerZone = GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone();
                //bool playerMoving = GameManager.Instance.Player.Controller.IsMoving;
                //Util.Message("sanity " + sanity.ToString("0.00"));
                //Util.Message("IsPlayerMoving " + Util.IsPlayerMoving()); 
                //var sorted = new SortedDictionary<string, int>(Testing.fishCount);
                //var ordered = Testing.fishCount.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                //foreach (var kvp in ordered)
                //    Util.Log(kvp.Key + " " + kvp.Value);

                //List<SpatialItemData> allFish = GameManager.Instance.ItemManager.GetSpatialItemDataBySubtype(ItemSubtype.FISH);
                //foreach (SpatialItemData fish in allFish)
                //{
                //    string id = fish.id;
                //    if (!ordered.ContainsKey(id))
                //        Util.Log("no fish spot " + id);
                //}

                //List<FishItemData> list1 = GameManager.Instance.ItemManager.GetSpatialItemDataBySubtype(ItemSubtype.FISH).OfType<FishItemData>().ToList().Where((i => !i.IsAberration && i.CanAppearInBaitBalls && i.canBeCaughtByRod && i.zonesFoundIn.HasFlag((Enum)currentPlayerZone) && GameManager.Instance.PlayerStats.HarvestableTypes.Contains(i.harvestableType))).ToList();

                //RandomFishSpotManager.PlaceFishSpotNearPlayer();
                //Vector3 pos = GameManager.Instance.Player.transform.forward * 10f + GameManager.Instance.Player.transform.position;
                //pos = new Vector3(pos.x, GameManager.Instance.Player.transform.position.x, pos.z);
                //Util.Log("Player.pos " + GameManager.Instance.Player.transform.position);
                //Util.Log("Player.transform.forward " + GameManager.Instance.Player.transform.forward);
                //Util.Log("pos " + pos);
                //Util.Log("currentDepth " + currentDepth);
                //Util.Log("playerDepth " + playerDepth);
                //Util.Message("" + formattedDepthString);
                //Util.Message("depthAv " + HarvestPatch.depthAv.Count);
                //foreach (KeyValuePair<string, float> kvp in HarvestPatch.depthAv)
                //{
                //    string depthString = GameManager.Instance.ItemManager.GetFormattedDepthString(kvp.Value);
                //    Util.Log(kvp.Key + " av depth " + depthString);
                //}
            }
            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    Util.PrintPlayerPos();
            //}
            //if (Input.GetKeyDown(KeyCode.C))
            //{
            //    Util.PrintPlayerTarget();
            //}
            //if (Input.GetKeyDown(KeyCode.P))
            //{
            //    if (GameManager.Instance.IsPaused)
            //        GameManager.Instance.UnpauseLite();
            //    else
            //        GameManager.Instance.PauseLite();
            //}



        }



    }
}
