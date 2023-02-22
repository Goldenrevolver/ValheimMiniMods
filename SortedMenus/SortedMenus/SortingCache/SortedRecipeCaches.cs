using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace SortedMenus
{
    [HarmonyPatch]
    internal class SortedRecipeCaches
    {
        internal static readonly List<Recipe> cachedSortedCauldronRecipes = new List<Recipe>();
        internal static readonly List<Recipe> cachedSortedWorkBenchRecipes = new List<Recipe>();
        internal static readonly List<Recipe> cachedSortedForgeRecipes = new List<Recipe>();

        internal static readonly List<Recipe> cachedSortedNoCostRecipes = new List<Recipe>();

        [HarmonyPatch(typeof(Game), nameof(Game.Logout)), HarmonyPrefix]
        internal static void ResetOnLogout()
        {
            InvalidateCaches();
        }

        // it's important that this is a prefix patch, because the recipe is already learned in postfix
        [HarmonyPatch(typeof(Player), nameof(Player.AddKnownRecipe)), HarmonyPrefix]
        internal static void ResetOnAddNewRecipe(Player __instance, Recipe recipe)
        {
            if (!__instance || __instance != Player.m_localPlayer)
            {
                return;
            }

            if (!__instance.m_knownRecipes.Contains(recipe.m_item.m_itemData.m_shared.m_name))
            {
                InvalidateCaches();
            }
        }

        // it's important that this is a prefix patch, because the recipe is already learned in postfix
        [HarmonyPatch(typeof(Player), nameof(Player.AddKnownPiece)), HarmonyPrefix]
        internal static void ResetOnAddNewPiece(Player __instance, Piece piece)
        {
            if (!__instance || __instance != Player.m_localPlayer)
            {
                return;
            }

            if (!__instance.m_knownRecipes.Contains(piece.m_name))
            {
                InvalidateCaches();
            }
        }

        internal static void InvalidateCaches()
        {
            cachedSortedCauldronRecipes.Clear();
            cachedSortedWorkBenchRecipes.Clear();
            cachedSortedForgeRecipes.Clear();

            cachedSortedNoCostRecipes.Clear();
        }

        internal static void UpdateSortCache(List<Recipe> newSortedList, List<Recipe> oldSortedList)
        {
            if (oldSortedList == null)
            {
                return;
            }

            oldSortedList.Clear();
            oldSortedList.AddRange(newSortedList);
        }

        internal static bool TryReapplySorting(ref List<Recipe> newSortedList, List<Recipe> oldSortedList)
        {
            if (oldSortedList == null || !SortConfig.EnablePerformanceMode.Value)
            {
                return false;
            }

            if (newSortedList.Count != oldSortedList.Count)
            {
                return false;
            }

            // this is (in this use case) equivalent to:
            // newSortedList = newSortedList.OrderBy((a) => oldSortedList.IndexOf(a)).ToList();
            // (but 5 times faster because IndexOf is expensive)
            newSortedList = oldSortedList.Join(newSortedList, i => i, j => j, (i, j) => j).ToList();

            return true;
        }
    }
}