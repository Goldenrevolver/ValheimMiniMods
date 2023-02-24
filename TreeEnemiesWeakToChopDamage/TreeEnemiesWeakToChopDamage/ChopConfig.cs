using BepInEx.Configuration;
using ServerSync;
using static TreeEnemiesWeakToChopDamage.ServerSyncWrapper;

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

        public static ConfigEntry<bool> IncreaseTheElderSlashResistanceByOne;
        public static ConfigEntry<bool> IncreaseAbominationSlashResistanceByOne;
        public static ConfigEntry<bool> IncreaseVanillaGreydwarfSlashResistanceByOne;
        public static ConfigEntry<bool> IncreaseModdedGreydwarfSlashResistanceByOne;
        public static ConfigEntry<bool> IncreaseOtherModdedTreeEnemySlashResistanceByOne;

        public static ConfigEntry<bool> LogNonGreydwarfEnemyResistanceChanges;
        public static ConfigEntry<bool> LogGreydwarfEnemyResistanceChanges;

        public static ConfigEntry<SyncEnabled> UseServerSync;
        public static ConfigEntry<bool> AddChopDamageToAxeTooltip;
        public static ConfigEntry<bool> PrioritiseChopDamageDisplayColor;

        // these default values are intentional to allow casting to HitData.DamageModifier
        public enum WeaknessLevel
        {
            Percent0Default = 4,
            Percent25 = 5,
            Percent50 = 1,
            Percent100 = 0,
            Percent150 = 2,
            Percent200 = 6,
        }

        //public enum WeaknessLevel
        //{
        //    ImmuneDefault = 4,
        //    VeryResistant = 5,
        //    Resistant = 1,
        //    Normal = 0,
        //    Weak = 2,
        //    VeryWeak = 6,
        //}

        // these default values are intentional to allow casting to HitData.DamageModifier
        public enum ResistanceLevel
        {
            NormalDefault = 0,
            Resistant = 1,
            VeryResistant = 5,
            Immune = 4,
        }
    }
}