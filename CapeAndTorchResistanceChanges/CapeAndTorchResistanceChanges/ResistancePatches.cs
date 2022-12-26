using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static CapeAndTorchResistanceChanges.CapeAndTorchResistanceChangesPlugin;
using static CapeAndTorchResistanceChanges.ResistanceHelper;
using static HitData;

namespace CapeAndTorchResistanceChanges
{
    internal static class NewResistances
    {
        // default HitData.DamageType are flags, so we continue to use powers of 2
        internal enum NewDamageTypes
        {
            Water = 1024,
            Cold = 2048
        }

        [HarmonyPatch(typeof(SEMan), nameof(SEMan.AddStatusEffect), new Type[] { typeof(string), typeof(bool), typeof(int), typeof(float) })]
        private static class SEMan_AddStatusEffectByName_Patch
        {
            private static bool Prefix(SEMan __instance, string name, bool resetTime, ref StatusEffect __result)
            {
                // non resets will get handled by the other overload of the method
                // this is just so being wet from swimming cannot be refreshed by rain while being very resistant to water
                if (!resetTime || CanAddStatusEffectOverride(name, __instance.m_character))
                {
                    return true;
                }
                else
                {
                    __result = null;
                    return false;
                }
            }
        }

        [HarmonyPatch(typeof(SEMan), nameof(SEMan.AddStatusEffect), new Type[] { typeof(StatusEffect), typeof(bool), typeof(int), typeof(float) })]
        private static class SEMan_AddStatusEffectByType_Patch
        {
            private static bool Prefix(SEMan __instance, StatusEffect statusEffect, ref StatusEffect __result)
            {
                if (CanAddStatusEffectOverride(statusEffect.name, __instance.m_character))
                {
                    return true;
                }
                else
                {
                    __result = null;
                    return false;
                }
            }
        }

        private static bool CanAddStatusEffectOverride(string name, Character character)
        {
            if (!character.IsPlayer())
            {
                return true;
            }

            if (!(character is Player player))
            {
                return true;
            }

            if (name == "Wet")
            {
                var waterModifier = GetNewDamageTypeModifier(NewDamageTypes.Water, player);

                if (waterModifier == DamageModifier.Ignore || waterModifier == DamageModifier.Immune)
                {
                    return false;
                }

                if (waterModifier == DamageModifier.VeryResistant && !player.InLiquidSwimDepth())
                {
                    return false;
                }
            }

            if (name == "Cold" || name == "Freezing")
            {
                DamageModifier coldModifier = GetNewDamageTypeModifier(NewDamageTypes.Cold, player);

                if (coldModifier == DamageModifier.Ignore || coldModifier == DamageModifier.Immune)
                {
                    return false;
                }

                //bool isBurning = __instance.HaveStatusEffect("Burning");
                //bool isInWarmCozyArea = EffectArea.IsPointInsideArea(player.transform.position, EffectArea.Type.WarmCozyArea, 1f);

                //if (isBurning || isInWarmCozyArea)
                //{
                //    return false;
                //}

                //// the immune and ignore check is hidden in here
                //if (IsAtLeastResistant(coldModifier))
                //{
                //    bool isWet = __instance.HaveStatusEffect("Wet");
                //    bool cannotGetFreezing = coldModifier != DamageModifier.Resistant && (coldModifier != DamageModifier.VeryResistant || !isWet);

                //    if (name == "Cold" || cannotGetFreezing)
                //    {
                //        return false;
                //    }
                //}
            }

            return true;
        }

