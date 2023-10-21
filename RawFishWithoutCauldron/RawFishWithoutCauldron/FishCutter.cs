using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RawFishWithoutCauldron
{
    internal class FishCutter
    {
        private static readonly Dictionary<string, Recipe> soloFishRecipes = new Dictionary<string, Recipe>();

        internal static bool TryFilletFish(Humanoid user, ItemDrop.ItemData item, ref bool __result)
        {
            if (!user.IsPlayer() || user != Player.m_localPlayer || !user.TakeInput())
            {
                return false;
            }

            Recipe fishRecipe = ObjectDB.instance.m_recipes.Find((Recipe x) => x.name.Equals("Recipe_Fish1"));

            if (ItemChecker.IsKnife(item.m_shared))
            {
                InventoryGui.instance.m_craftUpgradeItem = null;
                InventoryGui.instance.m_craftRecipe = fishRecipe;
            }
            else if (ItemChecker.IsRecipeFish(item.m_shared, fishRecipe, out Piece.Requirement foundRecipePart))
            {
                if (!ItemChecker.HasKnife(Player.m_localPlayer))
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, "$msg_missingrequirement: $skill_knives");

                    // if it's a knife or fish, intentionally always cancel orig call regardless of crafting success, so you don't get the annoying fire pit message
                    __result = true;
                    return true;
                }

                InventoryGui.instance.m_craftUpgradeItem = null;
                InventoryGui.instance.m_craftRecipe = GetSoloFishRecipe(item, fishRecipe, foundRecipePart);
            }
            else
            {
                return false;
            }

            if (TryDoCrafting(InventoryGui.instance))
            {
                Player.m_localPlayer.StartCoroutine(SpawnFishBlood());
            }

            InventoryGui.instance.m_craftRecipe = null;

            // if it's a knife or fish, intentionally always cancel orig call regardless of crafting success, so you don't get the annoying fire pit message
            __result = true;
            return true;
        }

        private static Recipe GetSoloFishRecipe(ItemDrop.ItemData item, Recipe baseFishRecipe, Piece.Requirement foundRecipePart)
        {
            if (soloFishRecipes.TryGetValue(item.m_shared.m_name, out Recipe recipe)
                && recipe != null && recipe.m_resources != null && recipe.m_resources.Length == 1 && recipe.m_resources[0] != null)
            {
                return recipe;
            }

            if (recipe != null)
            {
                RawFishWithoutCauldronPlugin.LogSource.LogInfo($"Found a fish recipe that is not null, but still invalid. If this happens a lot, please report.");
            }

            Recipe customRecipe = Object.Instantiate(baseFishRecipe);
            customRecipe.m_resources = new Piece.Requirement[] { foundRecipePart };

            soloFishRecipes[item.m_shared.m_name] = customRecipe;

            return customRecipe;
        }

        private static bool TryDoCrafting(InventoryGui inventoryGui)
        {
            Recipe recipe = inventoryGui.m_craftRecipe;
            string recipeName = recipe.m_item.m_itemData.m_shared.m_name;
            var craftStats = Game.instance.GetPlayerProfile().m_itemCraftStats;

            CraftingStation oldStation = recipe.m_craftingStation;
            recipe.m_craftingStation = null;

            float preCraftCount = craftStats.GetValueSafe(recipeName);
            inventoryGui.DoCrafting(Player.m_localPlayer);
            float postCraftCount = craftStats.GetValueSafe(recipeName);

            recipe.m_craftingStation = oldStation;

            return postCraftCount > preCraftCount;
        }

        private static IEnumerator SpawnFishBlood()
        {
            yield return new WaitForSeconds(0.22f);

            GameObject prefab = ZNetScene.instance.GetPrefab("fx_TickBloodHit");

            if (prefab)
            {
                Vector3 position = Player.m_localPlayer.transform.position
                    + (Player.m_localPlayer.transform.forward * 0.8f)
                    + (Player.m_localPlayer.transform.up * 1f);

                Object.Instantiate(prefab, position, Quaternion.identity);
            }
        }
    }
}