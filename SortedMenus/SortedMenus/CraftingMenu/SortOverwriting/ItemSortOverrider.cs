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

            // add base game armor sort overwrites first
            if (SortConfig.KeepArmorSetsTogether.Value != ArmorSetSorting.Disabled)
            {
                AddBaseGameArmorSortOverwrites();
            }

            // then custom sort overwrites are allowed to overwrite it
            if (SortConfig.EnableCustomSortOverrides.Value)
            {
                AddCustomSortOverrides();
            }

            // then we only try to add automatic modded sort overwrites if there is no overwrite already
            if (SortConfig.KeepArmorSetsTogether.Value == ArmorSetSorting.EnabledWithModSupport)
            {
                MaybeAddModdedArmorSortOverrides(recipes);
            }
        }

        private static void AddBaseGameArmorSortOverwrites()
        {
            foreach (var item in OverwritePatches.baseGameArmorSortOverwrites)
            {
                sortOverrides[item.Key] = item.Value;
            }

            if (SortConfig.KeepArmorSetsTogether.Value == ArmorSetSorting.EnabledWithModSupport)
            {
                if (Chainloader.PluginInfos.ContainsKey("goldenrevolver.SimpleSetAndCapeBonuses"))
                {
                    sortOverrides["$item_helmet_midsummercrown"] = "$item_chest_rags";
                }
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
        private static void MaybeAddModdedArmorSortOverrides(List<Recipe> recipes)
        {
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

            if (sortOverrides.ContainsKey(item.m_shared.m_name))
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