using BepInEx.Bootstrap;
using System.Collections.Generic;

namespace SortedMenus
{
    internal static class ItemSortOverrider
    {
        internal static readonly Dictionary<string, string> sortOverrides = new Dictionary<string, string>();

        private static readonly string[] uniformVanillaArmorSets = new string[] { "bronze", "carapace", "iron", "fenris", "leather", "mage", "trollleather", "root" };

        internal static string GetCompareName(this ItemDrop.ItemData item)
        {
            string itemName = item.m_shared.m_name;

            if (sortOverrides.TryGetValue(itemName, out string overrideName))
            {
                return overrideName;
            }

            return itemName;
        }

        internal static void UpdateSortOverrides(List<Recipe> recipes)
        {
            sortOverrides.Clear();

            if (SortConfig.KeepArmorSetsTogether.Value != ArmorSetSorting.Disabled)
            {
                UpdateArmorSortOverrides();

                if (SortConfig.KeepArmorSetsTogether.Value == ArmorSetSorting.EnabledWithModSupport)
                {
                    UpdateModdedArmorSortOverrides(recipes);
                }
            }

            if (SortConfig.UseCustomSortOverrides.Value)
            {
                UpdateCustomSortOverrides();
            }
        }

        internal static void UpdateArmorSortOverrides()
        {
            foreach (var set in uniformVanillaArmorSets)
            {
                string chest = $"$item_chest_{set}";

                sortOverrides[$"$item_legs_{set}"] = chest;
                sortOverrides[$"$item_helmet_{set}"] = chest;
            }

            sortOverrides["$item_legs_pgreaves"] = "$item_chest_pcuirass";
            sortOverrides["$item_helmet_padded"] = "$item_chest_pcuirass";

            sortOverrides["$item_legs_wolf"] = "$item_chest_wolf";
            sortOverrides["$item_helmet_drake"] = "$item_chest_wolf";

            sortOverrides["$item_legs_rags"] = "$item_chest_rags";

            if (Chainloader.PluginInfos.ContainsKey("goldenrevolver.SimpleSetAndCapeBonuses"))
            {
                sortOverrides["$item_helmet_midsummercrown"] = "$item_chest_rags";
            }

            //if (SortConfig.KeepArmorSetsTogether.Value == ArmorSetSorting.EnabledIncludingCapes)
            {
                sortOverrides["$item_cape_deerhide"] = "$item_chest_leather";
                sortOverrides["$item_cape_trollhide"] = "$item_chest_trollleather";

                // different crafting station, but still
                sortOverrides["$item_cape_wolf"] = "$item_chest_wolf";

                // for upgrading
                sortOverrides["$item_cape_odin"] = "$item_helmet_odin";
            }
        }

        // prepare in advance, so the comparator does not need to constantly evaluate that
        private static void UpdateModdedArmorSortOverrides(List<Recipe> recipes)
        {
            foreach (var recipe in recipes)
            {
                if (recipe == null || !recipe.m_item || recipe.m_item.m_itemData == null)
                {
                    continue;
                }

                MaybeAddModdedOverride(recipe.m_item.m_itemData);
            }

            // feel free to postfix patch this to add new overrides if your naming scheme is different
        }

