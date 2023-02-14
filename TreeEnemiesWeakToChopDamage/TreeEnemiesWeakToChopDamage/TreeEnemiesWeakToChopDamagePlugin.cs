using BepInEx;
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
        public const string VERSION = "1.0.0";

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            serverSyncInstance = ServerSyncWrapper.CreateRequiredConfigSync(GUID, NAME, VERSION);

            var sectionName = "0 - Chop Weakness";

            AbominationChopWeakness = Config.BindSynced(serverSyncInstance, sectionName, nameof(AbominationChopWeakness), WeaknessLevel.Weak);
            ModdedGreydwarfChopWeakness = Config.BindSynced(serverSyncInstance, sectionName, nameof(ModdedGreydwarfChopWeakness), WeaknessLevel.Weak, $"Includes modded Greylings.");
            OtherModdedTreeEnemyChopWeakness = Config.BindSynced(serverSyncInstance, sectionName, nameof(OtherModdedTreeEnemyChopWeakness), WeaknessLevel.Weak);
            TheElderChopWeakness = Config.BindSynced(serverSyncInstance, sectionName, nameof(TheElderChopWeakness), WeaknessLevel.Weak);
            VanillaGreydwarfChopWeakness = Config.BindSynced(serverSyncInstance, sectionName, nameof(VanillaGreydwarfChopWeakness), WeaknessLevel.Normal, $"Includes Greylings.");

            sectionName = "1 - Lightning Resistance";

            AbominationLightningResistance = Config.BindSynced(serverSyncInstance, sectionName, nameof(AbominationLightningResistance), ResistanceLevel.Resistant, "Be aware that this overwrites the lightning weakness that the wet debuff in the swamp usually grants (since resistances always overwrite weaknesses).");
            ModdedGreydwarfLightningResistance = Config.BindSynced(serverSyncInstance, sectionName, nameof(ModdedGreydwarfLightningResistance), ResistanceLevel.Resistant);
            OtherModdedTreeEnemyLightningResistance = Config.BindSynced(serverSyncInstance, sectionName, nameof(OtherModdedTreeEnemyLightningResistance), ResistanceLevel.Resistant);
            TheElderLightningResistance = Config.BindSynced(serverSyncInstance, sectionName, nameof(TheElderLightningResistance), ResistanceLevel.Resistant);
            VanillaGreydwarfLightningResistance = Config.BindSynced(serverSyncInstance, sectionName, nameof(VanillaGreydwarfLightningResistance), ResistanceLevel.Resistant);

            sectionName = "2 - Other";

            AddChopDamageToAxeTooltip = Config.Bind(sectionName, nameof(AddChopDamageToAxeTooltip), true);
            UseServerSync = Config.BindForceEnabledSyncLocker(serverSyncInstance, sectionName, nameof(UseServerSync));

            sectionName = "9 - Debug Logs";

            LogGreydwarfEnemyResistanceChanges = Config.Bind(sectionName, nameof(LogGreydwarfEnemyResistanceChanges), false, $"Whether the mod will log changes to resistances of greydwarves into the console/ log file.");
            LogNonGreydwarfEnemyResistanceChanges = Config.Bind(sectionName, nameof(LogNonGreydwarfEnemyResistanceChanges), false, $"Whether the mod will log changes to resistances of the Elder or an abomination into the console/ log file.");
        }
    }
}