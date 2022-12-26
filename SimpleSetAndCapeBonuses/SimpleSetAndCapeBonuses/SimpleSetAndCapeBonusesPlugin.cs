using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;

namespace SimpleSetAndCapeBonuses
{
    [BepInPlugin("goldenrevolver.SimpleSetAndCapeBonuses", NAME, VERSION)]
    public class SimpleSetAndCapeBonusesPlugin : BaseUnityPlugin
    {
        public const string NAME = "Simple New Set and Cape Bonuses";
        public const string VERSION = "1.0";

        public static ConfigEntry<bool> EnableLeatherArmorSetBonus;
        public static ConfigEntry<bool> EnableForagerArmorSetBonus;
        public static ConfigEntry<bool> EnableTrollArmorSetBonusChange;
        public static ConfigEntry<bool> EnableCapeBuffs;

        public static ConfigEntry<bool> EnableSetBonusIcons;
        public static ConfigEntry<bool> AlwaysEnableMidsummerCrownRecipe;

        public static ConfigEntry<bool> ForagerSetBonusUsesRandomness;

        public static ConfigEntry<string> LeatherArmorSetBonusLabel;
        public static ConfigEntry<string> LeatherArmorSetBonusTooltip;
        public static ConfigEntry<string> ForagerSetBonusLabel;
        public static ConfigEntry<string> ForagerSetBonusTooltip;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            var sectionName = "0 - Requires Restart";

            EnableTrollArmorSetBonusChange = Config.Bind(sectionName, nameof(EnableTrollArmorSetBonusChange), true, string.Empty);
            EnableLeatherArmorSetBonus = Config.Bind(sectionName, nameof(EnableLeatherArmorSetBonus), true, string.Empty);
            EnableForagerArmorSetBonus = Config.Bind(sectionName, nameof(EnableForagerArmorSetBonus), true, string.Empty);
            EnableCapeBuffs = Config.Bind(sectionName, nameof(EnableCapeBuffs), true, string.Empty);

            EnableSetBonusIcons = Config.Bind(sectionName, nameof(EnableSetBonusIcons), true, string.Empty);
            AlwaysEnableMidsummerCrownRecipe = Config.Bind(sectionName, nameof(AlwaysEnableMidsummerCrownRecipe), true, string.Empty);

            sectionName = "1 - No Restart Required";

            ForagerSetBonusUsesRandomness = Config.Bind(sectionName, nameof(ForagerSetBonusUsesRandomness), true, "Whether Forager always grants 1 more item, or 0 to 2 with an average of 1.");

            sectionName = "9 - Localization";

            LeatherArmorSetBonusLabel = Config.Bind(sectionName, nameof(LeatherArmorSetBonusLabel), "Quick Feet", string.Empty);
            LeatherArmorSetBonusTooltip = Config.Bind(sectionName, nameof(LeatherArmorSetBonusTooltip), "Your legs carry you faster and further.", string.Empty);
            ForagerSetBonusLabel = Config.Bind(sectionName, nameof(ForagerSetBonusLabel), "Forager", string.Empty);
            ForagerSetBonusTooltip = Config.Bind(sectionName, nameof(ForagerSetBonusTooltip), "Your foraging skills are improved.", string.Empty);
        }
    }
}