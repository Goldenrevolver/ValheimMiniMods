using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;

namespace ObtainableStonePickaxe
{
    [BepInPlugin("goldenrevolver.ObtainableStonePickaxePlugin", NAME, VERSION)]
    public class ObtainableStonePickaxePlugin : BaseUnityPlugin
    {
        public const string NAME = "Obtainable Stone Pickaxe";
        public const string VERSION = "1.0.1";

        public static ConfigEntry<bool> ChangeStonePickaxeRecipe;

        public static ConfigEntry<bool> UpgradeableStonePickaxe;
        public static ConfigEntry<bool> UpgradeableAntlerPickaxe;
        public static ConfigEntry<bool> BronzePickaxeUpgradeFix;

        public static ConfigEntry<bool> EnableDebugMessages;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            //var sectionName = "0 - General";

            var sectionName = "1 - Upgrading";

            BronzePickaxeUpgradeFix = Config.Bind(sectionName, nameof(BronzePickaxeUpgradeFix), true, "Whether to fix that the bronze pickaxe gains one less pickaxe damage than pierce damage per level, unlike all other pickaxes.");
            UpgradeableAntlerPickaxe = Config.Bind(sectionName, nameof(UpgradeableAntlerPickaxe), true, "Enables upgrading of the antler pickaxe with the default upgrade formula.");
            UpgradeableStonePickaxe = Config.Bind(sectionName, nameof(UpgradeableStonePickaxe), true, "Enables upgrading of the stone pickaxe with the default upgrade formula.");

            sectionName = "9 - Debugging";

            EnableDebugMessages = Config.Bind(sectionName, nameof(EnableDebugMessages), false, "Enable this if you can't break a rock with the stone pickaxe to see why the mod thinks it's not a rock. Tell this to the mod author if you think it's a bug.");
        }

        internal static void DebugLog(string message)
        {
            UnityEngine.Debug.Log($"{NAME}: {message}");
        }
    }
}