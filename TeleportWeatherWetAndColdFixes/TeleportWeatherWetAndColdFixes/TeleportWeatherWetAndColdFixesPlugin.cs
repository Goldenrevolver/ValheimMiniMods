using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;

//using static TeleportWeatherWetAndColdFixes.TeleportHelper;

namespace TeleportWeatherWetAndColdFixes
{
    [BepInIncompatibility("goldenrevolver.CapeAndTorchResistanceChanges")]
    [BepInPlugin("goldenrevolver.TeleportWeatherWetAndColdFixes", NAME, VERSION)]
    public class TeleportWeatherWetAndColdFixesPlugin : BaseUnityPlugin
    {
        public const string NAME = "Teleport Instantly Updates Weather - Removes Wet Debuff";
        public const string VERSION = "1.0.1";

        public static ConfigEntry<TeleportChange> TeleportChange;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            var sectionName = "General";

            TeleportChange = Config.Bind(sectionName, nameof(TeleportChange), TeleportWeatherWetAndColdFixes.TeleportChange.InstantlyUpdateWeatherAndClearWetAndColdDebuff, string.Empty);
        }
    }
}