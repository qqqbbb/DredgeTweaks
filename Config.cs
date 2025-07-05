using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using UnityEngine;

namespace Tweaks
{
    public static class Config
    {
        public static ConfigEntry<bool> spyGlassShowsFishingSpots;
        public static ConfigEntry<bool> controllerRumble;
        public static ConfigEntry<bool> boatTurnsOnlyWhenMoving;
        public static ConfigEntry<bool> photoMode;
        public static ConfigEntry<bool> waterRipplesOnFishingSpot;
        public static ConfigEntry<bool> showPOIicon;
        public static ConfigEntry<bool> aberrationParticleFXonFishingSpot;
        public static ConfigEntry<float> aberrationCatchBonusCap;
        public static ConfigEntry<bool> netCatchSound;
        public static ConfigEntry<bool> specialFishingSpots;
        public static ConfigEntry<bool> showPlayerMarkerOnMap;
        public static ConfigEntry<bool> showOrangeParticlesOnPOI;
        public static ConfigEntry<bool> randomizeFishStock;
        public static ConfigEntry<bool> randomizeDredgeStock;
        public static ConfigEntry<bool> showPOIglint;
        public static ConfigEntry<bool> showBoostGauge;
        public static ConfigEntry<bool> showFishSpotInfo;
        public static ConfigEntry<bool> showMinigameAnimationFeedback;
        public static ConfigEntry<bool> showRelicParticles;
        public static ConfigEntry<bool> fishDecays;
        public static ConfigEntry<bool> showNetCatchCount;
        public static ConfigEntry<bool> noAbilityCooldown;
        public static ConfigEntry<int> cameraFOV;
        public static ConfigEntry<float> chanceToCatchResearchPart;
        public static ConfigEntry<float> boatSpeedMult;
        public static ConfigEntry<float> boatTurnMult;
        public static ConfigEntry<float> daytimeAberrationChance;
        public static ConfigEntry<float> nighttimeAberrationChance;
        public static ConfigEntry<float> netCatchRateMult;
        public static ConfigEntry<float> netCatchMaterialChance;
        public static ConfigEntry<float> fishingSpotDisableChance;
        public static ConfigEntry<float> boostSpeedMult;
        public static ConfigEntry<float> boostCooldownMult;
        public static ConfigEntry<float> dayLenghtMult;
        public static ConfigEntry<float> NetBoatMassMult;
        public static ConfigEntry<float> cargoBoatMassMult;
        public static ConfigEntry<float> sanityAberrationCatchBonus;
        public static ConfigEntry<float> sanityMultiplier;
        public static ConfigEntry<float> daySanityMultiplier;
        public static ConfigEntry<float> nightSanityMultiplier;
        public static ConfigEntry<float> crabPotCatchChance;
        public static ConfigEntry<float> netCatchChance;
        public static ConfigEntry<float> netDurabilityLossPerCatch;
        public static ConfigEntry<float> crabPotDurabilityMultiplier;
        public static ConfigEntry<int> boatCollisionDamageChance;
        public static ConfigEntry<float> crabPotCatchRateMult;
        public static ConfigEntry<int> minAtrophyAberrations;
        public static ConfigEntry<bool> netBreakes;
        public static ConfigEntry<FishingSpots> fishingSpots;
        public static ConfigEntry<DredgeSpots> dredgeSpots;
        public static ConfigEntry<float> boatSpeedCollisionDamageChanceMult;
        public static ConfigEntry<float> boatDockSpeedMult;

        public static AcceptableValueRange<float> zeroToTenRange = new AcceptableValueRange<float>(0.0f, 10f);
        public static AcceptableValueRange<int> zeroTo100intRange = new AcceptableValueRange<int>(0, 100);
        public static AcceptableValueRange<int> zeroToTenIntRange = new AcceptableValueRange<int>(0, 10);
        public static AcceptableValueRange<float> point1To10Range = new AcceptableValueRange<float>(0.1f, 10f);
        public static AcceptableValueRange<float> zeroToOneRange = new AcceptableValueRange<float>(0.0f, 1f);
        public static AcceptableValueRange<int> cameraFOVrange = new AcceptableValueRange<int>(20, 80);
        public static AcceptableValueRange<float> point5To5Range = new AcceptableValueRange<float>(0.5f, 5f);
        public static AcceptableValueRange<float> oneTo5Range = new AcceptableValueRange<float>(1f, 5f);
        public static AcceptableValueRange<float> oneTo10Range = new AcceptableValueRange<float>(1f, 10f);
        public static AcceptableValueRange<float> point01To3Range = new AcceptableValueRange<float>(0.01f, 3f);
        public static ConfigDefinition advMapDef;

