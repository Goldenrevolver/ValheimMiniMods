using BepInEx.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using static ItemDrop;

namespace SimpleSmarterCorpseRun
{
    internal class CorpseRunMath
    {
        private const int baseCarryingCapacity = 300;

        internal static bool HaveCustomCorpseRunEffect(Player player, out SE_Stats corpseRun)
        {
            SE_Stats oldCorpseRun = null;

            // based on the original SEMan.HaveStatusEffect that still iterated over 'm_statusEffects'
            // but it returns the status effect in an out param
            foreach (var statusEffect in player.m_seman.m_statusEffects)
            {
                if (statusEffect.name == SimpleSmarterCorpseRunPlugin.CustomCorpseRun)
                {
                    oldCorpseRun = statusEffect as SE_Stats;
                    break;
                }
            }

            corpseRun = oldCorpseRun;
            return corpseRun != null;
        }

        internal static void UpdateCustomCorpseRunEffect(Player player, SE_Stats oldCorpseRun)
        {
            float potentialExtraCarryingCapacity = CalculatePotentialExtraCarryingCapacity(player.m_inventory.m_inventory, player.m_inventory.GetEquippedItems());

            oldCorpseRun.m_addMaxCarryWeight = potentialExtraCarryingCapacity;
        }

        internal static void AddCustomCorpseEffectRunToPlayer(Player player, SE_Stats oldCorpseRun)
        {
            float potentialExtraCarryingCapacity = CalculatePotentialExtraCarryingCapacity(player.m_inventory.m_inventory, player.m_inventory.GetEquippedItems());

            var customCorpseRun = oldCorpseRun.Clone() as SE_Stats;
            customCorpseRun.name = SimpleSmarterCorpseRunPlugin.CustomCorpseRun;
            customCorpseRun.m_addMaxCarryWeight = potentialExtraCarryingCapacity;

            player.m_seman.AddStatusEffect(customCorpseRun, true);
        }

        private static float GetCarryWeightBonus(ItemData item)
        {
            if (!CanHaveCarryWeightBonus(item))
            {
                return 0;
            }

            // we don't assume that the equip status effect is an SE_Stats
            // this way, it works with ANY status effect that properly adds carry weight through StatusEffect.ModifyMaxCarryWeight

            float limit = baseCarryingCapacity;

            item.m_shared.m_equipStatusEffect?.ModifyMaxCarryWeight(baseCarryingCapacity, ref limit);

            // used in mods like Jude's equipment for some reason
            if (item.m_shared.m_setSize == 1)
            {
                item.m_shared.m_setStatusEffect?.ModifyMaxCarryWeight(baseCarryingCapacity, ref limit);
            }

            return limit - baseCarryingCapacity;
        }

        private static bool CanHaveCarryWeightBonus(ItemData item)
        {
            switch (SimpleSmarterCorpseRunPlugin.WhichItemsToCheck.Value)
            {
                case ItemsToCheck.JustUtilityItems:
                    return item.IsEquipable() && item.m_shared.m_itemType == ItemData.ItemType.Utility;

                case ItemsToCheck.UtilityAndShoulderItems:
                    return item.IsEquipable() && item.m_shared.m_itemType == ItemData.ItemType.Utility || item.m_shared.m_itemType == ItemData.ItemType.Shoulder;

                default:
                    return item.IsEquipable();
            }
        }

        private static bool CanEquipMultipleUtilityItems()
        {
            return Chainloader.PluginInfos.ContainsKey("aedenthorn.EquipMultipleUtilityItems");
        }

        internal static float CalculatePotentialExtraCarryingCapacity(List<ItemData> inventory, List<ItemData> alreadyEquippedItems)
        {
            //var sw = new System.Diagnostics.Stopwatch();
            //sw.Start();

            bool considerMultipleUtilityItems = CanEquipMultipleUtilityItems();

            var currentlyEquippedCarryingCapacityBuffItems = new Dictionary<ItemData.ItemType, float>();
            // no need to remember all currently equipped utility buff items:
            // if the player has a mod to allow equipping multiple, then we assume they can equip every unique one (even if aedens config allows to set a maximum)

            foreach (var invItem in alreadyEquippedItems)
            {
                float bonus = GetCarryWeightBonus(invItem);
                if (bonus <= 0)
                {
                    continue;
                }

                currentlyEquippedCarryingCapacityBuffItems.TryGetValue(invItem.m_shared.m_itemType, out var bestBuff);
                // intentionally no if statement, out is default, aka 0, if not found
                currentlyEquippedCarryingCapacityBuffItems[invItem.m_shared.m_itemType] = Math.Max(bestBuff, bonus);
            }

            var tombStoneUtilityCarryingCapacityItems = new Dictionary<string, float>();
            var tombStoneCarryingCapacityItems = new Dictionary<ItemData.ItemType, float>();

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

                if (considerMultipleUtilityItems && invItem.m_shared.m_itemType == ItemData.ItemType.Utility)
                {
                    tombStoneUtilityCarryingCapacityItems[invItem.m_shared.m_name] = bonus;
                }

                tombStoneCarryingCapacityItems.TryGetValue(invItem.m_shared.m_itemType, out var bestBuff);
                // intentionally no if statement, out is default, aka 0, if not found
                tombStoneCarryingCapacityItems[invItem.m_shared.m_itemType] = Math.Max(bestBuff, bonus);
            }

            float potentialExtraCarryingCapacity = 0f;

            foreach (var item in tombStoneCarryingCapacityItems)
            {
                if (considerMultipleUtilityItems && item.Key == ItemData.ItemType.Utility)
                {
                    foreach (var utilityItem in tombStoneUtilityCarryingCapacityItems)
                    {
                        potentialExtraCarryingCapacity += utilityItem.Value;
                    }
                }
                else
                {
                    currentlyEquippedCarryingCapacityBuffItems.TryGetValue(item.Key, out float currentValue);
                    // intentionally no if statement, out is default, aka 0, if not found
                    potentialExtraCarryingCapacity += Math.Max(item.Value - currentValue, 0);
                }
            }

            //sw.Stop();
            //Helper.Log($"Math time: {sw.Elapsed}");

            return potentialExtraCarryingCapacity;
        }
    }
}