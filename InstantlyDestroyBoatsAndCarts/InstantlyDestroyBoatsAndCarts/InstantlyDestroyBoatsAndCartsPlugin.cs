using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;

namespace InstantlyDestroyBoatsAndCarts
{
    [BepInPlugin("goldenrevolver.instantly_destroy_boats_and_carts", NAME, VERSION)]
    public class InstantlyDestroyBoatsAndCartsPlugin : BaseUnityPlugin
    {
        public const string NAME = "Instantly Destroy Boats And Carts";
        public const string VERSION = "1.0";

        public static ConfigEntry<AllowDestroy> AllowDetroyFor;
        public static ConfigEntry<bool> PreventWhenContainerIsNotEmpty;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            AllowDetroyFor = Config.Bind("General", nameof(AllowDetroyFor), AllowDestroy.Both, $"Which of these you can now immediately destroy with a hammer, if {nameof(PreventWhenContainerIsNotEmpty)} and the internal checks allow it.");
            PreventWhenContainerIsNotEmpty = Config.Bind("General", nameof(PreventWhenContainerIsNotEmpty), true, "Whether the container of the boat or wagon needs to be empty to allow for it to get immediately destroyed with a hammer.");
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