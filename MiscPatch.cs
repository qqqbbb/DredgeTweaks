using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static Yarn.VirtualMachine;
using Yarn;

namespace Tweaks
{
    internal class MiscPatch
    {
        static float globalSanityModifier;

        //[HarmonyPatch(typeof(PlayerSanityUI), "Start")]
        class PlayerSanityUI_Start_PrefixPatch
        {
            public static void Postfix(PlayerSanityUI __instance)
            {
                //__instance.container.SetActive(Config.showSanityUI.Value);
            }
        }

        [HarmonyPatch(typeof(SplashController), "BeginSplashAnimation")]
        public class SplashController_BeginSplashAnimation_Patch
        {
            public static bool Prefix(SplashController __instance)
            {
                __instance.OnSplashComplete();
                return false;
            }
        }

        [HarmonyPatch(typeof(VibrationManager), "Vibrate")]
        public class VibrationManager_Vibrate_Patch
        {
            public static bool Prefix(VibrationManager __instance)
            {
                //Util.Message("controllerRumble " + Config.controllerRumble.Value);
                return Config.controllerRumble.Value;
            }
        }

        [HarmonyPatch(typeof(PlayerCamera), "Start")]
        public class PlayerCamera_Start_Patch
        {
            public static void Postfix(PlayerCamera __instance)
            {
                //playerCamera = __instance;
                //Util.Log("PlayerCamera defaultFOV " + __instance.defaultFOV);
                Config.cameraFOV.SettingChanged += CameraFOV_SettingChanged;
                __instance.defaultFOV = Config.cameraFOV.Value;
                __instance.cinemachineCamera.m_Lens.FieldOfView = __instance.defaultFOV;
            }
        }

        private static void CameraFOV_SettingChanged(object sender, EventArgs e)
        {
            //SettingChangedEventArgs args = (SettingChangedEventArgs)e;
            //ConfigEntry<int> entry = (ConfigEntry<int>)sender;
            //Util.Log("BoatTurnsOnlyWhenMoving_SettingChanged " + entry.Value + " args " + args.ToString());
            GameManager.Instance.PlayerCamera.defaultFOV = Config.cameraFOV.Value;
            GameManager.Instance.PlayerCamera.cinemachineCamera.m_Lens.FieldOfView = Config.cameraFOV.Value;
        }

        private static void SanityMult_SettingChanged(object sender, EventArgs e)
        {
            GameManager.Instance.GameConfigData.globalSanityModifier = globalSanityModifier * Config.sanityMultiplier.Value;
        }

        //[HarmonyPatch(typeof(InspectPOI), "OnEnable")]  !
        public class InspectPOI_OnEnable_Patch
        {
            public static void Postfix(InspectPOI __instance)
            {
                DisableGlint(__instance.gameObject);
            }
        }

        [HarmonyPatch(typeof(ConversationPOI), "Start")]
        class ConversationPOI_Start_PostfixPatch
        {
            public static void Postfix(ConversationPOI __instance)
            {
                DisableGlint(__instance.gameObject);
            }
        }

        [HarmonyPatch(typeof(WreckMonster), "OnEnable")]
        public class WreckMonster_OnEnable_Patch
        {
            public static void Postfix(WreckMonster __instance)
            {
                DisableGlint(__instance.gameObject);
            }
        }

        private static void DisableGlint(GameObject __instance)
        {
            if (Config.showPOIglint.Value)
                return;

            Transform InspectionGlint = __instance.transform.Find("InspectionGlint");
            if (InspectionGlint)
                InspectionGlint.gameObject.SetActive(false);
        }

