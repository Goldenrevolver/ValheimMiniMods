using HarmonyLib;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BetterElderBuff
{
    [HarmonyPatch(typeof(Localization))]
    internal static class PatchLocalization
    {
        [HarmonyPatch(nameof(Localization.Translate))]
        [HarmonyPostfix]
        public static void Translate_Postfix(Localization __instance, string word, ref string __result)
        {
            if (word == PatchObjectDB.elderPowerTooltip)
            {
                StringBuilder stringBuilder = new StringBuilder(256);

                stringBuilder.AppendFormat(BetterElderBuffPlugin.Tooltip.Value + '\n');
                stringBuilder.AppendFormat($"{__instance.Translate("inventory_chop")}: <color=orange>{BetterElderBuffPlugin.DamageModifier.Value * 100f:+0;-0}</color>\n");

                if (BetterElderBuffPlugin.GivePickaxeDamage.Value)
                {
                    stringBuilder.AppendFormat($"{__instance.Translate("inventory_pickaxe")}: <color=orange>{BetterElderBuffPlugin.DamageModifier.Value * 100f:+0;-0}</color>\n");
                }

                if (BetterElderBuffPlugin.MaxCarryWeight.Value > 0)
                {
                    stringBuilder.AppendFormat($"{__instance.Translate("se_max_carryweight")}: <color=orange>{BetterElderBuffPlugin.MaxCarryWeight.Value:+0;-0}</color>\n");
                }

                __result = stringBuilder.ToString();
            }
        }
    }

    [HarmonyPatch(typeof(ObjectDB))]
    internal static class PatchObjectDB
    {
        internal const string elderPowerName = "GP_TheElder";
        internal const string elderPowerTooltip = "se_theelder_tooltip";

        [HarmonyPatch(nameof(ObjectDB.Awake))]
        [HarmonyPrefix]
        public static void Show_Postfix(ObjectDB __instance)
        {
            if (SceneManager.GetActiveScene().name != "main")
            {
                return;
            }

            bool found = false;

            for (int i = 0; i < __instance.m_StatusEffects.Count; i++)
            {
                StatusEffect item = __instance.m_StatusEffects[i];

                if (item.name == elderPowerName)
                {
                    BetterElderBuff betterElder = ScriptableObject.CreateInstance<BetterElderBuff>();
                    found = true;

                    try
                    {
                        foreach (var field in typeof(SE_Stats).GetFields())
                        {
                            field.SetValue(betterElder, field.GetValue(item));
                        }

                        foreach (var field in typeof(StatusEffect).GetFields())
                        {
                            field.SetValue(betterElder, field.GetValue(item));
                        }

                        betterElder.name = item.name;

                        betterElder.m_addMaxCarryWeight = BetterElderBuffPlugin.MaxCarryWeight.Value;
                        betterElder.m_damageModifier = BetterElderBuffPlugin.DamageModifier.Value;

                        __instance.m_StatusEffects[i] = betterElder;
                    }
                    catch (System.Exception)
                    {
                        Debug.LogError("Copying Elder buff failed, aborting.");
                        __instance.m_StatusEffects[i] = item;
                    }
                    break;
                }
            }

            if (!found)
            {
                Debug.LogError("Couldn't find vanilla Elder buff. Something is wrong.");
            }
        }
    }

    internal class BetterElderBuff : SE_Stats
    {
        public override void ModifyAttack(Skills.SkillType skill, ref HitData hitData)
        {
            // do not call base!

            if (skill == Skills.SkillType.WoodCutting || (BetterElderBuffPlugin.GivePickaxeDamage.Value && skill == Skills.SkillType.Pickaxes))
            {
                hitData.m_damage.Modify(this.m_damageModifier);
            }
        }
    }
}