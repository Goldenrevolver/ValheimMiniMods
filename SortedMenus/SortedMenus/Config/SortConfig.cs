using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using static SortedMenus.ConfigurationManagerAttributes;

namespace SortedMenus
{
    internal class SortConfig
    {
        internal static ConfigEntry<bool> CheckIfItemIsInputForOvenOrCookingStation;
        internal static ConfigEntry<SortCookingMenu> CookingMenuSorting;
        internal static ConfigEntry<bool> SortFoodInAscendingOrder;
        internal static ConfigEntry<bool> SortInediblesToBottom;

        internal static ConfigEntry<SortCraftingMenu> CraftingMenuSorting;
        internal static ConfigEntry<SortHandCraftingMenu> HandCraftingMenuSorting;
        internal static ConfigEntry<ArmorSetSorting> KeepArmorSetsTogether;

        internal static ConfigEntry<SortAmmo> SortAmmo;
        internal static ConfigEntry<SortAmmo> SortAmmoAtForge;
        internal static ConfigEntry<bool> UseSmartAmmoSorting;

        internal static ConfigEntry<bool> EnableCustomSortOverrides;
        internal static ConfigEntry<bool> EnableCustomNameOverrides;

        internal static ConfigEntry<SortSkillsMenu> SkillsMenuSorting;
        internal static ConfigEntry<UpdateSkillsMenu> UpdateSkillsMenuOnChange;

        internal static ConfigEntry<bool> EnableDebugLogs;
        internal static ConfigEntry<bool> EnablePerformanceMode;

        private static ConfigFile config;

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            config = plugin.Config;

            var sectionName = "0 - Cooking Menu";

            CheckIfItemIsInputForOvenOrCookingStation = config.Bind(sectionName, nameof(CheckIfItemIsInputForOvenOrCookingStation), true, "If this is enabled, then items like bread dough will use the food values of the baked bread for the sorting, rather than being viewed as inedible.");
            CookingMenuSorting = config.Bind(sectionName, nameof(CookingMenuSorting), SortCookingMenu.ByTotalThenHealth, "'Total' means 'health + stamina' to make sorting consistent with progression.");
            SortFoodInAscendingOrder = config.Bind(sectionName, nameof(SortFoodInAscendingOrder), true, "This only applies to the food centric parameters, not the name.");
            SortInediblesToBottom = config.Bind(sectionName, nameof(SortInediblesToBottom), true, "Includes items like mead bases and fishing bait.");

            sectionName = "1 - Crafting Menus";

            CraftingMenuSorting = config.Bind(sectionName, nameof(CraftingMenuSorting), SortCraftingMenu.ByName, string.Empty);
            HandCraftingMenuSorting = config.Bind(sectionName, nameof(HandCraftingMenuSorting), SortHandCraftingMenu.OnlyWhileUsingNoCostCheat, "Whether to sort the 'by hand' crafting menu that is directly in your inventory. Since without mods it only has 4 recipes and has to get sorted every time you open your inventory, by default it's only done when using the 'no cost' cheat.");
            KeepArmorSetsTogether = config.Bind(sectionName, nameof(KeepArmorSetsTogether), ArmorSetSorting.EnabledWithModSupport, "If enabled, then armor set sorting is determined by the name of the chest piece, and the headpiece and legpiece are added around it. If applicable, the (thematic) cape is also added.");

            SortAmmo = config.Bind(sectionName, nameof(SortAmmo), SortedMenus.SortAmmo.ToTheTop, string.Empty);
            SortAmmoAtForge = config.Bind(sectionName, nameof(SortAmmoAtForge), SortedMenus.SortAmmo.ToTheBottom, "For people who don't use metal arrows.");
            UseSmartAmmoSorting = config.Bind(sectionName, nameof(UseSmartAmmoSorting), true, "Smart sort differentiates between normal ammo, fishing bait and unequippable ammo.");

            sectionName = "1.1 - Custom Name or Sort Overwrites";

            EnableCustomNameOverrides = config.Bind(sectionName, nameof(EnableCustomNameOverrides), true, "Whether to load a custom file to override the item name of some items like the 'Battleaxe' to 'Iron battleaxe' or uniforming all the 'black metal' and 'blackmetal' items to 'black metal'. Only works if the file exists for your current language.");
            EnableCustomSortOverrides = config.Bind(sectionName, nameof(EnableCustomSortOverrides), true, "Whether to load a custom file to override the sorting behavior of items like 'Frostner' to be sorted to silver gear, or the 'Porcupine' to black metal gear, or to put all mistlands weapons close to each other. Only works if the file exists for your current language.");

            sectionName = "2 - Skills Menu";

