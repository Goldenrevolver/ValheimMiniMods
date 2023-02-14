namespace SortedMenus
{
    internal static class GenericItemExtension
    {
        internal static string GetItemTranslation(this string itemName)
        {
            return Localization.instance.Localize(itemName);
        }

        internal static int CompareItemNameTo(this string thisItemName, string otherItemName)
        {
            return thisItemName.GetItemTranslation().CompareTo(otherItemName.GetItemTranslation());
        }

        internal static int CompareItemNameTo(this ItemDrop.ItemData thisItem, ItemDrop.ItemData otherItem)
        {
            return thisItem.m_shared.m_name.CompareItemNameTo(otherItem.m_shared.m_name);
        }

        internal static int CompareRecipeTo(this Recipe thisRecipe, Recipe otherRecipe, CraftingStationType stationType)
        {
            bool thisNull = thisRecipe == null || !thisRecipe.m_item || thisRecipe.m_item.m_itemData == null;
            bool otherNull = otherRecipe == null || !otherRecipe.m_item || otherRecipe.m_item.m_itemData == null;

            if (thisNull || otherNull)
            {
                return (!thisNull).CompareTo(!otherNull);
            }

            var thisItem = thisRecipe.m_item.m_itemData;
            var otherItem = otherRecipe.m_item.m_itemData;

            if (stationType == CraftingStationType.CookingPot)
            {
                return thisItem.CompareFoodByConfig(otherItem);
            }
            else
            {
                SortAmmo sortOrder = stationType == CraftingStationType.Forge ? SortConfig.SortAmmoAtForge.Value : SortConfig.SortAmmo.Value;

                int comp = thisItem.CompareAmmoTo(otherItem, sortOrder);

                if (comp != 0)
                {
                    return comp;
                }

                if (SortConfig.KeepArmorSetsTogether.Value != ArmorSetSorting.Disabled)
                {
                    return thisItem.CompareItemWithArmorOverridesTo(otherItem);
                }
                else
                {
                    return thisItem.CompareItemNameTo(otherItem);
                }
            }
        }
    }
}