using HarmonyLib;
using static HitData;

namespace TreeEnemiesWeakToChopDamage
{
    [HarmonyPatch]
    internal class PatchTooltip
    {
        [HarmonyPatch(typeof(HitData), nameof(HitData.ApplyResistance)), HarmonyPostfix]
        public static void ApplyResistance(HitData __instance, DamageModifiers modifiers, ref HitData.DamageModifier significantModifier)
        {
            if (!ChopConfig.PrioritiseChopDamageDisplayColor.Value)
            {
                return;
            }

            if (__instance.m_damage.m_chop > 0f && modifiers.m_chop != DamageModifier.Immune && modifiers.m_chop != DamageModifier.Ignore)
            {
                significantModifier = DamageModifier.Weak;
            }
        }

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