using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using System.Reflection;

namespace BossAltarRuleOfThree
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class BossAltarRuleOfThreePlugin : BaseUnityPlugin
    {
        public const string GUID = "goldenrevolver.BossAltarRuleOfThree";
        public const string NAME = "Boss Altar - Rule of Three";
        public const string VERSION = "1.1.0";

        internal static ConfigSync serverSyncInstance;
        internal static ConfigEntry<bool> UseServerSync;

        internal static ConfigEntry<bool> EnableEikthyrChange;
        internal static ConfigEntry<bool> EnableBonemassChange;

        internal static BossAltarRuleOfThreePlugin Instance { get; private set; }

        protected void Awake()
        {
            Instance = this;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            serverSyncInstance = ServerSyncWrapper.CreateRequiredConfigSync(GUID, NAME, VERSION);

            var sectionName = "General";

            UseServerSync = Config.BindHiddenForceEnabledSyncLocker(serverSyncInstance, sectionName, nameof(UseServerSync));
            Config.BindSynced(serverSyncInstance, sectionName, nameof(EnableEikthyrChange), true, string.Empty);

            Config.Bind(sectionName, "EnableYagluthChange", true, ConfigurationManagerAttributes.SeeOnlyDisplay("This option is always enabled. Changing this setting does nothing.", "Always Enabled", null));

            EnableEikthyrChange = Config.BindSynced(serverSyncInstance, sectionName, nameof(EnableEikthyrChange), true, string.Empty);
            EnableBonemassChange = Config.BindSynced(serverSyncInstance, sectionName, nameof(EnableBonemassChange), true, string.Empty);
        }
    }
}