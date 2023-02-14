namespace SortedMenus
{
    internal static class FoodItemExtension
    {
        internal struct HealthAndStamina
        {
            internal float health;
            internal float stamina;

            public HealthAndStamina(float health, float stamina)
            {
                this.health = health;
                this.stamina = stamina;
            }
        }

        private static bool IsOrCanBeEdible(ref ItemDrop.ItemData item)
        {
            if (SortConfig.CheckIfItemIsInputForOvenOrCookingStation.Value && CookingMenuPatches.cookingStationAndOvenRecipes.TryGetValue(item.m_shared.m_name, out ItemDrop.ItemData cookingStationOutput))
            {
                item = cookingStationOutput;
            }

            return item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Consumable;
        }

        internal static float GetSimpleFoodTotal(this ItemDrop.ItemData item)
        {
            if (item.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Consumable)
            {
                return 0;
            }

            return item.m_shared.m_food + item.m_shared.m_foodStamina;
        }

        internal static HealthAndStamina GetHealthAndStamina(this ItemDrop.ItemData item)
        {
            if (!IsOrCanBeEdible(ref item))
            {
                return new HealthAndStamina(0, 0);
            }

            return new HealthAndStamina(item.m_shared.m_food, item.m_shared.m_foodStamina);
        }

        internal static int CompareFoodByConfig(this ItemDrop.ItemData thisItem, ItemDrop.ItemData otherItem)
        {
            if (thisItem == null || otherItem == null)
            {
                return (thisItem != null).CompareTo(otherItem != null);
            }

            var thisHS = thisItem.GetHealthAndStamina();
            var otherHS = otherItem.GetHealthAndStamina();

            float thisTotal = thisHS.health + thisHS.stamina;
            float otherTotal = otherHS.health + otherHS.stamina;

            if (SortConfig.SortInediblesToBottom.Value && (thisTotal == 0 || otherTotal == 0))
            {
                if (otherTotal != 0)
                {
                    return 1;
                }
                else if (thisTotal != 0)
                {
                    return -1;
                }
            }

            if (thisTotal == 0 && otherTotal == 0)
            {
                // sort fishing bait to the bottom bottom
                int ammoComp = thisItem.IsAmmo().CompareTo(otherItem.IsAmmo());

                if (ammoComp != 0)
                {
                    return ammoComp;
                }
                else
                {
                    return thisItem.CompareItemNameTo(otherItem);
                }
            }

            int totalComp = thisTotal.CompareTo(otherTotal);
            int healthComp = thisHS.health.CompareTo(otherHS.health);
            int staminaComp = thisHS.stamina.CompareTo(otherHS.stamina);

            if (!SortConfig.SortFoodInAscendingOrder.Value)
            {
                totalComp *= -1;
                healthComp *= -1;
                staminaComp *= -1;
            }

            switch (SortConfig.CookingMenuSorting.Value)
            {
                case SortCookingMenu.ByTotalThenHealth:
                    if (totalComp != 0)
                    {
                        return totalComp;
                    }
                    else if (healthComp != 0)
                    {
                        return healthComp;
                    }
                    break;

                case SortCookingMenu.ByTotalThenStamina:
                    if (totalComp != 0)
                    {
                        return totalComp;
                    }
                    else if (staminaComp != 0)
                    {
                        return staminaComp;
                    }
                    break;

                case SortCookingMenu.ByHealthThenTotal:
                    if (healthComp != 0)
                    {
                        return healthComp;
                    }
                    else if (totalComp != 0)
                    {
                        return totalComp;
                    }
                    break;

                case SortCookingMenu.ByStaminaThenTotal:
                    if (staminaComp != 0)
                    {
                        return staminaComp;
                    }
                    else if (totalComp != 0)
                    {
                        return totalComp;
                    }
                    break;
            }

            return thisItem.CompareItemNameTo(otherItem);
        }
    }
}