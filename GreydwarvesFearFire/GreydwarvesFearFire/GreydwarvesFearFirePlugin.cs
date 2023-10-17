using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using System.Collections;
using System.Reflection;
using UnityEngine;
using static GreydwarvesFearFire.FearFireConfig;

namespace GreydwarvesFearFire
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class GreydwarvesFearFirePlugin : BaseUnityPlugin
    {
        public const string GUID = "goldenrevolver.GreydwarvesFearFire";
        public const string NAME = "Greydwarves Fear Fire";
        public const string VERSION = "1.0.1";

        // TODO all weapons that deal fire damage inflict fear
        // TODO fire arrows?
        // TODO player burning creates fear
        // TODO enemy burning creates fear

        protected void Awake()
        {
            GlobalVars.plugin = this;

            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            serverSyncInstance = ServerSyncWrapper.CreateRequiredConfigSync(GUID, NAME, VERSION);

            var sectionName = "0 - General";

            RequireFireWeakness = Config.BindSynced(serverSyncInstance, sectionName, nameof(RequireFireWeakness), FireWeakness.UsuallyWeakToFire, "'Currently weak to fire' requires checks every AI update, so it may lower frame rate on weak PCs or highly modded games.");
            RequireKillingTheElder = Config.BindSynced(serverSyncInstance, sectionName, nameof(RequireKillingTheElder), true);
            UseServerSync = Config.BindForceEnabledSyncLocker(serverSyncInstance, sectionName, nameof(UseServerSync));

            sectionName = "1 - Base Game";

            GreydwarfBruteFearLevel = Config.BindSynced(serverSyncInstance, sectionName, nameof(GreydwarfBruteFearLevel), FearLevel.Afraid);
            GreydwarfFearLevel = Config.BindSynced(serverSyncInstance, sectionName, nameof(GreydwarfFearLevel), FearLevel.Afraid);
            GreydwarfShamanFearLevel = Config.BindSynced(serverSyncInstance, sectionName, nameof(GreydwarfShamanFearLevel), FearLevel.Afraid);
            GreylingFearLevel = Config.BindSynced(serverSyncInstance, sectionName, nameof(GreylingFearLevel), FearLevel.Afraid);

            sectionName = "2 - Modded";

            ModdedBossGreydwarfFearLevel = Config.BindSynced(serverSyncInstance, sectionName, nameof(ModdedBossGreydwarfFearLevel), FearLevel.NoFear);
            ModdedGreydwarfFearLevel = Config.BindSynced(serverSyncInstance, sectionName, nameof(ModdedGreydwarfFearLevel), FearLevel.Afraid);

            Config.SettingChanged += Config_SettingChanged;
        }

        private void Config_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            GlobalVars.RequiresFearAIUpdate = true;
        }
    }

    internal static class GlobalVars
    {
        internal const string defeatedElderKey = "defeated_gdking";
        internal static BaseUnityPlugin plugin;
        private static bool _requiresUpdate = false;
        private static Coroutine updateCoroutine = null;

        internal static bool WaitingForFirstCreatureUpdate { get; set; } = false;

        internal static bool RequiresFearAIUpdate
        {
            get => _requiresUpdate;
            set
            {
                if (value && updateCoroutine == null)
                {
                    updateCoroutine = plugin.StartCoroutine(TurnOffUpdate());
                }
                _requiresUpdate = value;
            }
        }

        internal static IEnumerator TurnOffUpdate()
        {
            Log("Starting fear update, waiting");

            WaitingForFirstCreatureUpdate = true;

            while (WaitingForFirstCreatureUpdate)
            {
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(1f);

            RequiresFearAIUpdate = false;
            updateCoroutine = null;

            Log("Finished fear update");
        }

        internal static void Log(string message, bool debug = true)
        {
            // dirty way of disabling my testing logs for release
            if (debug)
            {
                return;
            }

            Debug.Log($"{GreydwarvesFearFirePlugin.NAME} {GreydwarvesFearFirePlugin.VERSION}: {(message != null ? message.ToString() : "null")}");
        }
    }

    internal class FearFireConfig
    {
        internal static ConfigSync serverSyncInstance;

        public static ConfigEntry<bool> UseServerSync;
        public static ConfigEntry<bool> RequireKillingTheElder;
        public static ConfigEntry<FireWeakness> RequireFireWeakness;

        public static ConfigEntry<FearLevel> GreylingFearLevel;
        public static ConfigEntry<FearLevel> GreydwarfFearLevel;
        public static ConfigEntry<FearLevel> GreydwarfShamanFearLevel;
        public static ConfigEntry<FearLevel> GreydwarfBruteFearLevel;
        public static ConfigEntry<FearLevel> ModdedGreydwarfFearLevel;
        public static ConfigEntry<FearLevel> ModdedBossGreydwarfFearLevel;
    }

    internal enum FireWeakness
    {
        NotRequired,
        CurrentlyWeakToFire,
        UsuallyWeakToFire,
    }

    internal enum FearLevel
    {
        NoFear,
        Avoid,
        Afraid,
    }
}