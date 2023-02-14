using BepInEx.Bootstrap;
using System.Collections.Generic;

namespace SortedMenus
{
    internal class ItemSortOverrider
    {
        internal static readonly Dictionary<string, string> sortOverrides = new Dictionary<string, string>();

        internal static void UpdateSortOverrides(List<Recipe> recipes)
        {
            sortOverrides.Clear();

            if (SortConfig.KeepArmorSetsTogether.Value != ArmorSetSorting.Disabled)
            {
                UpdateArmorSortOverrides();

                if (SortConfig.KeepArmorSetsTogether.Value == ArmorSetSorting.EnabledWithModSupport)
                {
                    AddModdedArmorSortOverrides(recipes);
                }
            }

            if (SortConfig.EnableCustomSortOverrides.Value)
            {
                AddCustomSortOverrides();
            }
        }

        private static void UpdateArmorSortOverrides()
        {
            foreach (var item in OverwritePatches.baseGameArmorSortOverwrites)
            {
                sortOverrides[item.Key] = item.Value;
            }
        }

        private static void AddCustomSortOverrides()
        {
            foreach (var item in OverwritePatches.importedSortOverwrites)
            {
                AddOverrideWithPrefix(item.Key, item.Value);
            }
        }

        private static void AddOverrideWithPrefix(string key, string prefix)
        {
            sortOverrides[key] = $"{prefix} {key}";
        }

        // prepare in advance, so the comparator does not need to constantly evaluate that
        private static void AddModdedArmorSortOverrides(List<Recipe> recipes)
        {
            if (Chainloader.PluginInfos.ContainsKey("goldenrevolver.SimpleSetAndCapeBonuses"))
            {
                sortOverrides["$item_helmet_midsummercrown"] = "$item_chest_rags";
            }

            foreach (var recipe in recipes)
            {
                if (recipe == null || !recipe.m_item || recipe.m_item.m_itemData == null)
                {
                    continue;
                }

                MaybeAddModdedOverride(recipe.m_item.m_itemData);
            }
        }

        private static void MaybeAddModdedOverride(ItemDrop.ItemData item)
        {
            if (!item.IsOverridableArmor())
            {
                return;
            }

            if (TryGetModdedOverride(item, out string overrideName))
            {
                sortOverrides[item.m_shared.m_name] = overrideName;
            }
        }

        private static bool TryGetModdedOverride(ItemDrop.ItemData item, out string moddedOverrideName)
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

        private static bool TryReplaceWithChest(ref string itemName, params string[] infixesToTest)
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

        private static bool TryReplaceInfix(ref string itemName, string infixToReplace, string infixToCheckFor)
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