        public static void Bind()
        {
            boatTurnsOnlyWhenMoving = Main.config.Bind<bool>("BOAT", "Boat can turn only when moving", false);
            boatSpeedMult = Main.config.Bind<float>("BOAT", "Boat movement speed multiplier", 1f, new ConfigDescription("", point5To5Range));
            boatTurnMult = Main.config.Bind<float>("BOAT", "Boat turning rate multiplier", 1f, new ConfigDescription("", point5To5Range));
            showBoostGauge = Main.config.Bind<bool>("BOAT", "Show haste ability overheat gauge", true);
            boostCooldownMult = Main.config.Bind<float>("BOAT", "Haste ability heat loss rate multiplier", 1f, new ConfigDescription("", point1To10Range));
            boostSpeedMult = Main.config.Bind<float>("BOAT", "Haste ability speed multiplier", 1f, new ConfigDescription("Your haste speed will be multiplied by this", oneTo5Range));
            boatCollisionDamageChance = Main.config.Bind<int>("BOAT", "Boat collision damage chance percent", 100, new ConfigDescription("Chance percent your boat gets damaged when colliding with things.", zeroTo100intRange));
            boatSpeedCollisionDamageChanceMult = Main.config.Bind<float>("BOAT", "Boat collision damage chance speed multiplier", 0, "Your boat speed will be multiplied by this and added to 'Boat collision damage chance percent' setting. When you start new game your boat's max speed is about 4. When you install 'engine stack' your boat max speed is about 12.");
            boatDockSpeedMult = Main.config.Bind<float>("BOAT", "Boat docking speed multiplier", 1f, new ConfigDescription("", oneTo10Range));



            cargoBoatMassMult = Main.config.Bind<float>("BOAT", "Boat cargo weight", 0.0f, new ConfigDescription("This is percent of boat cargo weight added to boat weight. When you have no free space in inventory your boat's weight will double if you set this to 100%.", zeroToOneRange));
            NetBoatMassMult = Main.config.Bind<float>("BOAT", "Trawl net weight", 0.0f, new ConfigDescription("Percent of trawl net weight added to boat weight.", zeroToOneRange));
            daytimeAberrationChance = Main.config.Bind<float>("FISHING", "Chance to catch aberrations during the day", 0.01f, new ConfigDescription("This setting does not affect chances of special fishing spots appearing in the world.", zeroToOneRange));
            nighttimeAberrationChance = Main.config.Bind<float>("FISHING", "Chance to catch aberrations at night", 0.03f, new ConfigDescription("This setting does not affect chances of special fishing spots appearing in the world.", zeroToOneRange));
            fishingSpotDisableChance = Main.config.Bind<float>("FISHING", "Chance to remove a fishing spot for a day", 0.0f, new ConfigDescription("", zeroToOneRange));
            crabPotCatchRateMult = Main.config.Bind<float>("CRAB POT", "Crab pot catch interval multiplier", 1f, new ConfigDescription("Time needed for crab pot to catch anything will be multiplied by this", point01To3Range));
            crabPotCatchChance = Main.config.Bind<float>("CRAB POT", "Crab pot catch chance", 1f, new ConfigDescription("", zeroToOneRange));
            crabPotDurabilityMultiplier = Main.config.Bind<float>("CRAB POT", "Crab pot durability multiplier", 1f, new ConfigDescription("", point1To10Range));
            fishingSpots = Main.config.Bind<FishingSpots>("FISHING", "Fishing spots", FishingSpots.Vanilla);
            fishDecays = Main.config.Bind<bool>("FISHING", "Caught fish decays", true);
            showMinigameAnimationFeedback = Main.config.Bind<bool>("FISHING", "Show fishing minigame animation feedback", true, "");
            showFishSpotInfo = Main.config.Bind<bool>("FISHING", "Show fishing spot info", true, "");
            aberrationCatchBonusCap = Main.config.Bind<float>("FISHING", "Aberration catch chance cap", 0.35f, new ConfigDescription("Your chances to catch aberations will be capped at this.", zeroToOneRange));
            aberrationParticleFXonFishingSpot = Main.config.Bind<bool>("FISHING", "Show particle effect on special fishing spots", true);
            specialFishingSpots = Main.config.Bind<bool>("FISHING", "Special fishing spots", true, "No fishing spot will give you 100% to catch aberrations if this is false.");
            minAtrophyAberrations = Main.config.Bind<int>("FISHING", "Min atrophy aberrations", 1, new ConfigDescription("Min number of aberations caught by atrohpy ability.", zeroToTenIntRange));
            waterRipplesOnFishingSpot = Main.config.Bind<bool>("FISHING", "Water ripples on fishing spot", true);
            randomizeFishStock = Main.config.Bind<bool>("FISHING", "Randomize fish stock at fishing spots", false);
            chanceToCatchResearchPart = Main.config.Bind<float>("DREDGING", "Chance to catch research part when dredging", 0.07f, new ConfigDescription("", zeroToOneRange));
            dredgeSpots = Main.config.Bind<DredgeSpots>("DREDGING", "Dredge spots", DredgeSpots.Vanilla);
            showNetCatchCount = Main.config.Bind<bool>("TRAWL NET", "Show trawl net catch count", true);
            netCatchSound = Main.config.Bind<bool>("TRAWL NET", "Trawl net catch sound", true);
            netCatchRateMult = Main.config.Bind<float>("TRAWL NET", "Trawl net catch rate multiplier", 1f, new ConfigDescription("", zeroToTenRange));
            netCatchChance = Main.config.Bind<float>("TRAWL NET", "Trawl net catch chance", 1f, new ConfigDescription("", zeroToOneRange));
            netBreakes = Main.config.Bind<bool>("TRAWL NET", "Trawl net gets damaged over time", true);
            sanityMultiplier = Main.config.Bind<float>("SANITY", "Sanity change rate multiplier", 1f, new ConfigDescription("Your sanity recovery and loss rate will be multiplied by this", point1To10Range));
            sanityAberrationCatchBonus = Main.config.Bind<float>("SANITY", "Bonus chance to catch aberrations at low sanity", 0.0f, new ConfigDescription("Bonus chance to catch aberrations that scales with your sanity. For example, if you set this to 50% your chance to catch aberrations will increase by 25% when your sanity is at 50% and by 50% when your sanity is at 0%.", zeroToOneRange));
            cameraFOV = Main.config.Bind<int>("MISC", "Camera field of view", 40, new ConfigDescription("", cameraFOVrange));
            dayLenghtMult = Main.config.Bind<float>("MISC", "Day/night length multiplier", 1f, new ConfigDescription("The higher the value the faster time passes when you move. This does not affect anything else.", point1To10Range));
            controllerRumble = Main.config.Bind<bool>("MISC", "Controller rumble", true);
            showPOIicon = Main.config.Bind<bool>("MISC", "Show point of interest icon when you are close to it", true);
            showPlayerMarkerOnMap = Main.config.Bind<bool>("MISC", "Show player marker on map", true);
            showOrangeParticlesOnPOI = Main.config.Bind<bool>("MISC", "Show orange particles on point of interest", true);
            showPOIglint = Main.config.Bind<bool>("MISC", "Show point of interest glint particle effect", true);
            spyGlassShowsFishingSpots = Main.config.Bind<bool>("MISC", "Spy glass shows fishing spots", true);
            showRelicParticles = Main.config.Bind<bool>("MISC", "Show relic beam particle effect", true, "");
            noAbilityCooldown = Main.config.Bind<bool>("MISC", "Abilities have no cooldown timer", false, "");

            //crabPotCatchRateMult = point1To10Range.MaxValue - crabPotCatchRateMult_.Value;
            //Mathf.Clamp(crabPotCatchRateMult, point1To10Range.MinValue, point1To10Range.MaxValue);
            crabPotCatchRateMult.SettingChanged += CrabPotCatchRate_SettingChanged;
        }

        static void CrabPotCatchRate_SettingChanged(object sender, EventArgs e)
        {
            //crabPotCatchRateMult = point1To10Range.MaxValue - crabPotCatchRateMult_.Value;
            //Mathf.Clamp(crabPotCatchRateMult, point1To10Range.MinValue, point1To10Range.MaxValue);
        }
    }
}
