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

            List<Recipe> relevantCache = null;
            CraftingStationType stationType = CraftingStationType.Other;

            if (isHandCrafting)
            {
                if (SortConfig.HandCraftingMenuSorting.Value == SortHandCraftingMenu.Disabled)
                {
                    return;
                }
                else if (SortConfig.HandCraftingMenuSorting.Value == SortHandCraftingMenu.OnlyWhileUsingNoCostCheat
                    && !Player.m_localPlayer.NoCostCheat())
                {
                    return;
                }

                if (Player.m_localPlayer.NoCostCheat())
                {
                    relevantCache = SortedRecipeCaches.cachedSortedNoCostRecipes;
                }
                else
                {
                    relevantCache = SortedRecipeCaches.cachedSortedHandCraftingRecipes;
                }
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