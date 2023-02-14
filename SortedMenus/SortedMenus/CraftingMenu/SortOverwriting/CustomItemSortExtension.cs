namespace SortedMenus
{
    internal static class CustomItemSortExtension
    {
        internal static string GetCompareName(this ItemDrop.ItemData item)
        {
            string itemName = item.m_shared.m_name;

            if (ItemSortOverrider.sortOverrides.TryGetValue(itemName, out string overrideName))
            {
                return overrideName;
            }

            return itemName;
        }

        internal static int CompareItemWithArmorOverridesTo(this ItemDrop.ItemData thisItem, ItemDrop.ItemData otherItem)
        {
            string thisFirstCompare = thisItem.GetCompareName();
            string otherFirstCompare = otherItem.GetCompareName();

            int comp = thisFirstCompare.CompareItemNameTo(otherFirstCompare);

            if (comp != 0)
            {
                return comp;
            }

            return thisItem.GetArmorCompareType().CompareTo(otherItem.GetArmorCompareType());
        }

        internal static int GetArmorCompareType(this ItemDrop.ItemData item)
        {
            switch (item.m_shared.m_itemType)
            {
                case ItemDrop.ItemData.ItemType.Helmet:
                    return 0;

                case ItemDrop.ItemData.ItemType.Chest:
                    return 1;

                case ItemDrop.ItemData.ItemType.Legs:
                    return 2;

                case ItemDrop.ItemData.ItemType.Shoulder:
                    return 3;

                default:
                    return 4;
            }
        }

        internal static int CompareAmmoTo(this ItemDrop.ItemData thisAmmo, ItemDrop.ItemData otherAmmo, SortAmmo ammoPriority)
        {
            if (ammoPriority == 0)
            {
                return 0;
            }

            if (SortConfig.UseSmartAmmoSorting.Value)
            {
                return thisAmmo.GetAmmoCompareValue(ammoPriority).CompareTo(otherAmmo.GetAmmoCompareValue(ammoPriority));
            }
            else
            {
                int comp = thisAmmo.IsAmmo().CompareTo(otherAmmo.IsAmmo());

                return ammoPriority == SortAmmo.ToTheTop ? -comp : comp;
            }
        }

        internal static int GetAmmoCompareValue(this ItemDrop.ItemData item, SortAmmo ammoPriority)
        {
            switch (item.m_shared.m_itemType)
            {
                case ItemDrop.ItemData.ItemType.Ammo:
                    return item.m_shared.m_name.ToLower().Contains("fishingbait") ? 2 : 0;

                case ItemDrop.ItemData.ItemType.AmmoNonEquipable:
                    return 1;

                default:
                    return ammoPriority == SortAmmo.ToTheTop ? 5 : -5;
            }
        }

        internal static bool IsAmmo(this ItemDrop.ItemData item)
        {
            switch (item.m_shared.m_itemType)
            {
                case ItemDrop.ItemData.ItemType.Ammo:
                case ItemDrop.ItemData.ItemType.AmmoNonEquipable:
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsOverridableArmor(this ItemDrop.ItemData item)
        {
            switch (item.m_shared.m_itemType)
            {
                case ItemDrop.ItemData.ItemType.Helmet:
                case ItemDrop.ItemData.ItemType.Legs:
                case ItemDrop.ItemData.ItemType.Shoulder:
                    return true;

                default:
                    return false;
            }
        }
    }
}