using BepInEx.Configuration;
using static ObtainableBlueMushrooms.ConfigurationManagerAttributes;

namespace ObtainableBlueMushrooms
{
    internal class MushroomConfig
    {
        public static ConfigEntry<LootChange> BatMushroomDropMethod;
        public static ConfigEntry<float> BatMushroomDropChance;
        public static ConfigEntry<bool> ReplaceMushroomsInWolfSkewer;
        public static ConfigEntry<bool> AddMushroomToOnionSoup;
        public static ConfigEntry<bool> MiniFoodRebalance;
        public static ConfigEntry<bool> EnablePlantingInCaves;
        public static ConfigEntry<PlantingTool> MushroomPlantingTool;

        internal static void LoadConfig(ConfigFile config)
        {
            var sectionName = "0 - Mushroom Planting";

            EnablePlantingInCaves = config.Bind(sectionName, nameof(EnablePlantingInCaves), true, CustomSeeOnlyDisplay());
            MushroomPlantingTool = config.Bind(sectionName, nameof(MushroomPlantingTool), PlantingTool.Hammer, CustomHiddenDisplay("Whether you plant blue mushrooms with the hammer or the cultivator."));

            // remove the value that I accidentally added in this section in version 1.0.0
            var temp = new ConfigDefinition(sectionName, "ReplaceMushroomsInWolfSkewer");
            config.Bind(temp, true);
            config.Remove(temp);

            sectionName = "1 - Mushroom Drop";

            BatMushroomDropMethod = config.Bind(sectionName, nameof(BatMushroomDropMethod), LootChange.ReplaceLeatherScraps, "How to add the blue mushroom drop to newly spawned bats");
            BatMushroomDropChance = config.Bind(sectionName, nameof(BatMushroomDropChance), 0.5f, new ConfigDescription("Chance for a newly spawned bat to drop a blue mushroom, default of 0.5 means 50%.", new AcceptableValueRange<float>(0f, 1f)));

            sectionName = "2 - Food Changes";

            ReplaceMushroomsInWolfSkewer = config.Bind(sectionName, nameof(ReplaceMushroomsInWolfSkewer), true, "Replaces the 2 normal mushrooms with 1 blue mushroom in the wolf skewer recipe.");
            AddMushroomToOnionSoup = config.Bind(sectionName, nameof(AddMushroomToOnionSoup), true, $"Adds the blue mushroom to the onion soup recipe. Without '{nameof(MiniFoodRebalance)}' this is a nerf.");
            MiniFoodRebalance = config.Bind(sectionName, nameof(MiniFoodRebalance), true, "Swaps the food stats for onion soup and eyescream, then adds minor buffs to both. Also reduces the amount of greydwarf eyes needed to craft eyescream.");
        }

        public enum LootChange
        {
            Disabled,
            AddToExistingDrops,
            ReplaceLeatherScraps
        }

        public enum PlantingTool
        {
            Hammer,
            Cultivator
        }
    }
}