        [HarmonyPatch(typeof(SE_Wet), nameof(SE_Wet.UpdateStatusEffect))]
        private static class SE_Wet_UpdateStatusEffect_Patch
        {
            private static void Postfix(SE_Wet __instance, float dt)
            {
                if (!__instance.m_character.IsPlayer())
                {
                    return;
                }

                if (!(__instance.m_character is Player player))
                {
                    return;
                }

                DamageModifier waterModifier = GetNewDamageTypeModifier(NewDamageTypes.Water, player);

                // changed from the original
                if (waterModifier == DamageModifier.Resistant || waterModifier == DamageModifier.VeryResistant)
                {
                    __instance.m_time += dt * 3 / 2;
                }
                else if (waterModifier == DamageModifier.Weak)
                {
                    __instance.m_time -= dt / 3;
                }
                else if (waterModifier == DamageModifier.VeryWeak)
                {
                    __instance.m_time -= dt * 2 / 3;
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.UpdateEnvStatusEffects))]
        private static class Player_UpdateEnvStatusEffects_Patch
        {
            private static bool Prefix(Player __instance, float dt)
            {
                __instance.m_nearFireTimer += dt;
                DamageModifiers damageModifiers = __instance.GetDamageModifiers(null);
                bool isNearCampfire = __instance.m_nearFireTimer < 0.25f;
                bool isBurning = __instance.m_seman.HaveStatusEffect("Burning");
                bool isInShelter = __instance.InShelter();
                DamageModifier frostModifier = damageModifiers.GetModifier(DamageType.Frost);
                bool isEnvFreezing = EnvMan.instance.IsFreezing();
                bool isEnvCold = EnvMan.instance.IsCold();
                bool isEnvWet = EnvMan.instance.IsWet();
                bool isSensed = __instance.IsSensed();
                bool isSitting = __instance.IsSitting();
                bool isInWarmCozyArea = EffectArea.IsPointInsideArea(__instance.transform.position, EffectArea.Type.WarmCozyArea, 1f);
                bool shouldGetFreezing = isEnvFreezing && !isNearCampfire && !isInShelter;
                bool shouldGetCold = (isEnvCold && !isNearCampfire) || (isEnvFreezing && isNearCampfire && !isInShelter) || (isEnvFreezing && !isNearCampfire && isInShelter);

                if (isEnvWet && !__instance.m_underRoof)
                {
                    // this can fail due to resistances
                    __instance.m_seman.AddStatusEffect("Wet", true);
                }

                bool isWet = __instance.m_seman.HaveStatusEffect("Wet");

                if (IsAtLeastResistant(frostModifier) || isInWarmCozyArea || isBurning)
                {
                    shouldGetFreezing = false;
                    shouldGetCold = false;
                }
                else
                {
                    DamageModifier coldModifier = GetNewDamageTypeModifier(NewDamageTypes.Cold, __instance);

                    if (IsAtLeastResistant(coldModifier))
                    {
                        shouldGetCold = false;

                        if (coldModifier != DamageModifier.Resistant && (coldModifier != DamageModifier.VeryResistant || !isWet))
                        {
                            shouldGetFreezing = false;
                        }
                    }
                }

                if (isInShelter)
                {
                    __instance.m_seman.AddStatusEffect("Shelter", false);
                }
                else
                {
                    __instance.m_seman.RemoveStatusEffect("Shelter", false);
                }

                if (isNearCampfire)
                {
                    __instance.m_seman.AddStatusEffect("CampFire", false);
                }
                else
                {
                    __instance.m_seman.RemoveStatusEffect("CampFire", false);
                }

                bool restedConditions = !isSensed && (isSitting || isInShelter) && !shouldGetCold && !shouldGetFreezing && (!isWet || isInWarmCozyArea) && !isBurning && isNearCampfire;

                if (restedConditions)
                {
                    __instance.m_seman.AddStatusEffect("Resting", false);
                }
                else
                {
                    __instance.m_seman.RemoveStatusEffect("Resting", false);
                }

                __instance.m_safeInHome = (restedConditions && isInShelter && (float)__instance.GetBaseValue() >= 1f);

                if (shouldGetFreezing)
                {
                    if (!__instance.m_seman.RemoveStatusEffect("Cold", true))
                    {
                        __instance.m_seman.AddStatusEffect("Freezing", false);

                        return false;
                    }
                }
                else if (shouldGetCold)
                {
                    if (!__instance.m_seman.RemoveStatusEffect("Freezing", true) && __instance.m_seman.AddStatusEffect("Cold", false))
                    {
                        __instance.ShowTutorial("cold", false);

                        return false;
                    }
                }
                else
                {
                    __instance.m_seman.RemoveStatusEffect("Cold", false);
                    __instance.m_seman.RemoveStatusEffect("Freezing", false);
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(SE_Stats), nameof(SE_Stats.GetDamageModifiersTooltipString))]
        private static class SE_Stats_GetDamageModifiersTooltipString_Patch
        {
            private static void Postfix(ref string __result, List<HitData.DamageModPair> mods)
            {
                __result = Regex.Replace(__result, @"\n.*<color=orange></color>", "");
                foreach (HitData.DamageModPair damageModPair in mods)
                {
                    // if it's a vanilla damage type
                    if (Enum.IsDefined(typeof(HitData.DamageType), damageModPair.m_type))
                    {
                        continue;
                    }

                    if (damageModPair.m_modifier != HitData.DamageModifier.Ignore && damageModPair.m_modifier != HitData.DamageModifier.Normal)
                    {
                        switch (damageModPair.m_modifier)
                        {
                            case HitData.DamageModifier.Resistant:
                                __result += "\n$inventory_dmgmod: <color=orange>$inventory_resistant</color> VS ";
                                break;

                            case HitData.DamageModifier.Weak:
                                __result += "\n$inventory_dmgmod: <color=orange>$inventory_weak</color> VS ";
                                break;

                            case HitData.DamageModifier.Immune:
                                __result += "\n$inventory_dmgmod: <color=orange>$inventory_immune</color> VS ";
                                break;

                            case HitData.DamageModifier.VeryResistant:
                                __result += "\n$inventory_dmgmod: <color=orange>$inventory_veryresistant</color> VS ";
                                break;

                            case HitData.DamageModifier.VeryWeak:
                                __result += "\n$inventory_dmgmod: <color=orange>$inventory_veryweak</color> VS ";
                                break;
                        }

                        if ((int)damageModPair.m_type == (int)NewDamageTypes.Water)
                        {
                            __result += $"<color=orange>{WaterModifierLabel.Value}</color>";
                        }
                        else if ((int)damageModPair.m_type == (int)NewDamageTypes.Cold)
                        {
                            __result += $"<color=orange>{ColdModifierLabel.Value}</color>";
                        }
                    }
                }
            }
        }
    }
}