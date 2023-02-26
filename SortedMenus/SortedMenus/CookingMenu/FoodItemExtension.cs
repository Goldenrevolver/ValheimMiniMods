namespace SortedMenus
{
    internal static class FoodItemExtension
    {
        internal readonly struct StatsWhenEaten
        {
            internal readonly float health;
            internal readonly float stamina;
            internal readonly float eitr;
            internal readonly float total;

            public StatsWhenEaten(float health, float stamina, float eitr)
            {
                this.health = health;
                this.stamina = stamina;
                this.eitr = eitr;
                this.total = health + stamina + eitr;
            }
        }

        private static bool IsOrCanBeEdible(ref ItemDrop.ItemData item)
        {
            if (SortConfig.CheckIfItemIsInputForOvenOrCookingStation.Value && CookingMenuSorting.cookingStationAndOvenRecipes.TryGetValue(item.m_shared.m_name, out ItemDrop.ItemData cookingStationOutput))
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

            return item.m_shared.m_food + item.m_shared.m_foodStamina + item.m_shared.m_foodEitr;
        }

        internal static StatsWhenEaten GetStatsWhenEaten(this ItemDrop.ItemData item)
        {
            if (!IsOrCanBeEdible(ref item))
            {
                return new StatsWhenEaten(0, 0, 0);
            }

            return new StatsWhenEaten(item.m_shared.m_food, item.m_shared.m_foodStamina, item.m_shared.m_foodEitr);
        }

        internal static int CompareFoodByConfig(this ItemDrop.ItemData thisItem, ItemDrop.ItemData otherItem)
        {
            var thisStats = thisItem.GetStatsWhenEaten();
            var otherStats = otherItem.GetStatsWhenEaten();

            int totalComp = thisStats.total.CompareTo(otherStats.total);

            if (SortConfig.SortInediblesToBottom.Value && (thisStats.total == 0 || otherStats.total == 0))
            {
                if (totalComp != 0)
                {
                    return -totalComp;
                }
            }

            if (thisStats.total == 0 && otherStats.total == 0)
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

            int healthComp = thisStats.health.CompareTo(otherStats.health);
            int staminaComp = thisStats.stamina.CompareTo(otherStats.stamina);
            int eitrComp = thisStats.eitr.CompareTo(otherStats.eitr);

            if (!SortConfig.SortFoodInAscendingOrder.Value)
            {
                totalComp *= -1;
                healthComp *= -1;
                staminaComp *= -1;
                eitrComp *= -1;
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

                case SortCookingMenu.ByTotalThenEitr:
                    if (totalComp != 0)
                    {
                        return totalComp;
                    }
                    else if (eitrComp != 0)
                    {
                        return eitrComp;
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

                case SortCookingMenu.ByEitrThenTotal:
                    if (eitrComp != 0)
                    {
                        return eitrComp;
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