using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using System.Reflection;

namespace BetterElderBuff
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class BetterElderBuffPlugin : BaseUnityPlugin
    {
        public const string GUID = "goldenrevolver.BetterElderBuffPlugin";
        public const string NAME = "Better Elder Buff";
        public const string VERSION = "1.1.0";

        internal static ConfigSync serverSyncInstance;
        internal static ConfigEntry<bool> UseServerSync;

        public static ConfigEntry<float> DamageModifier;
        public static ConfigEntry<bool> GivePickaxeDamage;
        public static ConfigEntry<int> BonusCarryingCapacity;
        public static ConfigEntry<string> Tooltip;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            serverSyncInstance = ServerSyncWrapper.CreateOptionalConfigSync(GUID, NAME, VERSION);

            var sectionName = "0 - General";

            UseServerSync = Config.BindSyncLocker(serverSyncInstance, sectionName, nameof(UseServerSync), true, string.Empty);

            DamageModifier = Config.BindSynced(serverSyncInstance, sectionName, nameof(DamageModifier), 1.6f, "The modifier to wood chopping and mining damage. 1.6 means 60% more damage. Values below 1 to reduce damage are not allowed and will be reset.");
            DamageModifier_SettingChanged(null, null);

            GivePickaxeDamage = Config.BindSynced(serverSyncInstance, sectionName, nameof(GivePickaxeDamage), true, "Whether the buff should grant mining damage in addition to wood chopping damage.");

            BonusCarryingCapacity = Config.BindSynced(serverSyncInstance, sectionName, nameof(BonusCarryingCapacity), 100, "Amount of bonus carrying capacity while the buff is active. Values below 0 are not allowed and will be reset.");
            MaxCarryWeight_SettingChanged(null, null);

            sectionName = "9 - Localization";

            Tooltip = Config.Bind(sectionName, nameof(Tooltip), "Your ability to chop wood, mine and carry is improved.", string.Empty);
        }

        private void DamageModifier_SettingChanged(object sender, System.EventArgs e)
        {
            DamageModifier.SettingChanged -= DamageModifier_SettingChanged;

            if (DamageModifier.Value < 1)
            {
                if (DamageModifier.Value >= 0)
                {
                    DamageModifier.Value += 1;
                }
                else
                {
                    DamageModifier.Value = 1;
                }
            }

            DamageModifier.SettingChanged += DamageModifier_SettingChanged;
        }

        private void MaxCarryWeight_SettingChanged(object sender, System.EventArgs e)
        {
            BonusCarryingCapacity.SettingChanged -= MaxCarryWeight_SettingChanged;

            if (BonusCarryingCapacity.Value < 0)
            {
                BonusCarryingCapacity.Value = 0;
            }

            BonusCarryingCapacity.SettingChanged += MaxCarryWeight_SettingChanged;
        }
    }
}