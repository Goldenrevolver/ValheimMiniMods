using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static ItemDrop;

namespace SmarterCorpseRun
{
    [BepInPlugin("goldenrevolver.SmarterCorpseRun", NAME, VERSION)]
    public class SmarterCorpseRunPlugin : BaseUnityPlugin
    {
        public const string NAME = "Smarter Corpse Run and Tombstone";
        public const string VERSION = "1.0";

        public const string CustomCorpseRun = "GoldensCorpseRunWithCustomCarryWeight";

        public static ConfigEntry<bool> EnableCorpseRunChange;
        public static ConfigEntry<bool> EnableTombStoneChange;

        public static ConfigEntry<bool> ConsiderEquippingMultipleUniqueUtilityItems;
        public static ConfigEntry<bool> CheckForNonUtilityItems;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            var section = "General";

            EnableCorpseRunChange = Config.Bind(section, nameof(EnableCorpseRunChange), true, "Corpse Run now gives exactly as much bonus carrying capacity as you can get from reequipping your items and updates with every reequip.");
            EnableTombStoneChange = Config.Bind(section, nameof(EnableTombStoneChange), true, "When deciding if you can take all your gear, the tombstone now condiders the bonus carrying capacity by reequipping items from your tombstone that you haven't equipped already.");

            ConsiderEquippingMultipleUniqueUtilityItems = Config.Bind(section, nameof(ConsiderEquippingMultipleUniqueUtilityItems), false, "Whether the bonus carrying capacity should consider equipping multiple utility items instead of only the best one. Only enable if using a mod that allows that.");
            CheckForNonUtilityItems = Config.Bind(section, nameof(CheckForNonUtilityItems), true, "Also check for bonus carrying capacity from non utility items (best one per category, set bonuses requiring more than one item are not considered).");
        }

        internal static void GrantCustomCorpseRun(Player player, SE_Stats oldCorpseRun)
        {
            float potentialExtraCarryingCapacity = CalculatePotentialExtraCarryingCapacity(player.m_inventory.m_inventory, player.m_inventory.GetEquipedtems());

            var customCorpseRun = oldCorpseRun.Clone() as SE_Stats;
            customCorpseRun.name = CustomCorpseRun;
            customCorpseRun.m_addMaxCarryWeight = potentialExtraCarryingCapacity;

            player.m_seman.AddStatusEffect(customCorpseRun, true, 0, 0f);
        }

        internal static float GetCarryWeightBonus(ItemData item)
        {
            float limit = 300;
            item.m_shared.m_equipStatusEffect?.ModifyMaxCarryWeight(300, ref limit);

            // used in mods like Jude's equipment for some reason
            if (item.m_shared.m_setSize == 1)
            {
                item.m_shared.m_setStatusEffect?.ModifyMaxCarryWeight(300, ref limit);
            }

            return limit - 300;
        }

        internal static float CalculatePotentialExtraCarryingCapacity(List<ItemData> inventory, List<ItemData> alreadyEquippedItems)
        {
            float currentHighestUtilBuff = 0f;
            var currentOtherWeightBuffItems = new Dictionary<ItemData.ItemType, float>();
            // no need to check for util buff items, because if the player has a mod to allow equipping multiple, then we assume they can equip every unique one

            foreach (var invItem in alreadyEquippedItems)
            {
                float bonus = GetCarryWeightBonus(invItem);
                if (bonus <= 0)
                {
                    continue;
                }

                if (invItem.m_shared.m_itemType == ItemData.ItemType.Utility)
                {
                    currentHighestUtilBuff = Math.Max(currentHighestUtilBuff, bonus);
                }
                else if (CheckForNonUtilityItems.Value)
                {
                    currentOtherWeightBuffItems.TryGetValue(invItem.m_shared.m_itemType, out var bestBuff);

                    currentOtherWeightBuffItems[invItem.m_shared.m_itemType] = Math.Max(bestBuff, bonus);
                }
            }

            float highestUtilBuff = 0f;
            var tombStoneUtilityWeightBuffItems = new Dictionary<string, float>();
            var tombStoneOtherWeightBuffItems = new Dictionary<ItemData.ItemType, float>();

            foreach (var invItem in inventory)
            {
                float bonus = GetCarryWeightBonus(invItem);
                if (bonus <= 0)
                {
                    continue;
                }

                // already equipped, so can't provide further bonus, skip
                if (alreadyEquippedItems != null && alreadyEquippedItems.Any((eItem) => eItem.m_shared.m_name == invItem.m_shared.m_name))
                {
                    continue;
                }

                if (invItem.m_shared.m_itemType == ItemData.ItemType.Utility)
                {
                    highestUtilBuff = Math.Max(highestUtilBuff, bonus);

                    tombStoneUtilityWeightBuffItems[invItem.m_shared.m_name] = bonus;
                }
                else if (CheckForNonUtilityItems.Value)
                {
                    tombStoneOtherWeightBuffItems.TryGetValue(invItem.m_shared.m_itemType, out var bestBuff);

                    tombStoneOtherWeightBuffItems[invItem.m_shared.m_itemType] = Math.Max(bestBuff, bonus);
                }
            }

            float potentialExtraCarryingCapacity = 0f;

            if (!ConsiderEquippingMultipleUniqueUtilityItems.Value)
            {
                potentialExtraCarryingCapacity += Math.Max(highestUtilBuff - currentHighestUtilBuff, 0);
            }
            else
            {
                foreach (var item in tombStoneUtilityWeightBuffItems)
                {
                    potentialExtraCarryingCapacity += item.Value;
                }
            }

            if (CheckForNonUtilityItems.Value)
            {
                foreach (var item in tombStoneOtherWeightBuffItems)
                {
                    currentOtherWeightBuffItems.TryGetValue(item.Key, out float currentValue);

                    potentialExtraCarryingCapacity += Math.Max(item.Value - currentValue, 0);
                }
            }

            return potentialExtraCarryingCapacity;
        }
    }
}