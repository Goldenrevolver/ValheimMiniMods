using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static ResizableBossTrophies.ConfigurationManagerAttributes;

namespace ResizableBossTrophies
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class ResizableBossTrophiesPlugin : BaseUnityPlugin
    {
        public const string GUID = "goldenrevolver.ResizableBossTrophies";
        public const string NAME = "Resizable Boss Trophies and more";
        public const string VERSION = "1.1.0";

        public static ConfigEntry<float> EikthyTrophySize;
        public static ConfigEntry<float> ElderTrophySize;
        public static ConfigEntry<float> BonemassTrophySize;
        public static ConfigEntry<float> ModerTrophySize;
        public static ConfigEntry<float> YagluthTrophySize;
        public static ConfigEntry<float> QueenTrophySize;

        public static ConfigEntry<float> TrollTrophySize;
        public static ConfigEntry<float> AbominationTrophySize;
        public static ConfigEntry<float> LeechTrophySize;
        public static ConfigEntry<float> WraithTrophySize;
        public static ConfigEntry<float> SerpentTrophySize;
        public static ConfigEntry<float> StoneGolemTrophySize;
        public static ConfigEntry<float> FulingBerserkerTrophySize;
        public static ConfigEntry<float> LoxTrophySize;
        public static ConfigEntry<float> GjallTrophySize;
        public static ConfigEntry<float> SeekerTrophySize;
        public static ConfigEntry<float> SeekerSoldierTrophySize;

        private static ConfigSync serverSyncInstance;
        public static ConfigEntry<bool> UseServerSync;

        private static bool shouldUpdateVisuals = false;
        private static Coroutine updateVisualsCoroutine = null;
        private static readonly HashSet<string> allAffectedTrophies = new HashSet<string>();

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            // disable saving while we add config values, so it doesn't save to file on every change, then enable it again
            Config.SaveOnConfigSet = false;

            LoadConfigInternal();

            Config.Save();
            Config.SaveOnConfigSet = true;

            Config.SettingChanged += (a, b) => { shouldUpdateVisuals = true; };

            if (updateVisualsCoroutine == null)
            {
                updateVisualsCoroutine = this.StartCoroutine(UpdateVisualsOnTimer());
            }
        }

        private void LoadConfigInternal()
        {
            serverSyncInstance = ServerSyncWrapper.CreateOptionalConfigSync(GUID, NAME, VERSION);

            var sectionName = "0 - Boss Trophies";

            int order = 10;

            UseServerSync = Config.BindSyncLocker(serverSyncInstance, sectionName, nameof(UseServerSync), false, SimpleOrderOverride(order--));

            EikthyTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(EikthyTrophySize), 1.2f, SimpleOrderOverride(order--));
            allAffectedTrophies.Add("$item_trophy_eikthyr");

            ElderTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(ElderTrophySize), 1.5f, SimpleOrderOverride(order--));
            allAffectedTrophies.Add("$item_trophy_elder");

            BonemassTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(BonemassTrophySize), 0.9f, SimpleOrderOverride(order--));
            allAffectedTrophies.Add("$item_trophy_bonemass");

            ModerTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(ModerTrophySize), 1.2f, SimpleOrderOverride(order--));
            allAffectedTrophies.Add("$item_trophy_dragonqueen");

            YagluthTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(YagluthTrophySize), 1f, SimpleOrderOverride(order--));
            allAffectedTrophies.Add("$item_trophy_goblinking");

            QueenTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(QueenTrophySize), 0.8f, SimpleOrderOverride(order--));
            allAffectedTrophies.Add("$item_trophy_seekerqueen");

            //sectionName = "1 - Meadows";

            sectionName = "2 - Black Forest";

            TrollTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(TrollTrophySize), 1f, string.Empty);
            allAffectedTrophies.Add("$item_trophy_troll");

            sectionName = "3 - Swamp";

            AbominationTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(AbominationTrophySize), 0.8f, string.Empty);
            allAffectedTrophies.Add("$item_trophy_abomination");

            LeechTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(LeechTrophySize), 0.7f, string.Empty);
            allAffectedTrophies.Add("$item_trophy_leech");

            WraithTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(WraithTrophySize), 0.7f, string.Empty);
            allAffectedTrophies.Add("$item_trophy_wraith");

            sectionName = "4 - Mountains";

            StoneGolemTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(StoneGolemTrophySize), 0.7f, string.Empty);
            allAffectedTrophies.Add("$item_trophy_sgolem");

            sectionName = "5 - Plains";

            FulingBerserkerTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(FulingBerserkerTrophySize), 0.8f, string.Empty);
            allAffectedTrophies.Add("$item_trophy_goblinbrute");

            LoxTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(LoxTrophySize), 0.7f, string.Empty);
            allAffectedTrophies.Add("$item_trophy_lox");

            sectionName = "6 - Mistlands";

            GjallTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(GjallTrophySize), 0.7f, string.Empty);
            allAffectedTrophies.Add("$item_trophy_gjall");

            SeekerTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(SeekerTrophySize), 0.7f, string.Empty);
            allAffectedTrophies.Add("$item_trophy_seeker");

            SeekerSoldierTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(SeekerSoldierTrophySize), 0.9f, string.Empty);
            allAffectedTrophies.Add("$item_trophy_seeker_brute");

            //sectionName = "7 - Deep North";

            //sectionName = "8 - Ashlands";

            sectionName = "9 - Ocean";

            SerpentTrophySize = Config.BindSynced(serverSyncInstance, sectionName, nameof(SerpentTrophySize), 1f, string.Empty);
            allAffectedTrophies.Add("$item_trophy_serpent");
        }

        private IEnumerator UpdateVisualsOnTimer()
        {
            while (true)
            {
                if (!shouldUpdateVisuals || !Player.m_localPlayer)
                {
                    yield return new WaitForSeconds(4f);
                    continue;
                }

                shouldUpdateVisuals = false;
                var itemStands = FindObjectsOfType<ItemStand>();

                foreach (var item in itemStands)
                {
                    if (!item || !allAffectedTrophies.Contains(item.m_currentItemName))
                    {
                        continue;
                    }

                    DestroyImmediate(item.m_visualItem);
                }

                yield return null;

                foreach (var item in itemStands)
                {
                    if (!item || !allAffectedTrophies.Contains(item.m_currentItemName))
                    {
                        continue;
                    }

                    // causes the base game 'UpdateVisuals' call
                    item.m_visualName = string.Empty;
                }
            }
        }
    }
}