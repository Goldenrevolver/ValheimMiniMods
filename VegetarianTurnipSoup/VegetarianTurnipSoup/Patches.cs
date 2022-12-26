using HarmonyLib;
using UnityEngine.SceneManagement;

namespace VegetarianTurnipSoup
{
    [HarmonyPatch(typeof(Localization))]
    internal static class PatchLocalization
    {
        [HarmonyPatch(nameof(Localization.Translate))]
        [HarmonyPostfix]
        public static void Translate_Postfix(string word, ref string __result)
        {
            if (word == "item_turnipstew")
            {
                __result = VegetarianTurnipSoupPlugin.TurnipSoupName.Value;
            }
        }
    }

    [HarmonyPatch(typeof(ObjectDB))]
    internal static class PatchObjectDB
    {
        [HarmonyPatch(nameof(ObjectDB.Awake))]
        [HarmonyPrefix]
        public static void Awake_Prefix(ObjectDB __instance)
        {
            if (SceneManager.GetActiveScene().name != "main")
            {
                return;
            }

            foreach (var recipe in __instance.m_recipes)
            {
                if (recipe.m_item?.m_itemData.m_shared.m_name == "$item_turnipstew")
                {
                    foreach (var item in recipe.m_resources)
                    {
                        if (item.m_resItem.name == "RawMeat")
                        {
                            item.m_resItem = __instance.GetItemPrefab("MushroomYellow").GetComponent<ItemDrop>();
                        }
                    }
                }
            }
        }
    }
}