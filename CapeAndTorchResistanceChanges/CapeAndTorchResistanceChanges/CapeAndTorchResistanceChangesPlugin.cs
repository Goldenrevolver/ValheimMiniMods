using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;

namespace CapeAndTorchResistanceChanges
{
    [BepInIncompatibility("castix_FireIsHot")]
    [BepInIncompatibility("SSyl.ToastyTorches")]
    [BepInIncompatibility("aedenthorn.CustomArmorStats")]
    [BepInIncompatibility("goldenrevolver.TeleportWeatherWetAndColdFixes")]
    [BepInIncompatibility("goldenrevolver.TrollArmorSetBonusWithoutCape")]
    [BepInPlugin("goldenrevolver.CapeAndTorchResistanceChanges", NAME, VERSION)]
    public class CapeAndTorchResistanceChangesPlugin : BaseUnityPlugin
    {
        public const string NAME = "Cape and Torch Resistance Changes - Water and Cold Resistance";
        public const string VERSION = "1.0.3";

        public static ConfigEntry<CapeChanges> EnableCapeChanges;
        public static ConfigEntry<TorchChanges> EnableTorchChanges;

        public static ConfigEntry<bool> EnableTrollArmorSetBonusChange;
        public static ConfigEntry<WaterResistance> TrollCapeWaterResistance;
        public static ConfigEntry<ColdResistance> LoxCapeColdResistance;

        public static ConfigEntry<TeleportChange> TeleportInstantlyUpdatesWeather;
        public static ConfigEntry<bool> TeleportGrantsTemporaryWetAndColdImmunity;

        public static ConfigEntry<string> WaterModifierLabel;
        public static ConfigEntry<string> ColdModifierLabel;

        public enum CapeChanges
        {
            Disabled = 0,
            ResistanceChanges = 1,
            MovementSpeedBuffs = 2,
            Both = 3,
        }

        public enum TorchChanges
        {
            Disabled = 0,
            ResistanceChanges = 1,
            ResistanceChangesAndDurability = 2,
        }

        public enum WaterResistance
        {
            Disabled = 0,
            Resistant = 1,
            ImmuneExceptSwimming = 2,
            Immune = 3,
        }

        public enum ColdResistance
        {
            ImmuneWhileNotWet,
            Immune,
        }

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            var sectionName = "0 - Teleporting";

            TeleportInstantlyUpdatesWeather = Config.Bind(sectionName, nameof(TeleportInstantlyUpdatesWeather), TeleportChange.InstantlyUpdateWeatherAndClearWetAndColdDebuff, string.Empty);
            TeleportGrantsTemporaryWetAndColdImmunity = Config.Bind(sectionName, nameof(TeleportGrantsTemporaryWetAndColdImmunity), false, string.Empty);

            sectionName = "1 - Requires Restart";

            EnableCapeChanges = Config.Bind(sectionName, nameof(EnableCapeChanges), CapeChanges.Both, string.Empty);
            EnableTorchChanges = Config.Bind(sectionName, nameof(EnableTorchChanges), TorchChanges.ResistanceChangesAndDurability, string.Empty);

            EnableTrollArmorSetBonusChange = Config.Bind(sectionName, nameof(EnableTrollArmorSetBonusChange), true, string.Empty);
            TrollCapeWaterResistance = Config.Bind(sectionName, nameof(TrollCapeWaterResistance), WaterResistance.ImmuneExceptSwimming, string.Empty);
            LoxCapeColdResistance = Config.Bind(sectionName, nameof(LoxCapeColdResistance), ColdResistance.Immune, string.Empty);

            sectionName = "9 - Localization";

            WaterModifierLabel = Config.Bind(sectionName, nameof(WaterModifierLabel), "Water", string.Empty);
            ColdModifierLabel = Config.Bind(sectionName, nameof(ColdModifierLabel), "Cold", string.Empty);
        }
    }
}