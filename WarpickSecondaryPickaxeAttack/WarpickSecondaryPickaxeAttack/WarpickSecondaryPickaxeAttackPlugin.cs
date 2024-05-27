using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ServerSync;
using System.Reflection;

namespace WarpickSecondaryPickaxeAttack
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class WarpickSecondaryPickaxeAttackPlugin : BaseUnityPlugin
    {
        public const string GUID = "goldenrevolver.WarpickSecondaryPickaxeAttack";
        public const string NAME = "Warpick - Secondary Pickaxe Attack";
        public const string VERSION = "1.0.2";

        internal static ConfigSync serverSyncInstance;
        internal static ConfigEntry<bool> UseServerSync;
        public static ConfigEntry<bool> AddTerrainDamageToPickaxeTooltip;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            serverSyncInstance = ServerSyncWrapper.CreateRequiredConfigSync(GUID, NAME, VERSION);

            string sectionName = "General";

            UseServerSync = Config.BindHiddenForceEnabledSyncLocker(serverSyncInstance, sectionName, nameof(UseServerSync));
            AddTerrainDamageToPickaxeTooltip = Config.Bind(sectionName, nameof(AddTerrainDamageToPickaxeTooltip), true);
        }
    }
}