using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;

namespace VegetarianTurnipSoup
{
    [BepInPlugin("goldenrevolver.VegetarianTurnipSoup", NAME, VERSION)]
    public class VegetarianTurnipSoupPlugin : BaseUnityPlugin
    {
        public const string NAME = "Vegetarian Turnip Soup";
        public const string VERSION = "1.0";

        public static ConfigEntry<string> TurnipSoupName;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            var sectionName = "General";

            TurnipSoupName = Config.Bind(sectionName, nameof(TurnipSoupName), "Turnip soup", "You can translate the name here.");
        }
    }
}