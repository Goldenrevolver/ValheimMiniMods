using HarmonyLib;
using System.Collections.Generic;

namespace SortedMenus
{
    [HarmonyPatch]
    internal class CraftingMenuPatches
    {
        internal const string handCrafting = "hand crafting";

        [HarmonyPriority(Priority.VeryLow)]
        [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.UpdateRecipeList)), HarmonyPrefix]
        private static void UpdateRecipeList(ref List<Recipe> recipes)
        {
            if (!Player.m_localPlayer)
            {
                return;
            }

            if (SortConfig.CraftingMenuSorting.Value == SortCraftingMenu.Disabled)
            {
                return;
            }

            CraftingStation currentCraftingStation = Player.m_localPlayer.GetCurrentCraftingStation();

            // no need to sort the hand crafting menu while not in debug mode
            if (!currentCraftingStation && !Player.m_localPlayer.NoCostCheat())
            {
                return;
            }

            string stationName = !currentCraftingStation ? handCrafting : currentCraftingStation.m_name;

            if (stationName == "$piece_cauldron")
            {
                return;
            }

            List<Recipe> relevantCache = null;
            CraftingStationType stationType = CraftingStationType.Other;

            if (stationName == "$piece_forge")
            {
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
            else if (stationName == "$piece_workbench")
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
            else if (stationName == handCrafting && Player.m_localPlayer.NoCostCheat())
            {
                relevantCache = SortedRecipeCaches.cachedSortedNoCostRecipes;
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