using HarmonyLib;
using static HitData;

namespace TreeEnemiesWeakToChopDamage
{
    [HarmonyPatch]
    internal class PatchTooltip
    {
        [HarmonyPatch(typeof(DamageTypes), nameof(DamageTypes.GetTooltipString), new[] { typeof(Skills.SkillType) }), HarmonyPostfix]
        public static void GetTooltipString(DamageTypes __instance, Skills.SkillType skillType, ref string __result)
        {
            if (!Player.m_localPlayer)
            {
                return;
            }

            if (skillType != Skills.SkillType.Axes)
            {
                return;
            }

            if (!ChopConfig.AddChopDamageToAxeTooltip.Value)
            {
                return;
            }

            if (__result == null || __result.Contains("$inventory_chop"))
            {
                return;
            }

            Player.m_localPlayer.GetSkills().GetRandomSkillRange(out float minFactor, out float maxFactor, skillType);

            if (__instance.m_chop != 0f)
            {
                __result += "\n$inventory_chop: " + __instance.DamageRange(__instance.m_chop, minFactor, maxFactor);
            }
        }
    }
}