using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using System.Reflection;
using UnityEngine;

namespace BossAltarRuleOfThree
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class BossAltarRuleOfThreePlugin : BaseUnityPlugin
    {
        public const string GUID = "goldenrevolver.BossAltarRuleOfThree";
        public const string NAME = "Boss Altar - Rule of Three";
        public const string VERSION = "1.0.0";

        internal static ConfigSync serverSyncInstance;
        internal static ConfigEntry<bool> UseServerSync;

        protected void Awake()
        {
            Helper.plugin = this;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            serverSyncInstance = ServerSyncWrapper.CreateRequiredConfigSync(GUID, NAME, VERSION);

            UseServerSync = Config.BindHiddenForceEnabledSyncLocker(serverSyncInstance, "General", nameof(UseServerSync));
        }
    }

    public class Helper
    {
        internal static BaseUnityPlugin plugin;

        internal static void Log(string message, bool debug = true)
        {
            // dirty way of disabling my testing logs for release
            if (debug)
            {
                return;
            }

            Debug.Log($"{BossAltarRuleOfThreePlugin.NAME} {BossAltarRuleOfThreePlugin.VERSION}: {(message != null ? message.ToString() : "null")}");
        }
    }
}