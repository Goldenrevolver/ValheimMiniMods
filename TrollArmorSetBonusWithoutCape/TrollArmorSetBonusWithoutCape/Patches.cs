using HarmonyLib;
using System.Linq;
using TrollArmorSetBonusWithoutCape;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TrollCapeSetBonusChange
{
    [HarmonyPatch(typeof(ObjectDB))]
    internal static class PatchObjectDB
    {
        internal const string trollCapeName = "CapeTrollHide";

        private static readonly string[] trollSetItems = new string[]
        {
            "ArmorTrollLeatherChest", "ArmorTrollLeatherLegs", "HelmetTrollLeather"
        };

        [HarmonyPatch(nameof(ObjectDB.Awake))]
        [HarmonyPrefix]
        public static void Awake_Postfix(ObjectDB __instance)
        {
            if (SceneManager.GetActiveScene().name != "main")
            {
                return;
            }

            foreach (GameObject gameObject in __instance.m_items)
            {
                if (gameObject.name == trollCapeName)
                {
                    var shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;

                    shared.m_setName = string.Empty;
                    shared.m_setSize = 0;
                    shared.m_setStatusEffect = null;
                    shared.m_movementModifier = TrollArmorSetBonusWithoutCapePlugin.TrollCapeMovementSpeed.Value;
                }

                if (trollSetItems.Contains(gameObject.name))
                {
                    gameObject.GetComponent<ItemDrop>().m_itemData.m_shared.m_setSize = 3;
                }
            }
        }
    }
}