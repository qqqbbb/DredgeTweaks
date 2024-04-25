using BepInEx;
using HarmonyLib;
using System;
using UnityEngine;
using BepInEx.Configuration;

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
        public static ConfigEntry<float> safeCollisionMagnitudeThreshold;
        public static ConfigEntry<float> crabPotCatchRateMult;
        public static ConfigEntry<int> minAtrophyAberrations;
        public static ConfigEntry<bool> netBreakes;




        public static ConfigEntry<FishingSpots> fishingSpots;
        public static ConfigEntry<DredgeSpots> dredgeSpots;

        public static AcceptableValueRange<float> zeroToTenRange = new (0f, 10f);
        public static AcceptableValueRange<int> zeroToTenIntRange = new (0, 10);
        public static AcceptableValueRange<float> point1To10Range = new (0.1f, 10f);
        public static AcceptableValueRange<float> zeroToOneRange = new (0f, 1f);
        //public static AcceptableValueRange<int> percentRange = new (0, 100);
        //public static AcceptableValueRange<float> percentRangeFloat = new (0, 100);
        //public static AcceptableValueRange<int> lightBuoyLightRadiusRange = new (0, 50);
        public static AcceptableValueRange<int> cameraFOVrange = new (20, 80);
        public static AcceptableValueRange<float> point5To5Range = new (0.5f, 5f);
        public static AcceptableValueRange<float> oneTo5Range = new (1f, 5f);
        public static AcceptableValueRange<float> point01To3Range = new (.01f, 3f);

        public static ConfigDefinition advMapDef;

       public static void Bind()
        {
            //photoMode = Main.config.Bind("MISC", "Photo mode", false);
            boatTurnsOnlyWhenMoving = Main.config.Bind("BOAT", "Boat can turn only when moving", false);
            boatSpeedMult = Main.config.Bind("BOAT", "Boat movement speed multiplier", 1f, new ConfigDescription("", point5To5Range));
            boatTurnMult = Main.config.Bind("BOAT", "Boat turning speed multiplier", 1f, new ConfigDescription("", point5To5Range));
            showBoostGauge = Main.config.Bind("BOAT", "Show haste ability overheat gauge", true);
            boostCooldownMult = Main.config.Bind("BOAT", "Haste ability heat loss rate multiplier", 1f, new ConfigDescription("", point1To10Range));
            boostSpeedMult = Main.config.Bind("BOAT", "Haste ability speed multiplier", 1f, new ConfigDescription("Your haste speed will be multiplied by this if this value is more then 1", oneTo5Range));
            safeCollisionMagnitudeThreshold = Main.config.Bind("BOAT", "Safe collision speed threshold", 0f, new ConfigDescription("You will not take any damage if your boat speed is below this when you collide with something.", zeroToTenRange));
            cargoBoatMassMult = Main.config.Bind("BOAT", "Boat cargo weight", 0f, new ConfigDescription("This is percent of boat cargo weight added to boat weight. When you have no free space in inventory your boat's weight will double if you set this to 100%.", zeroToOneRange));
            NetBoatMassMult = Main.config.Bind("BOAT", "Trawl net weight", 0f, new ConfigDescription("Percent of trawl net weight added to boat weight.", zeroToOneRange));

            daytimeAberrationChance = Main.config.Bind("FISHING", "Chance to catch aberrations during the day", .01f, new ConfigDescription("This setting does not affect chances of special fishing spots appearing in the world.", zeroToOneRange));
            nighttimeAberrationChance = Main.config.Bind("FISHING", "Chance to catch aberrations at night", .03f, new ConfigDescription("This setting does not affect chances of special fishing spots appearing in the world.", zeroToOneRange));

            fishingSpotDisableChance = Main.config.Bind("FISHING", "Chance to remove a fishing spot for a day", 0f, new ConfigDescription("", zeroToOneRange));
            crabPotCatchRateMult = Main.config.Bind("CRAB POT", "Crab pot catch interval multiplier", 1f, new ConfigDescription("Time needed for crab pot to catch anything will be multiplied by this", point01To3Range));
            crabPotCatchChance = Main.config.Bind("CRAB POT", "Crab pot catch chance", 1f, new ConfigDescription("", zeroToOneRange));
            crabPotDurabilityMultiplier = Main.config.Bind("CRAB POT", "Crab pot durability multiplier", 1f, new ConfigDescription("", point1To10Range));
            fishingSpots = Main.config.Bind("FISHING", "Fishing spots", FishingSpots.Vanilla);
            fishDecays = Main.config.Bind("FISHING", "Caught fish decays", true);

            showMinigameAnimationFeedback = Main.config.Bind("FISHING", "Show fishing minigame animation feedback", true, "");
            showFishSpotInfo = Main.config.Bind("FISHING", "Show fishing spot info", true, "");
            aberrationCatchBonusCap = Main.config.Bind("FISHING", "Aberration catch chance cap", .35f, new ConfigDescription("Your chances to catch aberations will be capped at this.", zeroToOneRange));

            aberrationParticleFXonFishingSpot = Main.config.Bind("FISHING", "Show particle effect on special fishing spots", true);
            specialFishingSpots = Main.config.Bind("FISHING", "Special fishing spots", true, "No fishing spot will give you 100% to catch aberrations if this is false.");
            minAtrophyAberrations = Main.config.Bind("FISHING", "Min atrophy aberrations", 1, new ConfigDescription("Min number of aberations caught by atrohpy ability.", zeroToTenIntRange));

            waterRipplesOnFishingSpot = Main.config.Bind("FISHING", "Water ripples on fishing spot", true);
            randomizeFishStock = Main.config.Bind("FISHING", "Randomize fish stock at fishing spots", false);

            //randomizeDredgeStock = Main.config.Bind("DREDGING", "Randomize material stock at dredge spots", false);
            chanceToCatchResearchPart = Main.config.Bind("DREDGING", "Chance to catch research part when dredging", .07f, new ConfigDescription("", zeroToOneRange));
            dredgeSpots = Main.config.Bind("DREDGING", "Dredge spots", DredgeSpots.Vanilla);

            showNetCatchCount = Main.config.Bind("TRAWL NET", "Show trawl net catch count", true);
            netCatchSound = Main.config.Bind("TRAWL NET", "Trawl net catch sound", true);
            netCatchRateMult = Main.config.Bind("TRAWL NET", "Trawl net catch rate multiplier", 1f, new ConfigDescription("", zeroToTenRange));
            netCatchMaterialChance = Main.config.Bind("TRAWL NET", "Chance to catch materials with trawl net", 0f, new ConfigDescription("", zeroToOneRange));
            netCatchChance = Main.config.Bind("TRAWL NET", "Trawl net catch chance", 1f, new ConfigDescription("", zeroToOneRange));
            //netDurabilityLossPerCatch = Main.config.Bind("TRAWL NET", "Trawl net durability loss per catch", 0f, new ConfigDescription("When this is greater than 0 your trawl net will not degrade with time but will lose this much durability every time it catches fish. Set this to 10 to match vannilla durability loss rate.", zeroToTenRange));
            netBreakes = Main.config.Bind("TRAWL NET", "Trawl net gets damaged over time", true);

            
            sanityMultiplier = Main.config.Bind("SANITY", "Sanity change rate multiplier", 1f, new ConfigDescription("Your sanity recovery and loss rate will be multiplied by this", point1To10Range));
            //daySanityMultiplier = Main.config.Bind("SANITY", "Daytime sanity recovery rate multiplier", 1f, new ConfigDescription("", zeroTenRange));
            //nightSanityMultiplier = Main.config.Bind("SANITY", "Nighttime sanity loss rate multiplier", 1f, new ConfigDescription("", zeroTenRange));
            sanityAberrationCatchBonus = Main.config.Bind("SANITY", "Bonus chance to catch aberrations at low sanity", 0f, new ConfigDescription("Bonus chance to catch aberrations that scales with your sanity. For example, if you set this to 50% your chance to catch aberrations will increase by 25% when your sanity is at 50% and by 50% when your sanity is at 0%.", zeroToOneRange));

            //advancedMap = Main.config.Bind("MISC", "Advanced map", false, "This allows you to leave markers on your map. You have to reload the game after changing this.");
            //lightBuoyLightRadius = Main.config.Bind("MISC", "Light buoy light radius", 0, new ConfigDescription("", lightBuoyLightRadiusRange));
            cameraFOV = Main.config.Bind("MISC", "Camera field of view", 40, new ConfigDescription("", cameraFOVrange));
            dayLenghtMult = Main.config.Bind("MISC", "Day/night length multiplier", 1f, new ConfigDescription("The higher the value the faster time passes when you move. This does not affect anything else.", point1To10Range));
            controllerRumble = Main.config.Bind("MISC", "Controller rumble", true);
            showPOIicon = Main.config.Bind("MISC", "Show point of interest icon when you are close to it", true);
            showPlayerMarkerOnMap = Main.config.Bind("MISC", "Show player marker on map", true);
            showOrangeParticlesOnPOI = Main.config.Bind("MISC", "Show orange particles on point of interest", true);
            showPOIglint = Main.config.Bind("MISC", "Show point of interest glint particle FX", true);
            //hoodedQuestTimeLimit = Main.config.Bind("MISC", "Hooded figure pursuits are time limited", true);
            spyGlassShowsFishingSpots = Main.config.Bind("MISC", "Spy glass shows fishing spots", true);
            showRelicParticles = Main.config.Bind("MISC", "Show relic beam particle FX", true, "");
            noAbilityCooldown = Main.config.Bind("MISC", "Abilities have no cooldown timer", false, "");


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
