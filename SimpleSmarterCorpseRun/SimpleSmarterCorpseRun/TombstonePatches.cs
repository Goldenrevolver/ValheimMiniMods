﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using static ItemDrop;

namespace SimpleSmarterCorpseRun
{
    internal class TombStonePatches
    {
        [HarmonyPatch(typeof(TombStone), nameof(TombStone.EasyFitInInventory))]
        public static class EasyFitInInventory_Patch
        {
            private static void Postfix(TombStone __instance, Player player, ref bool __result)
            {
                if (!SimpleSmarterCorpseRunPlugin.EnableSmartTombStoneWeightCheck.Value)
                {
                    return;
                }

                // if its not the local player, or the base game check succeeded
                if (player != Player.m_localPlayer || __result)
                {
                    return;
                }

                // check failed due to space, nothing we can do
                if (!EasyFitInInventoryWithoutWeightCheck(__instance, player))
                {
                    return;
                }

                var tombStoneInventory = __instance.m_container.m_inventory;
                var playerInventory = player.m_inventory;

                var combinedInventory = new List<ItemData>();
                combinedInventory.AddRange(tombStoneInventory.m_inventory);
                combinedInventory.AddRange(playerInventory.m_inventory);

                float potentialExtraCarryingCapacity = CorpseRunMath.CalculatePotentialExtraCarryingCapacity(combinedInventory, playerInventory.GetEquippedItems());

                if (playerInventory.GetTotalWeight() + tombStoneInventory.GetTotalWeight() <= player.GetMaxCarryWeight() + potentialExtraCarryingCapacity)
                {
                    __result = true;
                    return;
                }
            }
        }

        // the base game method from TombStone.EasyFitInInventory without the carryweight check at the end
        private static bool EasyFitInInventoryWithoutWeightCheck(TombStone __instance, Player player)
        {
            int num = player.m_inventory.GetEmptySlots() - __instance.m_container.m_inventory.NrOfItems();

            if (num < 0)
            {
                foreach (ItemDrop.ItemData itemData in __instance.m_container.m_inventory.GetAllItems())
                {
                    if (player.m_inventory.FindFreeStackSpace(itemData.m_shared.m_name, (float)itemData.m_worldLevel) >= itemData.m_stack)
                    {
                        num++;
                    }
                }

                if (num < 0)
                {
                    return false;
                }
            }

            return true;
        }

        [HarmonyPatch(typeof(TombStone), nameof(TombStone.GiveBoost))]
        public static class GiveBoost_Patch
        {
            private static void Postfix(TombStone __instance)
            {
                if (!SimpleSmarterCorpseRunPlugin.EnableSmartCorpseRunBuff.Value)
                {
                    return;
                }

                if (__instance.m_lootStatusEffect is SE_Stats corpseRun)
                {
                    Player player = __instance.FindOwner();

                    if (player)
                    {
                        player.m_seman.RemoveStatusEffect(corpseRun, true);

                        CorpseRunMath.AddCustomCorpseEffectRunToPlayer(player, corpseRun);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Inventory), nameof(Inventory.MoveAll))]
    public static class Inventory_MoveAll_Patch
    {
        [HarmonyPriority(Priority.HigherThanNormal)]
        private static void Prefix(ref Inventory __instance, ref Inventory fromInventory)
        {
            if (!SimpleSmarterCorpseRunPlugin.EnableSmartTombStoneSlotCheck.Value)
            {
                return;
            }

            if (Player.m_localPlayer.m_inventory != __instance || !fromInventory.m_name.StartsWith("Grave"))
            {
                return;
            }

            // would work, but I don't actually like that behavior
            //if (InventoryGui.instance.IsContainerOpen())
            //{
            //    return;
            //}

            List<ItemData> playerItems = new List<ItemData>(__instance.GetAllItems());

            foreach (ItemData playerItem in playerItems)
            {
                if (playerItem.m_customData != null && playerItem.m_customData.Count > 0)
                {
                    continue;
                }

                if (playerItem.m_shared.m_maxStackSize <= 1)
                {
                    continue;
                }

                foreach (ItemData tombStoneItem in fromInventory.m_inventory)
                {
                    if (tombStoneItem.m_customData != null && tombStoneItem.m_customData.Count > 0)
                    {
                        continue;
                    }

                    if (tombStoneItem.m_shared.m_name == playerItem.m_shared.m_name && tombStoneItem.m_quality == playerItem.m_quality)
                    {
                        int itemsToMove = Math.Min(tombStoneItem.m_shared.m_maxStackSize - tombStoneItem.m_stack, playerItem.m_stack);
                        tombStoneItem.m_stack += itemsToMove;

                        if (playerItem.m_stack == itemsToMove)
                        {
                            playerItem.m_stack = 0;
                            __instance.RemoveItem(playerItem);
                            break;
                        }
                        else
                        {
                            playerItem.m_stack -= itemsToMove;
                        }
                    }
                }
            }
        }
    }
}