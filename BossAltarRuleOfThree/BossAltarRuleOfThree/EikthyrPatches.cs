using HarmonyLib;

namespace BossAltarRuleOfThree
{
    [HarmonyPatch]
    internal class EikthyrPatches
    {
        [HarmonyPatch(typeof(OfferingBowl), nameof(OfferingBowl.UseItem)), HarmonyPrefix]
        public static void PreUseItem(OfferingBowl __instance, Humanoid user, ref int __state)
        {
            __state = __instance.m_bossItems;
            Helper.Log($"deer altar count: {__instance.m_bossItems}");

            if (__instance.IsBossSpawnQueued())
            {
                return;
            }

            if (!(user is Player player) || player != Player.m_localPlayer)
            {
                return;
            }

            if (!__instance.m_useItemStands
                && __instance.gameObject.name == "offeraltar_deer"
                && __instance.m_bossPrefab.name == "Eikthyr")
            {
                __instance.m_bossItems = 3;
            }
        }

        [HarmonyPatch(typeof(OfferingBowl), nameof(OfferingBowl.UseItem)), HarmonyPostfix]
        public static void PostUseItem(OfferingBowl __instance, Humanoid user, int __state)
        {
            if (__instance.IsBossSpawnQueued())
            {
                return;
            }

            if (!(user is Player player) || player != Player.m_localPlayer)
            {
                return;
            }

            if (!__instance.m_useItemStands
                && __instance.gameObject.name == "offeraltar_deer"
                && __instance.m_bossPrefab.name == "Eikthyr")
            {
                __instance.m_bossItems = __state;
            }
        }
    }
}