using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using System.Reflection;

namespace ObtainableBlueMushrooms
{
    [BepInPlugin("goldenrevolver.ObtainableBlueMushroomsPlugin", NAME, VERSION)]
    public class ObtainableBlueMushroomsPlugin : BaseUnityPlugin
    {
        public const string NAME = "Immersively Obtainable Blue Mushrooms";
        public const string VERSION = "1.0.3";

        internal const string plantEverything = "advize.PlantEverything";

        protected void Awake()
        {
            MushroomConfig.LoadConfig(this.Config);

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        internal static bool AllowCavePlantingChanges()
        {
            return MushroomConfig.EnablePlantingInCaves.Value && !HasPlugin(plantEverything);
        }

        internal static bool HasPlugin(string guid)
        {
            return Chainloader.PluginInfos.ContainsKey(guid);
        }
    }
}