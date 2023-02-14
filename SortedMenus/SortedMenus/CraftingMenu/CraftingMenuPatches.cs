using HarmonyLib;
using System.Collections.Generic;

namespace SortedMenus
{
    [HarmonyPatch]
    internal class CraftingMenuPatches
    {
        [HarmonyPriority(Priority.VeryLow)]
        [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.UpdateRecipeList)), HarmonyPrefix]
        private static void UpdateRecipeList(List<Recipe> recipes)
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

            string stationName = !currentCraftingStation ? "hand crafting" : currentCraftingStation.m_name;

            if (stationName == "$piece_cauldron")
            {
                return;
            }

            var stationType = stationName == "$piece_forge" ? CraftingStationType.Forge : CraftingStationType.Other;

            ItemSortOverrider.UpdateSortOverrides(recipes);

            recipes.Sort((recipeA, recipeB) => recipeA.CompareRecipeTo(recipeB, stationType));
        }
    }
}