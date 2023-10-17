using HarmonyLib;

namespace BossAltarRuleOfThree
{
    [HarmonyPatch]
    internal class EikthyrBonemassPatches
    {
        private static bool IsEikthyrAltar(OfferingBowl offeringBowl)
        {
            return offeringBowl.gameObject.name == "offeraltar_deer" && offeringBowl.m_bossPrefab.name == "Eikthyr";
        }

        private static bool IsBonemassAltar(OfferingBowl offeringBowl)
        {
            return offeringBowl.transform.parent && offeringBowl.transform.parent.name == "offeraltar_bonemass" && offeringBowl.m_bossPrefab.name == "Bonemass";
        }

        [HarmonyPatch(typeof(OfferingBowl), nameof(OfferingBowl.UseItem)), HarmonyPrefix]
        public static void PreUseItem(OfferingBowl __instance, Humanoid user, ref int __state)
        {
            __state = __instance.m_bossItems;

            if (__instance.IsBossSpawnQueued())
            {
                return;
            }

            if (!(user is Player player) || player != Player.m_localPlayer)
            {
                return;
            }

            if (!__instance.m_useItemStands)
            {
                bool isEikthyr = BossAltarRuleOfThreePlugin.EnableEikthyrChange.Value && IsEikthyrAltar(__instance);
                bool isBonemass = BossAltarRuleOfThreePlugin.EnableBonemassChange.Value && IsBonemassAltar(__instance);

                if (isEikthyr || isBonemass)
                {
                    __instance.m_bossItems = 3;
                }
            }
        }

        [HarmonyPatch(typeof(OfferingBowl), nameof(OfferingBowl.UseItem)), HarmonyPostfix]
        public static void PostUseItem(OfferingBowl __instance, Humanoid user, int __state)
        {
            if (!(user is Player player) || player != Player.m_localPlayer)
            {
                return;
            }

            if (!__instance.m_useItemStands)
            {
                // always revert regardless of config in case it changed between prefix and postfix
                if (IsEikthyrAltar(__instance) || IsBonemassAltar(__instance))
                {
                    __instance.m_bossItems = __state;
                }
            }
        }
    }
}