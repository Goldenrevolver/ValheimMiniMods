using BepInEx.Configuration;
using ServerSync;

namespace TreeEnemiesWeakToChopDamage
{
    internal class ChopConfig
    {
        internal static ConfigSync serverSyncInstance;

        public static ConfigEntry<WeaknessLevel> TheElderChopWeakness;
        public static ConfigEntry<WeaknessLevel> AbominationChopWeakness;
        public static ConfigEntry<WeaknessLevel> VanillaGreydwarfChopWeakness;
        public static ConfigEntry<WeaknessLevel> ModdedGreydwarfChopWeakness;
        public static ConfigEntry<WeaknessLevel> OtherModdedTreeEnemyChopWeakness;

        public static ConfigEntry<ResistanceLevel> TheElderLightningResistance;
        public static ConfigEntry<ResistanceLevel> AbominationLightningResistance;
        public static ConfigEntry<ResistanceLevel> VanillaGreydwarfLightningResistance;
        public static ConfigEntry<ResistanceLevel> ModdedGreydwarfLightningResistance;
        public static ConfigEntry<ResistanceLevel> OtherModdedTreeEnemyLightningResistance;

        public static ConfigEntry<bool> LogNonGreydwarfEnemyResistanceChanges;
        public static ConfigEntry<bool> LogGreydwarfEnemyResistanceChanges;

        public static ConfigEntry<bool> UseServerSync;
        public static ConfigEntry<bool> AddChopDamageToAxeTooltip;

        // these default values are intentional to allow casting to HitData.DamageModifier
        public enum WeaknessLevel
        {
            IgnoreDefault = 5,
            Normal = 0,
            Weak = 2,
            VeryWeak = 6,
        }

        // these default values are intentional to allow casting to HitData.DamageModifier
        public enum ResistanceLevel
        {
            NormalDefault = 0,
            Resistant = 1,
            VeryResistant = 5,
            Immune = 4,
        }

        public enum GreydwarfCheck
        {
            OnlyBaseGame = 0,
            TryToFindModded = 1,
        }
    }
}