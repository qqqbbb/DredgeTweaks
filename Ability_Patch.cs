using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tweaks
{
    internal class Ability_Patch
    {

        static BoostAbility boostAbility;


        [HarmonyPatch(typeof(HasteAbilityInfoPanel), "Show")]
        class HasteAbilityInfoPanel_Show_PostfixPatch
        {
            public static bool Prefix(HasteAbilityInfoPanel __instance)
            {
                //Util.Message("HasteAbilityInfoPanel  Show");
                return Config.showBoostGauge.Value;
            }
        }

        [HarmonyPatch(typeof(BoostAbility))]
        class BoostAbility_Patch
        {
            static float hasteHeatLossOriginal;

            [HarmonyPostfix]
            [HarmonyPatch("Awake")]
            public static void AwakePostfix(BoostAbility __instance)
            {
                //Util.Log("BoostAbility Awake hasteHeatLoss " + __instance.hasteHeatLoss);
                boostAbility = __instance;
                Config.boostCooldownMult.SettingChanged += boostCooldownMult_SettingChanged;
                hasteHeatLossOriginal = __instance.hasteHeatLoss;
                //__instance.hasteHeatLoss *= Config.boostCooldownMult.Value;
                //Util.Log("BoostAbility Awake boostMagnitude " + __instance.boostMagnitude );
            }
            [HarmonyPrefix]
            [HarmonyPatch("Activate")]
            public static bool ActivatePrefix(BoostAbility __instance)
            {
                __instance.hasteHeatLoss = hasteHeatLossOriginal * Config.boostCooldownMult.Value;
                return GameManager.Instance.Player.Controller.IsMoving;
            }
            [HarmonyPostfix]
            [HarmonyPatch("Update")]
            public static void UpdatePostfix(BoostAbility __instance)
            {
                if (Config.boostSpeedMult.Value > 1 && !__instance.isOnCooldown && __instance.isActive)
                    __instance.playerControllerRef.AbilitySpeedModifier = Config.boostSpeedMult.Value;
            }

        }

        [HarmonyPatch(typeof(PlayerAbilityManager), "GetTimeSinceLastCast")]
        class PlayerAbilityManager_Patch
        {
            public static void Postfix(PlayerAbilityManager __instance, ref float __result)
            {
                //Util.Log(" GetTimeSinceLastCast " + __result);
                if (Config.noAbilityCooldown.Value)
                    __result = float.MaxValue;
                //Util.Log(" GetTimeSinceLastCast a " + __result);
            }
        }

        private static void boostCooldownMult_SettingChanged(object sender, EventArgs e)
        {
            //SettingChangedEventArgs args = (SettingChangedEventArgs)e;
            //ConfigEntry<int> entry = (ConfigEntry<int>)sender;
            //Util.Log("BoatTurnsOnlyWhenMoving_SettingChanged " + entry.Value + " args " + args.ToString());
            if (boostAbility)
            {
                boostAbility.hasteHeatLoss = GameManager.Instance.GameConfigData.HasteHeatCooldown * Config.boostCooldownMult.Value;
                //Util.Log("boostCooldownMult_SettingChanged hasteHeatCooldown " + boostAbility.hasteHeatLoss);
            }
        }
    }
}
