using BepInEx;
using HarmonyLib;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace RawFishWithoutCauldron
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class RawFishWithoutCauldronPlugin : BaseUnityPlugin
    {
        public const string GUID = "goldenrevolver.RawFishWithoutCauldron";
        public const string NAME = "Quickly Cut Raw Fish Without Cauldron";
        public const string VERSION = "1.0.0";

        protected void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch]
    internal static class Patches
    {
        private static bool IsKnife(ItemDrop.ItemData.SharedData shared)
        {
            switch (shared.m_itemType)
            {
                case ItemDrop.ItemData.ItemType.OneHandedWeapon:
                case ItemDrop.ItemData.ItemType.TwoHandedWeapon:
                case ItemDrop.ItemData.ItemType.TwoHandedWeaponLeft:
                    return shared.m_skillType == Skills.SkillType.Knives;

                default:
                    return false;
            }
        }

        [HarmonyPatch(typeof(CookingStation), nameof(CookingStation.UseItem)), HarmonyPrefix]
        public static bool FilletFishPatch(CookingStation __instance, Humanoid user, ItemDrop.ItemData item, ref bool __result)
        {
            var stationName = Utils.GetPrefabName(__instance.gameObject);

            if (stationName != "piece_cookingstation" && stationName != "piece_cookingstation_iron")
            {
                return true;
            }

            return !TryFilletFish(user, item, ref __result);
        }

        [HarmonyPatch(typeof(CraftingStation), nameof(CraftingStation.UseItem)), HarmonyPrefix]
        public static bool FilletFishPatch(CraftingStation __instance, Humanoid user, ItemDrop.ItemData item, ref bool __result)
        {
            var stationName = Utils.GetPrefabName(__instance.gameObject);

            if (stationName != "piece_workbench")
            {
                return true;
            }

            return !TryFilletFish(user, item, ref __result);
        }

        private static bool TryDoCrafting(InventoryGui inventoryGui)
        {
            var recipe = InventoryGui.instance.m_craftRecipe;
            var recipeName = recipe.m_item.m_itemData.m_shared.m_name;
            var craftStats = Game.instance.GetPlayerProfile().m_itemCraftStats;

            var oldStation = recipe.m_craftingStation;
            recipe.m_craftingStation = null;

            float preCraftCount = craftStats.GetValueSafe(recipeName);
            inventoryGui.DoCrafting(Player.m_localPlayer);
            float postCraftCount = craftStats.GetValueSafe(recipeName);

            recipe.m_craftingStation = oldStation;

            return postCraftCount > preCraftCount;
        }

        public static bool TryFilletFish(Humanoid user, ItemDrop.ItemData item, ref bool __result)
        {
            if (!user.IsPlayer() || user != Player.m_localPlayer || !user.TakeInput())
            {
                return false;
            }

            if (!IsKnife(item.m_shared))
            {
                return false;
            }

            InventoryGui.instance.m_craftUpgradeItem = null;
            InventoryGui.instance.m_craftRecipe = ObjectDB.instance.m_recipes.Find((Recipe x) => x.name.Equals("Recipe_Fish1"));

            if (TryDoCrafting(InventoryGui.instance))
            {
                Player.m_localPlayer.StartCoroutine(SpawnFishBlood());
            }

            InventoryGui.instance.m_craftRecipe = null;

            // if its a knife, intentionally always cancel orig call regardless of crafting success, so you don't get the annoying fire pit message
            __result = true;
            return true;
        }

        public static IEnumerator SpawnFishBlood()
        {
            yield return new WaitForSeconds(0.22f);

            var prefab = ZNetScene.instance.GetPrefab("fx_TickBloodHit");

            if (prefab)
            {
                var position = Player.m_localPlayer.transform.position
                    + (Player.m_localPlayer.transform.forward * 0.8f)
                    + (Player.m_localPlayer.transform.up * 1f);

                Object.Instantiate(prefab, position, Quaternion.identity);
            }
        }
    }
}