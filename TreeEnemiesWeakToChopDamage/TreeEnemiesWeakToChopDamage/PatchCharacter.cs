using HarmonyLib;
using System.Collections.Generic;
using static HitData;
using static TreeEnemiesWeakToChopDamage.ChopConfig;

namespace TreeEnemiesWeakToChopDamage
{
    [HarmonyPatch]
    internal static class PatchCharacter
    {
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
            internal bool slashChange;

            public ResistanceChanges(WeaknessLevel chopChange, ResistanceLevel lightningChange, bool slashChange)
            {
                this.chopChange = (DamageModifier)chopChange;
                this.lightningChange = (DamageModifier)lightningChange;
                this.slashChange = slashChange;
            }
        }

        private static ResistanceChanges? GetResistanceChanges(string name, out bool isGreyDwarf)
        {
            isGreyDwarf = false;

            if (name == "GDKing" || name == "TentaRoot")
            {
                return new ResistanceChanges(TheElderChopWeakness.Value, TheElderLightningResistance.Value, IncreaseTheElderSlashResistanceByOne.Value);
            }

            if (name == "Abomination")
            {
                return new ResistanceChanges(AbominationChopWeakness.Value, AbominationLightningResistance.Value, IncreaseAbominationSlashResistanceByOne.Value);
            }

            if (IsVanillaGreydwarf(name))
            {
                isGreyDwarf = true;
                return new ResistanceChanges(VanillaGreydwarfChopWeakness.Value, VanillaGreydwarfLightningResistance.Value, IncreaseVanillaGreydwarfSlashResistanceByOne.Value);
            }

            if (IsModdedGreydwarf(name))
            {
                isGreyDwarf = true;
                return new ResistanceChanges(ModdedGreydwarfChopWeakness.Value, ModdedGreydwarfLightningResistance.Value, IncreaseModdedGreydwarfSlashResistanceByOne.Value);
            }

            if (IsModdedTreeEnemy(name))
            {
                return new ResistanceChanges(OtherModdedTreeEnemyChopWeakness.Value, OtherModdedTreeEnemyLightningResistance.Value, IncreaseOtherModdedTreeEnemySlashResistanceByOne.Value);
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

        private static void MaybeIncreaseSlashResistance(ref DamageModifiers modifiers, DamageModifier chopMod, bool shouldChangeSlashMod)
        {
            // skip if we weren't the ones to change it (some modded enemies have custom chop resistance)
            if (modifiers.m_chop != chopMod)
            {
                return;
            }

            // skip if config doesn't want it
            if (!shouldChangeSlashMod)
            {
                return;
            }

            // don't turn very resistant into immune/ ignore
            switch (modifiers.m_slash)
            {
                case DamageModifier.Resistant:
                    modifiers.m_slash = DamageModifier.VeryResistant;
                    break;

                case DamageModifier.Normal:
                    modifiers.m_slash = DamageModifier.Resistant;
                    break;

                case DamageModifier.Weak:
                    modifiers.m_slash = DamageModifier.Normal;
                    break;

                case DamageModifier.VeryWeak:
                    modifiers.m_slash = DamageModifier.Weak;
                    break;
            }
        }

        private static void ApplyResistanceChanges(ref DamageModifiers modifiers, ResistanceChanges resistanceChanges)
        {
            ApplyChopWeaknessIfIgnore(ref modifiers, resistanceChanges.chopChange);
            ApplyLightningWeaknessIfNeutral(ref modifiers, resistanceChanges.lightningChange);
            MaybeIncreaseSlashResistance(ref modifiers, resistanceChanges.chopChange, resistanceChanges.slashChange);
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

            Helper.LogChop(__instance.m_damageModifiers, $"{prefabName} before", isGreyDwarf);
            ApplyResistanceChanges(ref __instance.m_damageModifiers, resistanceChanges.Value);
            Helper.LogChop(__instance.m_damageModifiers, $"{prefabName} after", isGreyDwarf);

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