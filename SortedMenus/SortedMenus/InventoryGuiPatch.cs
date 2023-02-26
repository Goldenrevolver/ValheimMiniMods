using HarmonyLib;
using System.Collections.Generic;

namespace SortedMenus
{
    [HarmonyPatch]
    internal class InventoryGuiPatch
    {
        [HarmonyPriority(Priority.VeryLow)]
        [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.UpdateRecipeList)), HarmonyPrefix]
        private static void UpdateRecipeList(InventoryGui __instance, ref List<Recipe> recipes)
        {
            if (!Player.m_localPlayer)
            {
                return;
            }

            CraftingStation currentCraftingStation = Player.m_localPlayer.GetCurrentCraftingStation();

            string stationName = currentCraftingStation ? Utils.GetPrefabName(currentCraftingStation.gameObject) : null;

            if (SortedMenusPlugin.IsCauldronLike(stationName))
            {
                CookingMenuSorting.UpdateRecipeList(ref recipes, stationName, __instance.InCraftTab());
            }
            else
            {
                CraftingMenuSorting.UpdateRecipeList(ref recipes, stationName, !currentCraftingStation);
            }
        }
    }
}