        private static void UpdateCustomSortOverrides()
        {
            if (Localization.instance.GetSelectedLanguage() != "English")
            {
                return;
            }

            sortOverrides["$item_barleywinebase"] = "Mead base: a Fire resistance";
            sortOverrides["$item_sword_blackmetal"] = "Black metal sword";
            sortOverrides["$item_atgeir_blackmetal"] = "Black metal atgeir";
            sortOverrides["$item_axe_blackmetal"] = "Black metal axe";
            sortOverrides["$item_bolt_blackmetal"] = "Black metal bolt";
            sortOverrides["$item_knife_blackmetal"] = "Black metal knife";

            AddOverrideWithPrefix("$item_knife_copper", "$item_bronze", "z");

            //AddOverrideWithPrefix("$item_knife_chitin", "$item_iron", "a");
            //AddOverrideWithPrefix("$item_shield_serpentscale", "$item_iron", "z");

            AddOverrideWithPrefix("$item_battleaxe", "$item_iron", "a");
            AddOverrideWithPrefix("$item_bow_huntsman", "$item_iron", "a");
            AddOverrideWithPrefix("$item_shield_banded", "$item_iron", "a");
            AddOverrideWithPrefix("$item_spear_ancientbark", "$item_iron", "a");

            AddOverrideWithPrefix("$item_battleaxe_crystal", "$item_silver", "a");
            AddOverrideWithPrefix("$item_fistweapon_fenris", "$item_silver", "a");
            AddOverrideWithPrefix("$item_spear_wolffang", "$item_silver", "a");
            AddOverrideWithPrefix("$item_mace_silver", "$item_silver", "a");
            AddOverrideWithPrefix("$item_bow_draugrfang", "$item_silver", "a");

            AddOverrideWithPrefix("$item_mace_needle", "$item_blackmetal", "z");

            AddOverrideWithPrefix("$item_atgeir_himminafl", "$item_sword_mistwalker");
            AddOverrideWithPrefix("$item_bow_snipesnap", "$item_sword_mistwalker");
            AddOverrideWithPrefix("$item_axe_jotunbane", "$item_sword_mistwalker");
            AddOverrideWithPrefix("$item_spear_carapace", "$item_sword_mistwalker");
            AddOverrideWithPrefix("$item_sledge_demolisher", "$item_sword_mistwalker");
            AddOverrideWithPrefix("$item_sword_krom", "$item_sword_mistwalker");
            AddOverrideWithPrefix("$item_knife_skollandhati", "$item_sword_mistwalker");
            AddOverrideWithPrefix("$item_crossbow_arbalest", "$item_sword_mistwalker");
        }

        private static void AddOverrideWithPrefix(string key, string prefix, string infix = "")
        {
            sortOverrides[key] = $"{prefix} {infix} {key}";
        }

        private static void MaybeAddModdedOverride(ItemDrop.ItemData item)
        {
            if (!item.IsOverridableArmor())
            {
                return;
            }

            if (item.TryGetModdedOverride(out string overrideName))
            {
                sortOverrides[item.m_shared.m_name] = overrideName;
            }
        }

        private static bool TryGetModdedOverride(this ItemDrop.ItemData item, out string moddedOverrideName)
        {
            moddedOverrideName = item.m_shared.m_name;

            if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Helmet)
            {
                if (TryReplaceWithChest(ref moddedOverrideName, "HelmetAlt", "Helmet", "HelmAlt", "Helm"))
                {
                    return true;
                }

                //UnityEngine.Debug.LogWarning($"Couldn't find partner for {itemName}");
            }
            else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Legs)
            {
                if (TryReplaceWithChest(ref moddedOverrideName, "Legs", "Greaves", "Pants"))
                {
                    return true;
                }

                //UnityEngine.Debug.LogWarning($"Couldn't find partner for {itemName}");
            }
            else if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Shoulder)
            {
                if (TryReplaceWithChest(ref moddedOverrideName, "Cape"))
                {
                    return true;
                }

                //UnityEngine.Debug.LogWarning($"Couldn't find partner for {itemName}");
            }

            return false;
        }

        internal static bool TryReplaceWithChest(ref string itemName, params string[] infixesToTest)
        {
            foreach (var infix in infixesToTest)
            {
                if (TryReplaceInfix(ref itemName, infix, "Chest"))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool TryReplaceInfix(ref string itemName, string infixToReplace, string infixToCheckFor)
        {
            bool containsUpper = itemName.Contains(infixToReplace);
            bool containsLower = itemName.Contains(infixToReplace.ToLower());

            if (!containsUpper && !containsLower)
            {
                return false;
            }

            if (containsUpper)
            {
                var potentialChestName = itemName.Replace(infixToReplace, infixToCheckFor);

                if (CheckIfTranslatable(potentialChestName))
                {
                    itemName = potentialChestName;
                    return true;
                }
            }

            // intentionally no 'else if'

            if (containsLower)
            {
                var potentialChestName = itemName.Replace(infixToReplace.ToLower(), infixToCheckFor.ToLower());

                if (CheckIfTranslatable(potentialChestName))
                {
                    itemName = potentialChestName;
                    return true;
                }
            }

            return false;
        }

        private static bool CheckIfTranslatable(string value)
        {
            if (value.IndexOf('$') != 0)
            {
                return false;
            }

            var loc = Localization.instance.Translate(value.Substring(1));

            return loc.IndexOf('[') != 0;
        }
    }
}