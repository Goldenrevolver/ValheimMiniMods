using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static HitData;
using static TreeEnemiesWeakToChopDamage.ChopConfig;

namespace TreeEnemiesWeakToChopDamage
{
    [HarmonyPatch]
    internal static class PatchCharacter
    {
        private static void Log(string message)
        {
            Debug.Log($"{TreeEnemiesWeakToChopDamagePlugin.NAME} {TreeEnemiesWeakToChopDamagePlugin.VERSION}: {(message != null ? message.ToString() : "null")}");
        }

        private static void LogChop(DamageModifiers mod, string infix, bool isGreyDwarf)
        {
            if (isGreyDwarf && !LogGreydwarfEnemyResistanceChanges.Value)
            {
                return;
            }

            if (!isGreyDwarf && !LogNonGreydwarfEnemyResistanceChanges.Value)
            {
                return;
            }

            Log($"Wooden enemy {infix} Chop: {mod.m_chop}, Lightning {mod.m_lightning}");
        }

        private static readonly HashSet<string> vanillaGreydwarves = new HashSet<string>() { "Greyling", "Greydwarf", "Greydwarf_Elite", "Greydwarf_Shaman" };

        private static readonly HashSet<string> moddedGreydwarves = new HashSet<string>() { "RRR_GDThornweaver", "Bitterstump_DoD" };

        private static readonly HashSet<string> moddedOtherTreeEnemies = new HashSet<string>() { "TreeEnt_DoD" };

        private static bool IsVanillaGreydwarf(string name)
        {
            return vanillaGreydwarves.Contains(name);
        }

        private static bool IsModdedGreydwarf(string name)
        {
            return !name.ToLower().Contains("spawner")
                && (name.ToLower().Contains("greyling")
                || name.ToLower().Contains("greydwar")
                || moddedGreydwarves.Contains(name));

            // intentional missing 'f' in "greydwar" to also find uses of 'greydwarves'
        }

        private static bool IsModdedTreeEnemy(string name)
        {
            return moddedOtherTreeEnemies.Contains(name);
        }

        internal struct ResistanceChanges
        {
            internal DamageModifier chopChange;
            internal DamageModifier lightningChange;

            public ResistanceChanges(WeaknessLevel chopChange, ResistanceLevel lightningChange)
            {
                this.chopChange = (DamageModifier)chopChange;
                this.lightningChange = (DamageModifier)lightningChange;
            }
        }

        private static ResistanceChanges? GetResistanceChanges(string name, out bool isGreyDwarf)
        {
            isGreyDwarf = false;

            if (name == "GDKing" || name == "TentaRoot")
            {
                return new ResistanceChanges(TheElderChopWeakness.Value, TheElderLightningResistance.Value);
            }

            if (name == "Abomination")
            {
                return new ResistanceChanges(AbominationChopWeakness.Value, AbominationLightningResistance.Value);
            }

            if (IsVanillaGreydwarf(name))
            {
                isGreyDwarf = true;
                return new ResistanceChanges(VanillaGreydwarfChopWeakness.Value, VanillaGreydwarfLightningResistance.Value);
            }

            if (IsModdedGreydwarf(name))
            {
                isGreyDwarf = true;
                return new ResistanceChanges(ModdedGreydwarfChopWeakness.Value, ModdedGreydwarfLightningResistance.Value);
            }

            if (IsModdedTreeEnemy(name))
            {
                return new ResistanceChanges(OtherModdedTreeEnemyChopWeakness.Value, OtherModdedTreeEnemyLightningResistance.Value);
            }

            return null;
        }

        private static void ApplyChopWeaknessIfIgnore(ref DamageModifiers modifiers, DamageModifier newMod)
        {
            // skip ones that were probably changed by other mods
            if (modifiers.m_chop == DamageModifier.Ignore)
            {
                modifiers.m_chop = newMod;
            }
        }

        private static void ApplyLightningWeaknessIfNeutral(ref DamageModifiers modifiers, DamageModifier newMod)
        {
            // skip ones that were probably changed by other mods
            if (modifiers.m_lightning == DamageModifier.Normal)
            {
                modifiers.m_lightning = newMod;
            }
        }

        private static void ApplyResistanceChanges(ref DamageModifiers modifiers, ResistanceChanges resistanceChanges)
        {
            ApplyChopWeaknessIfIgnore(ref modifiers, resistanceChanges.chopChange);
            ApplyLightningWeaknessIfNeutral(ref modifiers, resistanceChanges.lightningChange);
        }

        [HarmonyPatch(typeof(Character), nameof(Character.Start)), HarmonyPostfix]
        private static void PatchCharacterCreation(Character __instance)
        {
            if (__instance.name == null)
            {
                return;
            }

            string prefabName = Utils.GetPrefabName(__instance.gameObject);

            var resistanceChanges = GetResistanceChanges(prefabName, out bool isGreyDwarf);

            if (resistanceChanges == null)
            {
                return;
            }

            LogChop(__instance.m_damageModifiers, $"{prefabName} before", isGreyDwarf);
            ApplyResistanceChanges(ref __instance.m_damageModifiers, resistanceChanges.Value);
            LogChop(__instance.m_damageModifiers, $"{prefabName} after", isGreyDwarf);

            if (__instance.m_weakSpots == null)
            {
                return;
            }

            foreach (var weakSpot in __instance.m_weakSpots)
            {
                // no need to log this again
                ApplyResistanceChanges(ref weakSpot.m_damageModifiers, resistanceChanges.Value);
            }
        }
    }
}