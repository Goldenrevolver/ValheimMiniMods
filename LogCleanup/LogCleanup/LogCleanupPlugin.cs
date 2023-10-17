using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;

namespace LogCleanup
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class LogCleanupPlugin : BaseUnityPlugin
    {
        public const string GUID = "goldenrevolver.LogCleanup";
        public const string NAME = "Log Cleanup";
        public const string VERSION = "1.0.0";

        protected void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch]
    internal static class LoggingPatches
    {
        private static bool doneSuppressing = false;

        [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Start)), HarmonyPrefix, HarmonyPriority(Priority.First - 1)]
        public static void DisableSuppression()
        {
            doneSuppressing = true;
        }

        [HarmonyPatch(typeof(ZLog), nameof(ZLog.LogWarning)), HarmonyPrefix]
        public static bool SuppressBaseGameWarning(object o)
        {
            return doneSuppressing || !(o is string s && IsUnwanted(s));
        }

        private static bool IsUnwanted(string s)
        {
            switch (s)
            {
                case "Fetching PlatformPrefs 'GuiScale' before loading defaults":
                case "Set button \"CamZoomIn\" to None!":
                case "Set button \"CamZoomOut\" to None!":
                case "Missing audio clip in music respawn":
                    return true;

                default:
                    return false;
            }
        }

        private const string fuckYou = "Only custom filters can be played. Please add a custom filter or an audioclip to the audiosource (Amb_MainMenu).";

        [HarmonyPatch(typeof(BepInEx.Logging.Logger), "InternalLogEvent", new Type[] { typeof(object), typeof(LogEventArgs) }), HarmonyPrefix]
        public static bool SuppressTheSpecialLogDirectlyInBepInEx(object sender, LogEventArgs eventArgs)
        {
            return doneSuppressing || !(eventArgs.Data is string s && s == fuckYou);
        }
    }
}