using HarmonyLib;
using System.Collections.Generic;
using static ObtainableBlueMushrooms.MushroomConfig;

namespace ObtainableBlueMushrooms
{
    [HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.Start))]
    public static class Patch_CharacterDrop_Start
    {
        private static void Postfix(CharacterDrop __instance)
        {
            if (BatMushroomDropMethod.Value == LootChange.Disabled)
            {
                return;
            }

            if (__instance.m_character.m_name != "$enemy_bat")
            {
                return;
            }

            if (PatchObjectDB.blueMushroomItemDrop == null)
            {
                UnityEngine.Debug.Log("Failed to add blue mushroom to bat drops, because blue mushroom couldn't be found.");
                return;
            }

            if (__instance.m_drops == null)
            {
                __instance.m_drops = new List<CharacterDrop.Drop>();
            }

            var mushroomDrop = new CharacterDrop.Drop()
            {
                m_prefab = PatchObjectDB.blueMushroomItemDrop.gameObject,
                m_chance = UnityEngine.Mathf.Clamp01(BatMushroomDropChance.Value),
                m_amountMin = 1,
                m_amountMax = 1,
                m_onePerPlayer = false,
                m_levelMultiplier = false,
            };

            if (BatMushroomDropMethod.Value == LootChange.AddToExistingDrops)
            {
                __instance.m_drops.Add(mushroomDrop);
            }
            else if (BatMushroomDropMethod.Value == LootChange.ReplaceLeatherScraps)
            {
                if (__instance.m_drops.Count <= 0)
                {
                    return;
                }

                ReplaceLeatherDrop(__instance, mushroomDrop);
            }
        }

        private static void ReplaceLeatherDrop(CharacterDrop characterDrops, CharacterDrop.Drop newDrop)
        {
            for (int i = 0; i < characterDrops.m_drops.Count; i++)
            {
                CharacterDrop.Drop item = characterDrops.m_drops[i];

                // pulled into individual if statements because ?. is unreliable on monobehaviors, and other mods may have changed this loot

                if (item?.m_prefab == null)
                {
                    continue;
                }

                var drop = item.m_prefab.GetComponent<ItemDrop>();

                if (drop == null)
                {
                    continue;
                }

                if (drop.m_itemData?.m_shared?.m_name == "$item_leatherscraps")
                {
                    characterDrops.m_drops[i] = newDrop;
                }
            }
        }
    }
}