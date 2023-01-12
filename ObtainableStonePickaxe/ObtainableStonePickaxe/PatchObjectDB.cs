using HarmonyLib;
using UnityEngine.SceneManagement;
using static ObtainableStonePickaxe.ObtainableStonePickaxePlugin;

namespace ObtainableStonePickaxe
{
    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
    internal static class PatchObjectDB
    {
        private const string bronzePickaxe = "$item_pickaxe_bronze";
        private const string anterPickaxe = "$item_pickaxe_antler";
        private const string stonePickaxe = "$item_pickaxe_stone";
        private const string stoneAxe = "$item_axe_stone";

        public static void Postfix(ObjectDB __instance)
        {
            if (SceneManager.GetActiveScene().name != "main")
            {
                return;
            }

            EffectList hitEffect = null;
            EffectList hitTerrainEffect = null;

            foreach (var gameObject in __instance.m_items)
            {
                if (gameObject == null)
                {
                    continue;
                }

                var item = gameObject.GetComponent<ItemDrop>();

                if (item == null)
                {
                    continue;
                }

                ItemDrop.ItemData.SharedData shared = item.m_itemData.m_shared;

                if (shared.m_name == bronzePickaxe && BronzePickaxeUpgradeFix.Value)
                {
                    shared.m_damagesPerLevel.m_pickaxe = shared.m_damagesPerLevel.m_pierce;
                }

                if (shared.m_name == anterPickaxe)
                {
                    hitEffect = shared.m_hitEffect;
                    hitTerrainEffect = shared.m_hitTerrainEffect;

                    if (UpgradeableAntlerPickaxe.Value)
                    {
                        shared.m_maxQuality = 4;
                        shared.m_damagesPerLevel.m_pickaxe = 5;
                        shared.m_damagesPerLevel.m_pierce = 5;

                        shared.m_blockPowerPerLevel = 0;
                        shared.m_deflectionForcePerLevel = 5;
                        shared.m_durabilityPerLevel = 50;
                    }
                }
            }

            foreach (var gameObject in __instance.m_items)
            {
                if (gameObject == null)
                {
                    continue;
                }

                var item = gameObject.GetComponent<ItemDrop>();

                if (item == null)
                {
                    continue;
                }

                ItemDrop.ItemData.SharedData shared = item.m_itemData.m_shared;

                if (shared.m_name == stonePickaxe)
                {
                    // m_spawnOnHit and m_spawnOnHitTerrain are already the same
                    shared.m_hitEffect = hitEffect;
                    shared.m_hitTerrainEffect = hitTerrainEffect;

                    shared.m_deflectionForce = 20;
                    shared.m_attack.m_attackStamina = 6;

                    shared.m_toolTier = RockPatches.stonePickaxeTier;

                    if (UpgradeableStonePickaxe.Value)
                    {
                        shared.m_maxQuality = 4;
                        shared.m_damagesPerLevel.m_pickaxe = 5;
                        shared.m_damagesPerLevel.m_pierce = 5;

                        shared.m_blockPowerPerLevel = 0;
                        shared.m_deflectionForcePerLevel = 5;
                        shared.m_durabilityPerLevel = 50;
                    }
                }
            }

            CraftingStation repairStation = null;
            //Piece.Requirement[] craftingRecipe = null;

            foreach (var recipe in __instance.m_recipes)
            {
                if (recipe.m_item?.m_itemData.m_shared.m_name == stoneAxe)
                {
                    repairStation = recipe.m_repairStation;
                    //craftingRecipe = recipe.m_resources;
                }
            }

            foreach (var recipe in __instance.m_recipes)
            {
                if (recipe.m_item?.m_itemData.m_shared.m_name == stonePickaxe)
                {
                    recipe.m_enabled = true;
                    recipe.m_craftingStation = null;
                    recipe.m_repairStation = repairStation;

                    // I quite like iron gate's recipe now
                    //if (craftingRecipe != null && UpdateStonePickaxeRecipe.Value)
                    //{
                    //    recipe.m_resources = craftingRecipe;
                    //}
                }
            }
        }
    }
}