using BepInEx;
using BepInEx.Configuration;
using System;
using static SortedMenus.ConfigurationManagerAttributes;

namespace SortedMenus
{
    internal class SortConfig
    {
        internal static ConfigEntry<bool> SortFoodInAscendingOrder;
        internal static ConfigEntry<bool> SortPotionsAndOtherInediblesToBottom;
        internal static ConfigEntry<bool> CheckIfItemIsProduceInOvenOrCookingStation;

        internal static ConfigEntry<UpdateSkillsMenu> UpdateSkillsMenuOnChange;
        internal static ConfigEntry<SortCraftingMenu> CraftingMenuSorting;
        internal static ConfigEntry<SortCookingMenu> CookingMenuSorting;
        internal static ConfigEntry<SortSkillsMenu> SkillsMenuSorting;

        internal static ConfigEntry<ArmorSetSorting> KeepArmorSetsTogether;
        internal static ConfigEntry<SortAmmo> SortAmmo;
        internal static ConfigEntry<SortAmmo> SortAmmoAtForge;

        internal static ConfigEntry<bool> UseSmartAmmoSorting;
        internal static ConfigEntry<bool> UseCustomSortOverrides;

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            var sectionName = "0 - Cooking Menu";

            CheckIfItemIsProduceInOvenOrCookingStation = plugin.Config.Bind(sectionName, nameof(CheckIfItemIsProduceInOvenOrCookingStation), true, "If this is enabled, then items like bread dough will use the food values of the baked bread for the sorting, rather than being viewed as inedible.");
            CookingMenuSorting = plugin.Config.Bind(sectionName, nameof(CookingMenuSorting), SortCookingMenu.ByTotalThenHealth, "'Total' means 'health + stamina' to make sorting consistent with progression.");
            SortFoodInAscendingOrder = plugin.Config.Bind(sectionName, nameof(SortFoodInAscendingOrder), true, "This only applies to the food centric parameters, not the name.");
            SortPotionsAndOtherInediblesToBottom = plugin.Config.Bind(sectionName, nameof(SortPotionsAndOtherInediblesToBottom), true, string.Empty);

            sectionName = "1 - Crafting Menus";

            CraftingMenuSorting = plugin.Config.Bind(sectionName, nameof(CraftingMenuSorting), SortCraftingMenu.ByName, string.Empty);
            KeepArmorSetsTogether = plugin.Config.Bind(sectionName, nameof(KeepArmorSetsTogether), ArmorSetSorting.EnabledWithModSupport, "If enabled, then armor set sorting is determined by the name of the chest piece, and the headpiece and legpiece are added around it. If applicable, the (thematic) cape is also added.");
            SortAmmo = plugin.Config.Bind(sectionName, nameof(SortAmmo), SortedMenus.SortAmmo.ToTheTop, string.Empty);
            SortAmmoAtForge = plugin.Config.Bind(sectionName, nameof(SortAmmoAtForge), SortedMenus.SortAmmo.ToTheBottom, "For people who don't use metal arrows.");

            UseSmartAmmoSorting = plugin.Config.Bind(sectionName, nameof(UseSmartAmmoSorting), true, "Smart sort differentiates between normal ammo, fishing bait and unequippable ammo.");
            UseCustomSortOverrides = plugin.Config.Bind(sectionName, nameof(UseCustomSortOverrides), true, "Whether to use a custom list of overrides to make sure items like the 'battleaxe' are sorted to iron gear, 'Frostner' is sorted to silver gear or the 'Porcupine' is sorted to black metal gear. Also puts all mistlands weapons close to each other. This is currently only available when playing in English.");

            sectionName = "2 - Skills Menu";

            SkillsMenuSorting = plugin.Config.Bind(sectionName, nameof(SkillsMenuSorting), SortSkillsMenu.ByLevel, SeeOnlyDisplay(SortedMenusPlugin.HasCombinedSkillsModInstalled, string.Empty, "Skills menu sorting from this mod is disabled in favor of the skills menu sorting of the 'Combine Skills' mod.", "Disabled in favor of 'Combine Skills' mod"));
            SkillsMenuSorting.SettingChanged += SkillsMenuSettingChanged;
            UpdateSkillsMenuOnChange = plugin.Config.Bind(sectionName, nameof(UpdateSkillsMenuOnChange), UpdateSkillsMenu.EnabledExceptRunSkill, HiddenDisplay(SortedMenusPlugin.HasCombinedSkillsModInstalled, "Skills menu updates when changing gear or leveling up instead of requiring to be closed and reopened."));
            UpdateSkillsMenuOnChange.SettingChanged += SkillsMenuSettingChanged;
        }

        private static void SkillsMenuSettingChanged(object sender, EventArgs e)
        {
            SkillsMenuPatches.UpdateSkillsMenu(Player.m_localPlayer);
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
        ByTotalThenHealth = 2,
        ByTotalThenStamina = 3,
        ByHealthThenTotal = 4,
        ByStaminaThenTotal = 5,
    }
}