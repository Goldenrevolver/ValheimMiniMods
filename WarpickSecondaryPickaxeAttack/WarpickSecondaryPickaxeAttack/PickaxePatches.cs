using HarmonyLib;
using static HitData;

namespace WarpickSecondaryPickaxeAttack
{
    [HarmonyPatch]
    internal class PickaxePatches
    {
        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem)), HarmonyPrefix]
        public static void AddSecondaryAttackToPickaxe(ItemDrop.ItemData item)
        {
            if (item != null && item.m_shared.m_skillType == Skills.SkillType.Pickaxes)
            {
                if (item.m_shared.m_attack.m_attackAnimation == "swing_pickaxe" && !item.HaveSecondaryAttack())
                {
                    item.m_shared.m_secondaryAttack = item.m_shared.m_attack.Clone();

                    item.m_shared.m_secondaryAttack.m_attackAnimation = "swing_axe0";
                    //item.m_shared.m_secondaryAttack.m_attackAnimation = "greatsword0";
                    //item.m_shared.m_secondaryAttack.m_attackAnimation = "swing_longsword0";

                    item.m_shared.m_secondaryAttack.m_attackType = Attack.AttackType.Horizontal;
                    item.m_shared.m_secondaryAttack.m_attackAngle = 90f;
                    item.m_shared.m_secondaryAttack.m_attackRange = 2.2f;
                    item.m_shared.m_secondaryAttack.m_attackHeight = 1f;
                    item.m_shared.m_secondaryAttack.m_attackRayWidth = 0.3f;
                }
            }
        }

        [HarmonyPatch(typeof(ZSyncAnimation), nameof(ZSyncAnimation.SyncParameters))]
        [HarmonyPostfix]
        public static void SlowDownPickaxeSwings(ZSyncAnimation __instance)
        {
            var humanoid = __instance.GetComponent<Humanoid>();

            if (!humanoid || !humanoid.m_nview || humanoid.GetCurrentWeapon()?.m_shared?.m_skillType != Skills.SkillType.Pickaxes)
            {
                return;
            }

            foreach (var item in __instance.m_animator.GetCurrentAnimatorClipInfo(0))
            {
                if (item.clip != null && item.clip.name == "axe_swing")
                {
                    if (__instance.m_animator.speed == 1)
                    {
                        __instance.m_animator.speed = 0.6f;
                    }

                    break;
                }
            }
        }

        [HarmonyPatch(typeof(DamageTypes), nameof(DamageTypes.GetTooltipString), new[] { typeof(Skills.SkillType) }), HarmonyPostfix]
        public static void GetTooltipString(DamageTypes __instance, Skills.SkillType skillType, ref string __result)
        {
            if (!Player.m_localPlayer)
            {
                return;
            }

            if (skillType != Skills.SkillType.Pickaxes)
            {
                return;
            }

            if (!WarpickSecondaryPickaxeAttackPlugin.AddTerrainDamageToPickaxeTooltip.Value)
            {
                return;
            }

            if (__result == null || __result.Contains("$inventory_pickaxe"))
            {
                return;
            }

            Player.m_localPlayer.GetSkills().GetRandomSkillRange(out float minFactor, out float maxFactor, skillType);

            if (__instance.m_pickaxe != 0f)
            {
                __result += "\n$inventory_pickaxe: " + __instance.DamageRange(__instance.m_pickaxe, minFactor, maxFactor);
            }
        }
    }
}