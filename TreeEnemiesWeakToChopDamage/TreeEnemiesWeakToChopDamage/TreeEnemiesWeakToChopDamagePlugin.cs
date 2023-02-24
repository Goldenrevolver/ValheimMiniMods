using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;
using static TreeEnemiesWeakToChopDamage.ChopConfig;

namespace TreeEnemiesWeakToChopDamage
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class TreeEnemiesWeakToChopDamagePlugin : BaseUnityPlugin
    {
        public const string GUID = "goldenrevolver.TreeEnemiesWeakToChopDamage";
        public const string NAME = "Tree Enemies Are Weak To Axe Damage";
        public const string VERSION = "1.1.0";

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            Config.SaveOnConfigSet = false;

            LoadConfigInternal();

            Config.Save();
            Config.SaveOnConfigSet = true;
        }

        private void LoadConfigInternal()
        {
            serverSyncInstance = ServerSyncWrapper.CreateRequiredConfigSync(GUID, NAME, VERSION);

            var sectionName = "0 - Chop Weakness";

            AbominationChopWeakness = Config.BindSynced(serverSyncInstance, sectionName, nameof(AbominationChopWeakness), WeaknessLevel.Percent50);
            ModdedGreydwarfChopWeakness = Config.BindSynced(serverSyncInstance, sectionName, nameof(ModdedGreydwarfChopWeakness), WeaknessLevel.Percent50, $"Includes modded Greylings.");
            OtherModdedTreeEnemyChopWeakness = Config.BindSynced(serverSyncInstance, sectionName, nameof(OtherModdedTreeEnemyChopWeakness), WeaknessLevel.Percent50);
            TheElderChopWeakness = Config.BindSynced(serverSyncInstance, sectionName, nameof(TheElderChopWeakness), WeaknessLevel.Percent50);
            VanillaGreydwarfChopWeakness = Config.BindSynced(serverSyncInstance, sectionName, nameof(VanillaGreydwarfChopWeakness), WeaknessLevel.Percent50, $"Includes Greylings.");

            sectionName = "1 - Lightning Resistance";

            AbominationLightningResistance = Config.BindSynced(serverSyncInstance, sectionName, nameof(AbominationLightningResistance), ResistanceLevel.Resistant, "Be aware that this overwrites the lightning weakness that the wet debuff in the swamp usually grants (since resistances always overwrite weaknesses).");
            ModdedGreydwarfLightningResistance = Config.BindSynced(serverSyncInstance, sectionName, nameof(ModdedGreydwarfLightningResistance), ResistanceLevel.Resistant);
            OtherModdedTreeEnemyLightningResistance = Config.BindSynced(serverSyncInstance, sectionName, nameof(OtherModdedTreeEnemyLightningResistance), ResistanceLevel.Resistant);
            TheElderLightningResistance = Config.BindSynced(serverSyncInstance, sectionName, nameof(TheElderLightningResistance), ResistanceLevel.Resistant);
            VanillaGreydwarfLightningResistance = Config.BindSynced(serverSyncInstance, sectionName, nameof(VanillaGreydwarfLightningResistance), ResistanceLevel.Resistant);

            sectionName = "2 - Slash Resistance";

            string tooltip = "This is only applied if its not a modded enemy with custom chop resistance. 'very resistant' does not change into 'immune'.";

            IncreaseAbominationSlashResistanceByOne = Config.BindSynced(serverSyncInstance, sectionName, nameof(IncreaseAbominationSlashResistanceByOne), false, tooltip);
            IncreaseModdedGreydwarfSlashResistanceByOne = Config.BindSynced(serverSyncInstance, sectionName, nameof(IncreaseModdedGreydwarfSlashResistanceByOne), false, tooltip);
            IncreaseOtherModdedTreeEnemySlashResistanceByOne = Config.BindSynced(serverSyncInstance, sectionName, nameof(IncreaseOtherModdedTreeEnemySlashResistanceByOne), false, tooltip);
            IncreaseTheElderSlashResistanceByOne = Config.BindSynced(serverSyncInstance, sectionName, nameof(IncreaseTheElderSlashResistanceByOne), false, tooltip);
            IncreaseVanillaGreydwarfSlashResistanceByOne = Config.BindSynced(serverSyncInstance, sectionName, nameof(IncreaseVanillaGreydwarfSlashResistanceByOne), false, tooltip);

            sectionName = "3 - Other";

            AddChopDamageToAxeTooltip = Config.Bind(sectionName, nameof(AddChopDamageToAxeTooltip), true);
            PrioritiseChopDamageDisplayColor = Config.Bind(sectionName, nameof(PrioritiseChopDamageDisplayColor), true, "When an enemy/ tree is vulnerable to chop damage, the damage text will be colored yellow regardless of other resistances like slash damage.");
            UseServerSync = Config.BindForceEnabledSyncLocker(serverSyncInstance, sectionName, nameof(UseServerSync));

            bool oldValue = false;
            string oldSectionName = "2 - Other";

            if (Config.TryGetOldConfigValue(new ConfigDefinition(oldSectionName, "AddChopDamageToAxeTooltip"), ref oldValue))
            {
                AddChopDamageToAxeTooltip.Value = oldValue;
            }

            // simply delete the old value to clean up the config
            Config.TryGetOldConfigValue(new ConfigDefinition(oldSectionName, "UseServerSync"), ref oldValue);

            sectionName = "9 - Debug Logs";

            LogGreydwarfEnemyResistanceChanges = Config.Bind(sectionName, nameof(LogGreydwarfEnemyResistanceChanges), false, $"Whether the mod will log changes to resistances of greydwarves into the console/ log file.");
            LogNonGreydwarfEnemyResistanceChanges = Config.Bind(sectionName, nameof(LogNonGreydwarfEnemyResistanceChanges), false, $"Whether the mod will log changes to resistances of the Elder or an abomination into the console/ log file.");
        }
    }
}