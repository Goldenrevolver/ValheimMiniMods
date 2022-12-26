using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SimpleSetAndCapeBonuses.SimpleSetAndCapeBonusesPlugin;

namespace SimpleSetAndCapeBonuses
{
    [HarmonyPatch(typeof(Localization))]
    internal static class PatchLocalization
    {
        [HarmonyPatch(nameof(Localization.Translate))]
        [HarmonyPostfix]
        public static void Show_Postfix(string word, ref string __result)
        {
            if (word == PatchObjectDB.leatherSetBonus.ToLower())
            {
                __result = LeatherArmorSetBonusLabel.Value;
            }
            else if (word == PatchObjectDB.ragsSetBonus.ToLower())
            {
                __result = ForagerSetBonusLabel.Value;
            }
        }
    }

    [HarmonyPatch(typeof(ObjectDB))]
    internal static class PatchObjectDB
    {
        internal const string leatherSetBonus = "GoldensLeatherSetBonus";
        internal const string ragsSetBonus = "GoldensRagsSetBonus";

        private static readonly string[] ragsSet = new string[] { "HelmetMidsummerCrown", "ArmorRagsChest", "ArmorRagsLegs", };
        private static readonly string[] leatherSet = new string[] { "HelmetLeather", "ArmorLeatherChest", "ArmorLeatherLegs" };
        private static readonly string[] trollSet = new string[] { "HelmetTrollLeather", "ArmorTrollLeatherChest", "ArmorTrollLeatherLegs" };

        [HarmonyPatch(nameof(ObjectDB.Awake))]
        [HarmonyPrefix]
        public static void Show_Postfix(ObjectDB __instance)
        {
            if (SceneManager.GetActiveScene().name != "main")
            {
                return;
            }

            if (AlwaysEnableMidsummerCrownRecipe.Value)
            {
                foreach (var recipe in __instance.m_recipes)
                {
                    if (recipe.m_item?.m_itemData.m_shared.m_name == "$item_helmet_midsummercrown")
                    {
                        if (!recipe.m_enabled)
                        {
                            recipe.m_enabled = true;
                            Debug.Log("Reenabled midsummer crown recipe");
                        }
                    }
                }
            }

            var leatherSetBonusBuff = ScriptableObject.CreateInstance<SE_Stats>();
            leatherSetBonusBuff.m_skillLevel = Skills.SkillType.Run;
            leatherSetBonusBuff.m_skillLevelModifier = 10;
            leatherSetBonusBuff.m_runStaminaDrainModifier = -0.15f;
            leatherSetBonusBuff.name = $"{leatherSetBonus}";
            leatherSetBonusBuff.m_name = $"${leatherSetBonus.ToLower()}";
            leatherSetBonusBuff.m_tooltip = LeatherArmorSetBonusTooltip.Value;

            __instance.m_StatusEffects.Add(leatherSetBonusBuff);

            var ragsSetBonusBuff = ScriptableObject.CreateInstance<StatusEffect>();
            ragsSetBonusBuff.name = $"{ragsSetBonus}";
            ragsSetBonusBuff.m_name = $"${ragsSetBonus.ToLower()}";
            ragsSetBonusBuff.m_tooltip = ForagerSetBonusTooltip.Value;

            __instance.m_StatusEffects.Add(ragsSetBonusBuff);

            foreach (GameObject gameObject in __instance.m_items)
            {
                if (EnableCapeBuffs.Value)
                {
                    if (gameObject.name == "CapeDeerHide")
                    {
                        gameObject.GetComponent<ItemDrop>().m_itemData.m_shared.m_movementModifier = 0.03f;
                    }

                    if (gameObject.name == "CapeTrollHide")
                    {
                        gameObject.GetComponent<ItemDrop>().m_itemData.m_shared.m_movementModifier = 0.05f;
                    }

                    if (gameObject.name == "CapeLinen")
                    {
                        gameObject.GetComponent<ItemDrop>().m_itemData.m_shared.m_movementModifier = 0.1f;
                    }
                }

                if (EnableTrollArmorSetBonusChange.Value)
                {
                    if (gameObject.name == "CapeTrollHide")
                    {
                        var shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;

                        shared.m_setName = string.Empty;
                        shared.m_setSize = 0;
                        shared.m_setStatusEffect = null;
                    }

                    if (trollSet.Contains(gameObject.name))
                    {
                        gameObject.GetComponent<ItemDrop>().m_itemData.m_shared.m_setSize = 3;
                    }
                }

                if (EnableLeatherArmorSetBonus.Value)
                {
                    if (leatherSet.Contains(gameObject.name))
                    {
                        var shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;

                        shared.m_setSize = 3;
                        shared.m_setName = leatherSetBonus;
                        shared.m_setStatusEffect = leatherSetBonusBuff;

                        if (EnableSetBonusIcons.Value && shared.m_itemType == ItemDrop.ItemData.ItemType.Helmet && shared.m_icons?.Length > 0)
                        {
                            leatherSetBonusBuff.m_icon = shared.m_icons[0];
                        }
                    }
                }

                if (EnableForagerArmorSetBonus.Value)
                {
                    if (ragsSet.Contains(gameObject.name))
                    {
                        var shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;

                        shared.m_setSize = 3;
                        shared.m_setName = ragsSetBonus;
                        shared.m_setStatusEffect = ragsSetBonusBuff;

                        if (EnableSetBonusIcons.Value && shared.m_itemType == ItemDrop.ItemData.ItemType.Helmet && shared.m_icons?.Length > 0)
                        {
                            ragsSetBonusBuff.m_icon = shared.m_icons[0];
                        }
                    }
                }
            }
        }
    }
}