using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using static SneakCameraAndCollision.SneakConfig;

namespace SneakCameraAndCollision
{
    [BepInPlugin("goldenrevolver.SneakCameraAndCollision", NAME, VERSION)]
    public class SneakCameraAndCollisionPlugin : BaseUnityPlugin
    {
        public const string NAME = "Sneak Camera and Collision Height";
        public const string VERSION = "1.0.1";

        // maybe handle uncroaching (even if the value ranges and base game behavior already handle it quite well)

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            var sectionName = "Camera Height";

            LowerCameraWhen = Config.Bind(sectionName, nameof(LowerCameraWhen), CrouchPosition.Crouching, "When to lower the camera height (crouching means both standing and moving)");
            CameraHeightReduction = Config.Bind(sectionName, nameof(CameraHeightReduction), 0.5f, ConfigurationManagerAttributes.ShowAsPercentOverride(false, new AcceptableValueRange<float>(CameraMin, CameraMax), "By how much to lower the camera (in meters)"));

            sectionName = "Collision Height";

            ChangeCollisionCrouchHeight = Config.Bind(sectionName, nameof(ChangeCollisionCrouchHeight), true, "Whether to change the player collision height at all");
            CrouchHeightMultiplier = Config.Bind(sectionName, nameof(CrouchHeightMultiplier), 0.8f, ConfigurationManagerAttributes.ShowAsPercentOverride(false, new AcceptableValueRange<float>(HeightMultMin, HeightMultMax), "By how many percent to reduce the default standing player height (which by default is 1.85 meters)"));
            CrouchHeightMultiplier.SettingChanged += (a, b) => CollisionPatches.CalculateCrouchedSize();
        }
    }

    internal class SneakConfig
    {
        public static ConfigEntry<CrouchPosition> LowerCameraWhen;
        public static ConfigEntry<float> CameraHeightReduction;
        internal const float CameraMin = 0f;
        internal const float CameraMax = 1f;

        public static ConfigEntry<bool> ChangeCollisionCrouchHeight;
        public static ConfigEntry<float> CrouchHeightMultiplier;
        internal const float HeightMultMin = 0.75f;
        internal const float HeightMultMax = 1f;

        public enum CrouchPosition
        {
            Disabled,
            Crouching,
            CrouchStanding,
            CrouchWalking
        }
    }
}