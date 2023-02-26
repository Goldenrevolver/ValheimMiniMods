using System.Collections.Generic;

namespace SortedMenus
{
    internal class CookingMenuSorting
    {
        internal static readonly Dictionary<string, ItemDrop.ItemData> cookingStationAndOvenRecipes = new Dictionary<string, ItemDrop.ItemData>();

        internal static void UpdateRecipeList(ref List<Recipe> recipes, string stationName, bool isInCraftTab)
        {
            if (SortConfig.CookingMenuSorting.Value == SortCookingMenu.Disabled)
            {
                return;
            }

            if (!isInCraftTab)
            {
                return;
            }

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            // modded cooking pots don't get caching
            if (stationName != "piece_cauldron")
            {
                recipes.Sort((recipeA, recipeB) => recipeA.CompareRecipeTo(recipeB, CraftingStationType.CookingPot));
            }
            else if (!SortedRecipeCaches.TryReapplySorting(ref recipes, SortedRecipeCaches.cachedSortedCauldronRecipes))
            {
                UpdateCookingStationAndOvenRecipeList();

                recipes.Sort((recipeA, recipeB) => recipeA.CompareRecipeTo(recipeB, CraftingStationType.CookingPot));
                SortedRecipeCaches.UpdateSortCache(recipes, SortedRecipeCaches.cachedSortedCauldronRecipes);
            }

            sw.Stop();
            Helper.Log($"Time to sort {recipes.Count} recipes: {sw.Elapsed}");
        }

        internal static void UpdateCookingStationAndOvenRecipeList()
        {
            cookingStationAndOvenRecipes.Clear();

            if (!SortConfig.CheckIfItemIsInputForOvenOrCookingStation.Value)
            {
                return;
            }

            var oven = ZNetScene.instance.GetPrefab("piece_oven").GetComponent<CookingStation>();
            var woodCookingStation = ZNetScene.instance.GetPrefab("piece_cookingstation").GetComponent<CookingStation>();
            var ironCookingStation = ZNetScene.instance.GetPrefab("piece_cookingstation_iron").GetComponent<CookingStation>();

            IterateOverCookingStation(oven);
            IterateOverCookingStation(woodCookingStation);
            IterateOverCookingStation(ironCookingStation);
        }

        private static void IterateOverCookingStation(CookingStation station)
        {
            foreach (var conversion in station.m_conversion)
            {
                if (conversion == null || !conversion.m_to || !conversion.m_from)
                {
                    continue;
                }

                var key = conversion.m_from.m_itemData.m_shared.m_name;

                if (!cookingStationAndOvenRecipes.TryGetValue(key, out ItemDrop.ItemData currentConversion) || currentConversion.GetSimpleFoodTotal() < conversion.m_to.m_itemData.GetSimpleFoodTotal())
                {
                    cookingStationAndOvenRecipes[key] = conversion.m_to.m_itemData;
                }
            }
        }
    }
}