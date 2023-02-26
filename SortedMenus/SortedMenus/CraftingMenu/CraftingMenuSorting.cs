using System.Collections.Generic;

namespace SortedMenus
{
    internal class CraftingMenuSorting
    {
        internal static void UpdateRecipeList(ref List<Recipe> recipes, string stationName, bool isHandCrafting)
        {
            if (SortConfig.CraftingMenuSorting.Value == SortCraftingMenu.Disabled)
            {
                return;
            }

            // no need to sort the hand crafting menu while not in debug mode
            if (isHandCrafting && !Player.m_localPlayer.NoCostCheat())
            {
                return;
            }

            List<Recipe> relevantCache = null;
            CraftingStationType stationType = CraftingStationType.Other;

            if (isHandCrafting)
            {
                relevantCache = SortedRecipeCaches.cachedSortedNoCostRecipes;
            }
            else if (stationName == "forge")
            {
                // the prefab key is not "piece_forge" for some reason, even if the translation key is
                stationType = CraftingStationType.Forge;

                if (Player.m_localPlayer.NoCostCheat())
                {
                    relevantCache = SortedRecipeCaches.cachedSortedNoCostRecipes;
                }
                else
                {
                    relevantCache = SortedRecipeCaches.cachedSortedForgeRecipes;
                }
            }
            else if (stationName == "piece_workbench")
            {
                if (Player.m_localPlayer.NoCostCheat())
                {
                    relevantCache = SortedRecipeCaches.cachedSortedNoCostRecipes;
                }
                else
                {
                    relevantCache = SortedRecipeCaches.cachedSortedWorkBenchRecipes;
                }
            }

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            if (!SortedRecipeCaches.TryReapplySorting(ref recipes, relevantCache))
            {
                ItemSortOverrider.UpdateSortOverrides(recipes);

                recipes.Sort((recipeA, recipeB) => recipeA.CompareRecipeTo(recipeB, stationType));
                SortedRecipeCaches.UpdateSortCache(recipes, relevantCache);
            }

            sw.Stop();
            Helper.Log($"Time to sort {recipes.Count} recipes: {sw.Elapsed}");
        }
    }
}