using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TrollCapeSetBonusChange;

namespace TrollArmorSetBonusWithoutCape
{
    [BepInIncompatibility("goldenrevolver.CapeAndTorchResistanceChanges")]
    [BepInPlugin("goldenrevolver.TrollArmorSetBonusWithoutCape", NAME, VERSION)]
    public class TrollArmorSetBonusWithoutCapePlugin : BaseUnityPlugin
    {
        public const string NAME = "Troll Armor Set Bonus Without Cape, Water Resistant Cape";
        public const string VERSION = "1.0";

        public static ConfigEntry<float> TrollCapeMovementSpeed;
        public static ConfigEntry<WaterResistance> GiveTrollCapeWaterResistanceIfPossible;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            var sectionName = "Requires Logout";

            TrollCapeMovementSpeed = Config.Bind(sectionName, nameof(TrollCapeMovementSpeed), 0.05f, $"How much movement speed to add to the troll cape. Default is 5%. Set to zero to disable.");
            TrollCapeMovementSpeed.SettingChanged += SettingChanged;

            GiveTrollCapeWaterResistanceIfPossible = Config.Bind(sectionName, nameof(GiveTrollCapeWaterResistanceIfPossible), WaterResistance.Resistant, $"If Aedenthorns CustomArmorStats mod is installed, add water resistance to the troll cape.");
            GiveTrollCapeWaterResistanceIfPossible.SettingChanged += SettingChanged;
        }

        private void SettingChanged(object sender, System.EventArgs e)
        {
            ObjectDB.instance.GetItemPrefab(PatchObjectDB.trollCapeName).GetComponent<ItemDrop>().m_itemData.m_shared.m_movementModifier = TrollCapeMovementSpeed.Value;

            UpdateAedenStats();
        }

        protected void Start()
        {
            UpdateAedenStats();
        }

        private static string ParseWaterResistance(WaterResistance res)
        {
            switch (res)
            {
                case WaterResistance.Resistant:
                    return "\"Water:Resistant\"";

                case WaterResistance.Immune:
                    return "\"Water:Immune\"";

                case WaterResistance.ImmuneExceptSwimming:
                    return "\"Water:VeryResistant\"";

                default:
                    return string.Empty;
            }
        }

        private static void UpdateAedenStats()
        {
            var plugins = FindObjectsOfType<BaseUnityPlugin>();

            var found = plugins.Any(plugin => plugin.Info.Metadata.GUID == "aedenthorn.CustomArmorStats");

            if (!found)
            {
                return;
            }

            var assembly = Assembly.Load("CustomArmorStats");

            if (assembly != null)
            {
                var assetPath = Path.Combine(Path.GetDirectoryName(assembly.Location), "CustomArmorStats");
                Directory.CreateDirectory(assetPath);

                var type = assembly.GetTypes().First(a => a.IsClass && a.Name == "BepInExPlugin");
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
                var methodInfo = methods.First(t => t.Name == "GetArmorDataFromFiles");

                var rawJson = $"{{\"name\": \"CapeTrollHide\", \"armor\": 1.0, \"armorPerLevel\": 1.0, \"movementModifier\": {TrollCapeMovementSpeed.Value}, \"damageModifiers\": [{ParseWaterResistance(GiveTrollCapeWaterResistanceIfPossible.Value)}]}}";

                using (Stream s = new MemoryStream())
                using (BinaryReader r = new BinaryReader(s))
                using (FileStream fs = new FileStream(Path.Combine(assetPath, "CapeTrollHide.json"), FileMode.Create))
                using (BinaryWriter w = new BinaryWriter(fs))
                {
                    w.Write(Encoding.UTF8.GetBytes(rawJson));
                }

                methodInfo?.Invoke(null, new object[0]);
            }
        }

        public enum WaterResistance
        {
            Disabled = 0,
            Resistant = 1,
            ImmuneExceptSwimming = 2,
            Immune = 3,
        }
    }
}