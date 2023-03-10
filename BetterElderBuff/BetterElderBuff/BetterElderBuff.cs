using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;

namespace BetterElderBuff
{
    [BepInPlugin("goldenrevolver.BetterElderBuffPlugin", NAME, VERSION)]
    public class BetterElderBuffPlugin : BaseUnityPlugin
    {
        public const string NAME = "Better Elder Buff";

        public const string VERSION = "1.0.1";

        public static ConfigEntry<string> Tooltip;
        public static ConfigEntry<float> DamageModifier;
        public static ConfigEntry<bool> GivePickaxeDamage;
        public static ConfigEntry<int> MaxCarryWeight;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            var sectionName = "Requires Restart";

            Tooltip = Config.Bind(sectionName, nameof(Tooltip), "Your ability to chop wood, mine and carry is improved.", string.Empty);

            GivePickaxeDamage = Config.Bind(sectionName, nameof(GivePickaxeDamage), true, string.Empty);

            DamageModifier = Config.Bind(sectionName, nameof(DamageModifier), 1.6f, "The modifier to wood chopping and mining damage. 1.6 means 60% more damage. Values below 1 to reduce damage are not allowed and will be reset.");
            DamageModifier_SettingChanged(null, null);

            MaxCarryWeight = Config.Bind(sectionName, nameof(MaxCarryWeight), 100, string.Empty);
            MaxCarryWeight_SettingChanged(null, null);
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
            MaxCarryWeight.SettingChanged -= MaxCarryWeight_SettingChanged;

            if (MaxCarryWeight.Value < 0)
            {
                MaxCarryWeight.Value = 0;
            }

            MaxCarryWeight.SettingChanged += MaxCarryWeight_SettingChanged;
        }
    }
}