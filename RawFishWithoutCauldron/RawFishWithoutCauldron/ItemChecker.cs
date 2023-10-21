namespace RawFishWithoutCauldron
{
    internal class ItemChecker
    {
        internal static bool IsRecipeFish(ItemDrop.ItemData.SharedData shared, Recipe fishRecipe, out Piece.Requirement foundRecipePart)
        {
            foundRecipePart = null;

            if (shared == null || shared.m_itemType != ItemDrop.ItemData.ItemType.Fish)
            {
                return false;
            }

            foreach (Piece.Requirement fish in fishRecipe.m_resources)
            {
                if (fish.m_resItem && fish.m_resItem.m_itemData?.m_shared?.m_name == shared.m_name)
                {
                    foundRecipePart = fish;
                    return true;
                }
            }

            return false;
        }

        internal static bool IsKnife(ItemDrop.ItemData.SharedData shared)
        {
            if (shared == null)
            {
                return false;
            }

            switch (shared.m_itemType)
            {
                case ItemDrop.ItemData.ItemType.OneHandedWeapon:
                case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
                case ItemDrop.ItemData.ItemType.TwoHandedWeaponLeft:
                    return shared.m_skillType == Skills.SkillType.Knives;

                default:
                    return false;
            }
        }

        internal static bool HasKnife(Player player)
        {
            foreach (ItemDrop.ItemData item in player.m_inventory.m_inventory)
            {
                if (item != null && IsKnife(item.m_shared))
                {
                    return true;
                }
            }

            return false;
        }
    }
}