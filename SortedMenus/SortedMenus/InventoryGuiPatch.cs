using HarmonyLib;
using System.Collections.Generic;

namespace SortedMenus
{
    [HarmonyPatch]
    internal class InventoryGuiPatch
    {
        internal static Dictionary<string, string> craftingStationSortingOverwrites = new Dictionary<string, string>();

        // higher than normal due to AAA crafting paginator
        [HarmonyPriority(Priority.HigherThanNormal)]
        [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.UpdateRecipeList)), HarmonyPrefix]
        private static void UpdateRecipeList(InventoryGui __instance, ref List<Recipe> recipes)
        {
            if (!Player.m_localPlayer)
            {
                return;
            }

            CraftingStation currentCraftingStation = Player.m_localPlayer.GetCurrentCraftingStation();

            string stationName = currentCraftingStation ? Utils.GetPrefabName(currentCraftingStation.gameObject) : null;

            if (stationName != null && craftingStationSortingOverwrites.TryGetValue(stationName, out string sortingType))
            {
                switch (sortingType.ToLower())
                {
                    case "ignored":
                        return;

                    case "cooking":
                        CookingMenuSorting.UpdateRecipeList(ref recipes, stationName, __instance.InCraftTab());
                        return;

                    default:
                        break;
                }
            }

            if (stationName == "piece_cauldron")
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