            SkillsMenuSorting = config.Bind(sectionName, nameof(SkillsMenuSorting), SortSkillsMenu.ByLevel, CustomSeeOnlyDisplay(string.Empty));
            SkillsMenuSorting.SettingChanged += SkillsMenuSettingChanged;

            UpdateSkillsMenuOnChange = config.Bind(sectionName, nameof(UpdateSkillsMenuOnChange), UpdateSkillsMenu.EnabledExceptRunSkill, HiddenDisplay(SortedMenusPlugin.HasSkillsMenuIncompatibleModInstalled, "Skills menu updates when changing gear or leveling up instead of requiring to be closed and reopened."));
            UpdateSkillsMenuOnChange.SettingChanged += SkillsMenuSettingChanged;

            // clean up config file from 1.0
            bool oldValue = false;

            if (TryGetOldConfigValue(new ConfigDefinition("0 - Cooking Menu", "CheckIfItemIsProduceInOvenOrCookingStation"), ref oldValue))
            {
                CheckIfItemIsInputForOvenOrCookingStation.Value = oldValue;
            }
            if (TryGetOldConfigValue(new ConfigDefinition("0 - Cooking Menu", "SortPotionsAndOtherInediblesToBottom"), ref oldValue))
            {
                SortInediblesToBottom.Value = oldValue;
            }
            if (TryGetOldConfigValue(new ConfigDefinition("1 - Crafting Menus", "UseCustomSortOverrides"), ref oldValue))
            {
                if (!oldValue)
                {
                    EnableCustomSortOverrides.Value = false;
                    EnableCustomNameOverrides.Value = false;
                }
            }

            sectionName = "9 - Other";

            EnableDebugLogs = config.Bind(sectionName, nameof(EnableDebugLogs), false, "Mostly shows loading logs for custom name and sorting overwrite files.");
            EnablePerformanceMode = config.Bind(sectionName, nameof(EnablePerformanceMode), true, "This speeds up the sorting by saving (caching) the result of the last sorting for the base game crafting stations.");

            config.SettingChanged += Config_SettingChanged;
        }

        private static void Config_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            SortedRecipeCaches.InvalidateCaches();
        }

        private static void SkillsMenuSettingChanged(object sender, EventArgs e)
        {
            SkillsMenuPatches.UpdateSkillsMenu(Player.m_localPlayer);
        }

        public static bool TryGetOldConfigValue<T>(ConfigDefinition configDefinition, ref T oldValue, bool removeIfFound = true)
        {
            if (!TomlTypeConverter.CanConvert(typeof(T)))
            {
                throw new ArgumentException(string.Format("Type {0} is not supported by the config system. Supported types: {1}", typeof(T), string.Join(", ", (from x in TomlTypeConverter.GetSupportedTypes() select x.Name).ToArray())));
            }

            try
            {
                var iolock = AccessTools.FieldRefAccess<ConfigFile, object>("_ioLock").Invoke(config);
                var orphanedEntries = (Dictionary<ConfigDefinition, string>)AccessTools.PropertyGetter(typeof(ConfigFile), "OrphanedEntries").Invoke(config, new object[0]);

                lock (iolock)
                {
                    if (orphanedEntries.TryGetValue(configDefinition, out string oldValueString))
                    {
                        oldValue = (T)TomlTypeConverter.ConvertToValue(oldValueString, typeof(T));

                        if (removeIfFound)
                        {
                            orphanedEntries.Remove(configDefinition);
                        }

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Helper.LogWarning($"Error getting orphaned entry: {e.StackTrace}");
            }

            return false;
        }
    }

    public enum UpdateSkillsMenu
    {
        Disabled = 0,
        Enabled = 1,
        EnabledExceptRunSkill = 2,
    }

    public enum ArmorSetSorting
    {
        Disabled = 0,
        Enabled = 1,
        EnabledWithModSupport = 2,
    }

    public enum SortHandCraftingMenu
    {
        Disabled = 0,
        OnlyWhileUsingNoCostCheat = 1,
        Enabled = 2
    }

    public enum SortCraftingMenu
    {
        Disabled = 0,
        ByName = 1,
    }

    public enum SortAmmo
    {
        NoPriority = 0,
        ToTheTop = 1,
        ToTheBottom = 2,
    }

    public enum SortSkillsMenu
    {
        Disabled = 0,
        ByName = 1,
        ByLevel = 2,
    }

    public enum SortCookingMenu
    {
        Disabled = 0,
        ByName = 1,
        ByTotalThenHealth = 10,
        ByTotalThenStamina = 11,
        ByTotalThenEitr = 12,
        ByHealthThenTotal = 20,
        ByStaminaThenTotal = 21,
        ByEitrThenTotal = 22,
    }
}