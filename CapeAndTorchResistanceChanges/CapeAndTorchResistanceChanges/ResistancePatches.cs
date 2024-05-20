using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly List<DamageType> customDamageTypes = Enum.GetValues(typeof(NewDamageTypes)).Cast<DamageType>().ToList();

        [HarmonyPatch(typeof(SEMan), nameof(SEMan.AddStatusEffect), new Type[] { typeof(int), typeof(bool), typeof(int), typeof(float) })]
        private static class SEMan_AddStatusEffectByName_Patch
        {
            private static bool Prefix(SEMan __instance, int nameHash, bool resetTime, ref StatusEffect __result)
            {
                // non resets will get handled by the other overload of the method
                // this is just so being wet from swimming cannot be refreshed by rain while being very resistant to water
                var status = ObjectDB.instance.GetStatusEffect(nameHash);

                if (status == null || !resetTime || CanAddStatusEffectOverride(status.name, __instance.m_character))
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
                    __instance.m_seman.AddStatusEffect(Player.s_statusEffectWet, true);
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
                    __instance.m_seman.AddStatusEffect(Player.s_statusEffectShelter, false);
                }
                else
                {
                    __instance.m_seman.RemoveStatusEffect(Player.s_statusEffectShelter, false);
                }

                if (isNearCampfire)
                {
                    __instance.m_seman.AddStatusEffect(Player.s_statusEffectCampFire, false);
                }
                else
                {
                    __instance.m_seman.RemoveStatusEffect(Player.s_statusEffectCampFire, false);
                }

                bool restedConditions = !isSensed && (isSitting || isInShelter) && !shouldGetCold && !shouldGetFreezing && (!isWet || isInWarmCozyArea) && !isBurning && isNearCampfire;

                if (restedConditions)
                {
                    __instance.m_seman.AddStatusEffect(Player.s_statusEffectResting, false);
                }
                else
                {
                    __instance.m_seman.RemoveStatusEffect(Player.s_statusEffectResting, false);
                }

                __instance.m_safeInHome = (restedConditions && isInShelter && (float)__instance.GetBaseValue() >= 1f);

                if (shouldGetFreezing)
                {
                    if (!__instance.m_seman.RemoveStatusEffect(Player.s_statusEffectCold, true))
                    {
                        __instance.m_seman.AddStatusEffect(Player.s_statusEffectFreezing, false);

                        return false;
                    }
                }
                else if (shouldGetCold)
                {
                    if (!__instance.m_seman.RemoveStatusEffect(Player.s_statusEffectFreezing, true) && __instance.m_seman.AddStatusEffect(Player.s_statusEffectCold, false))
                    {
                        __instance.ShowTutorial("cold", false);

                        return false;
                    }
                }
                else
                {
                    __instance.m_seman.RemoveStatusEffect(Player.s_statusEffectCold, false);
                    __instance.m_seman.RemoveStatusEffect(Player.s_statusEffectFreezing, false);
                }

                return false;
            }
        }

        /// <summary>
        /// Intercept GetDamageModifiersTooltipString calls, filter out our custom mods, and handle their tooltip manually.
        /// </summary>
        [HarmonyPatch(typeof(SE_Stats), nameof(SE_Stats.GetDamageModifiersTooltipString))]
        private static class SE_Stats_GetDamageModifiersTooltipString_Patch
        {
            private static void Prefix(ref List<DamageModPair> mods, ref string __state)
            {
                var customMods = mods.Where(m => customDamageTypes.Contains(m.m_type));
                if (customMods.Any())
                {
                    var text = "";
                    foreach (var damageModPair in customMods)
                    {
                        switch (damageModPair.m_modifier)
                        {
                            case DamageModifier.Resistant:
                                text += "\n$inventory_dmgmod: <color=orange>$inventory_resistant</color> VS ";
                                break;
                            case DamageModifier.Weak:
                                text += "\n$inventory_dmgmod: <color=orange>$inventory_weak</color> VS ";
                                break;
                            case DamageModifier.Immune:
                                text += "\n$inventory_dmgmod: <color=orange>$inventory_immune</color> VS ";
                                break;
                            case DamageModifier.VeryResistant:
                                text += "\n$inventory_dmgmod: <color=orange>$inventory_veryresistant</color> VS ";
                                break;
                            case DamageModifier.VeryWeak:
                                text += "\n$inventory_dmgmod: <color=orange>$inventory_veryweak</color> VS ";
                                break;
                        }
                        text += "<color=orange>";
                        if (damageModPair.m_type == (DamageType)NewDamageTypes.Water) text += $"{WaterModifierLabel.Value}";
                        else if (damageModPair.m_type == (DamageType)NewDamageTypes.Cold) text += $"{ColdModifierLabel.Value}";
                        text += "</color>";
                    }

                    mods = mods.Where(m => !customDamageTypes.Contains(m.m_type)).ToList();
                    __state = text;
                }
            }

            private static void Postfix(ref string __result, string __state)
            {
                __result += __state;
            }
        }
    }
}
