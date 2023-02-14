using HarmonyLib;
using System.Collections.Generic;

namespace SortedMenus
{
    [HarmonyPatch]
    internal class CookingMenuPatches
    {
        internal static readonly Dictionary<string, ItemDrop.ItemData> cookingStationAndOvenRecipes = new Dictionary<string, ItemDrop.ItemData>();

        [HarmonyPriority(Priority.VeryLow)]
        [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.UpdateRecipeList)), HarmonyPrefix]
        private static void UpdateRecipeList(InventoryGui __instance, List<Recipe> recipes)
        {
            if (!Player.m_localPlayer)
            {
                return;
            }

            if (SortConfig.CookingMenuSorting.Value == SortCookingMenu.Disabled)
            {
                return;
            }

            if (!__instance.InCraftTab())
            {
                return;
            }

            CraftingStation currentCraftingStation = Player.m_localPlayer.GetCurrentCraftingStation();

            if (!currentCraftingStation)
            {
                return;
            }

            if (currentCraftingStation.m_name != "$piece_cauldron")
            {
                return;
            }

            if (SortConfig.CheckIfItemIsInputForOvenOrCookingStation.Value)
            {
                UpdateCookingStationAndOvenRecipeList();
            }

            recipes.Sort((recipeA, recipeB) => recipeA.CompareRecipeTo(recipeB, CraftingStationType.CookingPot));
        }

        private static void UpdateCookingStationAndOvenRecipeList()
        {
            cookingStationAndOvenRecipes.Clear();

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

                if (!cookingStationAndOvenRecipes.ContainsKey(key) || cookingStationAndOvenRecipes[key].GetSimpleFoodTotal() < conversion.m_to.m_itemData.GetSimpleFoodTotal())
                {
                    cookingStationAndOvenRecipes[key] = conversion.m_to.m_itemData;
                }
            }
        }
    }
}