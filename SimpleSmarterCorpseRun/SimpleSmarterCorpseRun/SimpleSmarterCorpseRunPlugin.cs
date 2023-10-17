using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace SimpleSmarterCorpseRun
{
    public enum ItemsToCheck
    {
        JustUtilityItems,
        UtilityAndShoulderItems,
        AllEquippableItems
    }

    [BepInPlugin(GUID, NAME, VERSION)]
    public class SimpleSmarterCorpseRunPlugin : BaseUnityPlugin
    {
        public const string GUID = "goldenrevolver.SimpleSmarterCorpseRun";
        public const string NAME = "Simple Smarter Corpse Run and Tombstone";
        public const string VERSION = "1.0.0";

        public const string CustomCorpseRun = "GoldensCorpseRunWithCustomCarryWeight";

        public static ConfigEntry<bool> EnableSmartCorpseRunBuff;
        public static ConfigEntry<bool> EnableSmartTombStoneWeightCheck;
        public static ConfigEntry<bool> EnableSmartTombStoneSlotCheck;

        public static ConfigEntry<ItemsToCheck> WhichItemsToCheck;

        protected void Awake()
        {
            Helper.plugin = this;
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            var section = "General";

            EnableSmartCorpseRunBuff = Config.Bind(section, nameof(EnableSmartCorpseRunBuff), true, "Corpse Run now gives as much carrying capacity as reequipping your utility item would give and updates with every equipment change.");
            EnableSmartTombStoneWeightCheck = Config.Bind(section, nameof(EnableSmartTombStoneWeightCheck), true, "When trying to recover your tombstone, the tombstone now condiders the carrying capacity of reequipping your utility item from your tombstone.");
            EnableSmartTombStoneSlotCheck = Config.Bind(section, nameof(EnableSmartTombStoneSlotCheck), true, "When trying to recover your tombstone, the tombstone first tries to quick stack your inventory into the tombstone to free up more slots.");

            WhichItemsToCheck = Config.Bind(section, nameof(WhichItemsToCheck), ItemsToCheck.AllEquippableItems, "Only change if you have performance issues. Which items to check when calculating bonus carrying capacity. Megingjord is a utility items, Adventure backpacks and Jude's backpacks are shoulder items.");
        }
    }

    public class Helper
    {
        internal static BaseUnityPlugin plugin;

        internal static void Log(object message, bool debug = true)
        {
            if (debug)
            {
                return;
            }

            Debug.LogWarning($"{SimpleSmarterCorpseRunPlugin.NAME} {SimpleSmarterCorpseRunPlugin.VERSION}: {(message != null ? message.ToString() : "null")}");
        }
    }
}