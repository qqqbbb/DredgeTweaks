using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Tweaks
{
    internal class Boat_Patch
    {
        static public float baseMovementModifier;
        static public float baseTurnSpeed;

        public static void BoatMoveSpeed_SettingChanged(object sender, EventArgs e)
        {
            if (baseMovementModifier > 0 && GameManager.Instance && GameManager.Instance.Player && GameManager.Instance.Player._controller)
                GameManager.Instance.Player._controller._baseMovementModifier = baseMovementModifier * Config.boatSpeedMult.Value;
        }

        public static void BoatTurnSpeed_SettingChanged(object sender, EventArgs e)
        {
            if (baseTurnSpeed > 0 && GameManager.Instance && GameManager.Instance.Player && GameManager.Instance.Player._controller)
                GameManager.Instance.Player._controller._baseTurnSpeed = baseTurnSpeed * Config.boatTurnMult.Value;
        }

        [HarmonyPatch(typeof(PlayerController))]
        public class PlayerController_Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch("Start")]
            public static void StartPostfix(PlayerController __instance)
            {
                baseMovementModifier = __instance._baseMovementModifier;
                baseTurnSpeed = __instance._baseTurnSpeed;
                __instance._baseMovementModifier = baseMovementModifier * Config.boatSpeedMult.Value;
                __instance._baseTurnSpeed = baseTurnSpeed * Config.boatTurnMult.Value;
            }
            [HarmonyPrefix]
            [HarmonyPatch("FixedUpdate")]
            public static void FixedUpdatePrefix(PlayerController __instance)
            {
                //Util.Log("_baseMovementModifier " + __instance._baseMovementModifier);
                //Util.Log("_baseTurnSpeed " + __instance._baseTurnSpeed);
                if (Config.boatTurnsOnlyWhenMoving.Value)
                {
                    Vector2 input = GameManager.Instance.Input.GetValue(__instance.moveAction);
                    __instance._baseTurnSpeed = baseTurnSpeed * Mathf.Abs(input.y);
                }
            }

        }

        [HarmonyPatch(typeof(PlayerCollider), "OnCollisionEnter")]
        class PlayerCollider_OnCollisionEnter_Patch
        {
            public static bool Prefix(PlayerCollider __instance, Collision other)
            {
                //Util.Message(" OnCollisionEnter magnitude " + other.relativeVelocity.magnitude);
                if (Config.boatCollisionDamageChance.Value == 100 && Config.boatSpeedCollisionDamageChanceMult.Value == 0)
                    return true;

                bool safe = other.gameObject.CompareTag(__instance.safeColliderTag);
                if (safe == false && (__instance.iceLayer.value & 1 << other.gameObject.layer) > 0 && GameManager.Instance.SaveData.GetIsIcebreakerEquipped())
                    safe = true;

                if (Config.boatCollisionDamageChance.Value == 0)
                    safe = true;
                else
                {
                    int r = UnityEngine.Random.Range(0, 101);
                    int boatCollisionDamageChance = Config.boatCollisionDamageChance.Value;
                    if (Config.boatSpeedCollisionDamageChanceMult.Value > 0)
                    {
                        float s = other.relativeVelocity.magnitude * Config.boatSpeedCollisionDamageChanceMult.Value;
                        boatCollisionDamageChance += Mathf.RoundToInt(s);
                    }
                    if (r > boatCollisionDamageChance)
                        safe = true;
                }
                //Util.Message(" safe " + safe); 
                bool monster = other.gameObject.CompareTag(__instance.monsterTag);
                bool uniqueVib = other.gameObject.CompareTag(__instance.uniqueVibrationTag);
                __instance.ProcessHit(safe, monster, uniqueVib);
                return false;
            }
        }

        //[HarmonyPatch(typeof(PlayerCollider), "ProcessHit")]
        class PlayerCollider_ProcessHit_Patch
        {
            public static void Prefix(PlayerCollider __instance, ref bool isSafeCollider, bool isMonster, bool hasUniqueVibration)
            {
                //if (Config.boatSpeedAffectsCollisionDamage.Value == false)
                //    isSafeCollider = true;

                //Util.Message($" ProcessHit {isSafeCollider}");
            }
        }

        [HarmonyPatch(typeof(SerializableGrid))]
        class SerializableGrid_Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch("AddObjectToGridData")]
            public static void AddObjectToGridDataPatch(SerializableGrid __instance, SpatialItemInstance spatialItemInstance, Vector3Int pos, bool dispatchEvent)
            {
                //Util.Message("PickUpObject " + objectPickedUp.name);
                //Util.Log("AddObjectToGridData " + spatialItemInstance.id);
                Util.SetBoatWeight(__instance);
            }
            [HarmonyPostfix]
            [HarmonyPatch("RemoveObjectFromGridData")]
            public static void RemoveObjectFromGridDataPostfix(SerializableGrid __instance, SpatialItemInstance spatialItemInstanceToRemove, bool notify)
            {
                //Util.Message("PlaceObjectOnGrid " + o.name);
                //Util.Log("RemoveObjectFromGridData " + spatialItemInstanceToRemove.id);
                Util.SetBoatWeight(__instance);
            }
        }

        [HarmonyPatch(typeof(DockPOIHandler))]
        public static class DockPOIHandler_Patches
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(DockPOIHandler), "OnPressBegin")]
            public static void StopDockingMovement(DockPOIHandler __instance)
            {
                GameManager.Instance.Player.Controller._autoMoveSpeed = Config.boatDockSpeedMult.Value;
                GameManager.Instance.Player.Controller._lookSpeed = Config.boatDockSpeedMult.Value;
                //Util.Message($"OnPressBegin _autoMoveSpeed {GameManager.Instance.Player.Controller._autoMoveSpeed}");
                //Util.Message($"OnPressBegin _lookSpeed {GameManager.Instance.Player.Controller._lookSpeed}");
            }
        }

        private static void boatMassMult_SettingChanged(object sender, EventArgs e)
        {
            //SettingChangedEventArgs args = (SettingChangedEventArgs)e;
            //ConfigEntry<int> entry = (ConfigEntry<int>)sender;
            //Util.Log("BoatTurnsOnlyWhenMoving_SettingChanged " + entry.Value + " args " + args.ToString());
            //GameManager.Instance.Player.Controller.rb.mass = Config.boatMassMult.Value;
        }



    }
}
