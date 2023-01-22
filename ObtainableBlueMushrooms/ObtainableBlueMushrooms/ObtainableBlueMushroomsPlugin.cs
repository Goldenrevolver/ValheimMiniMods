using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;

namespace ObtainableBlueMushrooms
{
    [BepInPlugin("goldenrevolver.ObtainableBlueMushroomsPlugin", NAME, VERSION)]
    public class ObtainableBlueMushroomsPlugin : BaseUnityPlugin
    {
        public const string NAME = "Immersively Obtainable Blue Mushrooms";
        public const string VERSION = "1.0.1";

        public static ConfigEntry<LootChange> BatMushroomDropMethod;
        public static ConfigEntry<float> BatMushroomDropChance;
        public static ConfigEntry<bool> ReplaceMushroomsInWolfSkewer;
        public static ConfigEntry<bool> AddMushroomToOnionSoup;
        public static ConfigEntry<bool> MiniFoodRebalance;
        public static ConfigEntry<bool> EnablePlantingInCaves;
        public static ConfigEntry<PlantingTool> MushroomPlantingTool;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            var sectionName = "0 - Mushroom Planting";

            EnablePlantingInCaves = Config.Bind(sectionName, nameof(EnablePlantingInCaves), true, "Whether you can plant regrowing blue mushrooms in frost caves. This is automatically disabled when 'Plant Everything' is installed.");
            MushroomPlantingTool = Config.Bind(sectionName, nameof(MushroomPlantingTool), PlantingTool.Hammer, "Whether you plant blue mushrooms with the hammer or the cultivator.");

            // remove the value that I accidentally added in this section in version 1.0.0
            var temp = new ConfigDefinition(sectionName, "ReplaceMushroomsInWolfSkewer");
            Config.Bind(temp, true);
            Config.Remove(temp);

            sectionName = "1 - Mushroom Drop";

            BatMushroomDropMethod = Config.Bind(sectionName, nameof(BatMushroomDropMethod), LootChange.ReplaceLeatherScraps, "How to add the blue mushroom drop to newly spawned bats");
            BatMushroomDropChance = Config.Bind(sectionName, nameof(BatMushroomDropChance), 0.5f, "Chance for a newly spawned bat to drop a blue mushroom, default of 0.5 means 50%.");
            BatMushroomDropChance.SettingChanged += BatDropChance_SettingChanged;

            sectionName = "2 - Food Changes";

            ReplaceMushroomsInWolfSkewer = Config.Bind(sectionName, nameof(ReplaceMushroomsInWolfSkewer), true, "Replaces the 2 normal mushrooms with 1 blue mushroom in the wolf skewer recipe.");
            AddMushroomToOnionSoup = Config.Bind(sectionName, nameof(AddMushroomToOnionSoup), true, $"Adds the blue mushroom to the onion soup recipe. Without '{nameof(MiniFoodRebalance)}' this is a nerf.");
            MiniFoodRebalance = Config.Bind(sectionName, nameof(MiniFoodRebalance), true, "Swaps the food stats for onion soup and eyescream, then adds minor buffs to both. Also reduces the amount of greydwarf eyes needed to craft eyescream.");
        }

        private void BatDropChance_SettingChanged(object sender, System.EventArgs e)
        {
            BatMushroomDropChance.SettingChanged -= BatDropChance_SettingChanged;

            if (BatMushroomDropChance.Value < 0)
            {
                BatMushroomDropChance.Value = 0;
            }
            else if (BatMushroomDropChance.Value > 1)
            {
                BatMushroomDropChance.Value = 1;
            }

            BatMushroomDropChance.SettingChanged += BatDropChance_SettingChanged;
        }

        public static bool HasPlugin(string guid)
        {
            return Chainloader.PluginInfos.ContainsKey(guid);
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