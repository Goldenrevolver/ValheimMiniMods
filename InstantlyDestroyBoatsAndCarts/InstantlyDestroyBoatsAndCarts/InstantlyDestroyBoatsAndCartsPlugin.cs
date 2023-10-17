using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;

namespace InstantlyDestroyBoatsAndCarts
{
    [BepInPlugin("goldenrevolver.InstantlyDestroyBoatsAndCarts", NAME, VERSION)]
    public class InstantlyDestroyBoatsAndCartsPlugin : BaseUnityPlugin
    {
        public const string NAME = "Instantly Destroy Boats and Carts";
        public const string VERSION = "1.0.4";

        public static ConfigEntry<AllowDestroy> AllowDestroyFor;
        public static ConfigEntry<bool> PreventWhenContainerIsNotEmpty;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            var section = "General";

            AllowDestroyFor = Config.Bind(section, nameof(AllowDestroyFor), AllowDestroy.Both, $"Which of these you can now immediately destroy with a hammer, if {nameof(PreventWhenContainerIsNotEmpty)} and the internal checks allow it.");
            PreventWhenContainerIsNotEmpty = Config.Bind(section, nameof(PreventWhenContainerIsNotEmpty), true, "Whether the container of the boat or wagon needs to be empty to allow for it to get immediately destroyed with a hammer.");
        }

        public enum AllowDestroy
        {
            Disabled,
            OnlyBoats,
            OnlyCarts,
            Both
        }
    }
}