        [HarmonyPatch(typeof(TimeController))]
        public class TimeController_Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch("Update")]
            public static bool UpdatePrefix(TimeController __instance)
            {
                if (Config.dayLenghtMult.Value == 1)
                    return true;
                
                if (Application.isPlaying)
                {
                    __instance.wasDaytimeHelperVar = __instance.IsDaytime;
                    Decimal num = __instance.GetTimeChangeThisFrame();
                    if (__instance.IsTimePassingForcefully())
                    {
                        __instance.timeRemainingToForcefullyPass -= num;
                        if (__instance.timeRemainingToForcefullyPass <= 0M)
                        {
                            __instance.currentTimePassageMode = TimePassageMode.NONE;
                            GameEvents.Instance.TriggerTimeForcefullyPassingChanged(false, "", __instance.currentTimePassageMode);
                        }
                    }
                    if (__instance._freezeTime)
                        num = 0M;

                    if (num > 0M)
                    {
                        num *= (Decimal)Config.dayLenghtMult.Value;
                        __instance.timeProxy.SetTimeAndDay(__instance._timeAndDay + num);
                    }
                }
                __instance._timeAndDay = __instance.timeProxy.GetTimeAndDay();
                __instance._time = __instance._timeAndDay % 1M;
                __instance._day = (int)Math.Floor(__instance._timeAndDay);
                __instance._isDaytime = __instance.Time > __instance.dawnTime && __instance.Time < __instance.duskTime;
                if (__instance.wasDaytimeHelperVar != __instance.IsDaytime && GameEvents.Instance)
                    GameEvents.Instance.TriggerDayNightChanged();

                if (__instance._lastDay < __instance.Day)
                {
                    GameEvents.Instance.TriggerDayChanged(__instance.Day);
                    __instance._lastDay = __instance.Day;
                }
                __instance.SceneLightness = __instance.RecalculateSceneLightness();
                Shader.SetGlobalFloat("_SceneLightness", __instance.SceneLightness);
                Shader.SetGlobalFloat("_TimeOfDay", __instance.Time);
                __instance.directionalLight.transform.eulerAngles = new Vector3(__instance.lightAngleMin + 360f * __instance.Time, -90f, 0.0f);
                __instance.directionalLight.color = __instance.sunColour.Evaluate(__instance.Time);
                UnityEngine.RenderSettings.ambientLight = __instance.ambientLightColor.Evaluate(__instance.Time);
                if (__instance.playerProxy)
                {
                    Vector3 playerPosition = __instance.playerProxy.GetPlayerPosition();
                    Shader.SetGlobalVector("_FogCenter", new Vector4(playerPosition.x, playerPosition.y, playerPosition.z, 0.0f));
                }
                else
                {
                    Camera current = Camera.current;
                    if (current != null)
                        Shader.SetGlobalVector("_FogCenter", new Vector4(current.transform.position.x, current.transform.position.y, current.transform.position.z, 0f));
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerSanity))]
        class PlayerSanity_PrefixPatch
        {
            //DaySanityModifier 0.4
            //NightSanityModifier -0.55
            //SleepingSanityModifier 2

            [HarmonyPostfix]
            [HarmonyPatch("Start")]
            public static void StartPostfix(PlayerSanity __instance)
            {
                Config.sanityMultiplier.SettingChanged += SanityMult_SettingChanged;
                globalSanityModifier = GameManager.Instance.GameConfigData.globalSanityModifier;
                GameManager.Instance.GameConfigData.globalSanityModifier = globalSanityModifier * Config.sanityMultiplier.Value;
            }

        }

        //[HarmonyPatch(typeof(VirtualMachine), nameof(VirtualMachine.Continue))] !
        class VirtualMachine_Patch
        {
            public static bool Prefix(VirtualMachine __instance)
            {
                //Util.Log("VirtualMachine Continue currentNodeName " + __instance.state.currentNodeName);
                //if (Config.hoodedQuestTimeLimit.Value)
                //    return true;

                if (__instance.state.currentNodeName == "HoodedFigure1_Root" || __instance.state.currentNodeName == "HoodedFigure2_Root" || __instance.state.currentNodeName == "HoodedFigure3_Root" || __instance.state.currentNodeName == "HoodedFigure4_Root")
                { }
                else
                    return true;

                __instance.CheckCanContinue();
                if (__instance.CurrentExecutionState == ExecutionState.DeliveringContent)
                {
                    __instance.CurrentExecutionState = ExecutionState.Running;
                    return false;
                }
                __instance.CurrentExecutionState = ExecutionState.Running;
                while (__instance.CurrentExecutionState == ExecutionState.Running)
                {
                    Instruction i = __instance.currentNode.Instructions[__instance.state.programCounter];
                    bool skip = false;
                    if (i.opcode_ == Instruction.Types.OpCode.CallFunc)
                    {
                        foreach (Operand operand in i.operands_)
                        {
                            if (operand.valueCase_ == Operand.ValueOneofCase.StringValue)
                            {
                                if (operand.value_.ToString() == "GetDaysSinceTemporalMarker")
                                {
                                    //Util.Log("VirtualMachine Continue skip Instruction " + i);
                                    skip = true;
                                }
                            }
                        }
                    }
                    if (!skip)
                    {
                        //Util.Log("VirtualMachine Continue run Instruction " + i);
                        __instance.RunInstruction(i);
                    }
                    __instance.state.programCounter++;
                    if (__instance.state.programCounter >= __instance.currentNode.Instructions.Count())
                    {
                        __instance.NodeCompleteHandler(__instance.currentNode.Name);
                        __instance.CurrentExecutionState = ExecutionState.Stopped;
                        __instance.DialogueCompleteHandler();
                        __instance.dialogue.LogDebugMessage("Run complete.");
                    }
                }
                return false;
            }
        }


        [HarmonyPatch(typeof(GridUI))]
        class GridUI_Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch("CreateObject")]
            public static void CreateObjectPrefix(GridUI __instance, SpatialItemInstance entry)
            {
                //Util.Log("GridUI CreateObject " + entry.id);
                //SpatialItemData itemData = entry.GetItemData<SpatialItemData>();
                //Util.Log(entry.id + " itemType " + itemData.itemType + " subtype " + itemData.itemSubtype);
                if (entry.id == "engine1")
                {
                    SpatialItemData itemData = entry.GetItemData<SpatialItemData>();
                    if (itemData)
                    {
                        itemData.canBeSoldByPlayer = true;
                        itemData.value = 180;
                        //itemData.sellOverrideValue = 180;
                        //itemData.hasSellOverride = true;
                        //fixedPecEngine = true;
                    }
                }
                else if (entry.id == "rod5")
                {
                    SpatialItemData itemData = entry.GetItemData<SpatialItemData>();
                    if (itemData)
                    {
                        itemData.canBeSoldByPlayer = true;
                        itemData.value = 300;
                    }
                }
                else if (entry.id == "quest-map-1" || entry.id == "quest-map-2" || entry.id == "quest-map-3")
                {
                    SpatialItemData itemData = entry.GetItemData<SpatialItemData>();
                    if (itemData)
                    {
                        itemData.canBeSoldByPlayer = true;
                        itemData.value = 30;
                        itemData.itemSubtype = ItemSubtype.TRINKET;
                    }
                }
            }
        }



    }
}
