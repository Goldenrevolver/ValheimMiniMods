using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Reflection;

namespace ResizableBossTrophies
{
    [BepInPlugin("goldenrevolver.ResizableBossTrophies", NAME, VERSION)]
    public class ResizableBossTrophiesPlugin : BaseUnityPlugin
    {
        public const string NAME = "Resizable Boss Trophies and more";

        public const string VERSION = "1.0";

        public static ConfigEntry<float> EikthyTrophySize;
        public static ConfigEntry<float> ElderTrophySize;
        public static ConfigEntry<float> BonemassTrophySize;
        public static ConfigEntry<float> ModerTrophySize;
        public static ConfigEntry<float> YagluthTrophySize;
        public static ConfigEntry<float> QueenTrophySize;

        public static ConfigEntry<float> TrollTrophySize;
        public static ConfigEntry<float> AbominationTrophySize;
        public static ConfigEntry<float> LeechTrophySize;
        public static ConfigEntry<float> WraithTrophySize;
        public static ConfigEntry<float> SerpentTrophySize;
        public static ConfigEntry<float> StoneGolemTrophySize;
        public static ConfigEntry<float> FulingBerserkerTrophySize;
        public static ConfigEntry<float> LoxTrophySize;

        protected void Awake()
        {
            LoadConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        private void LoadConfig()
        {
            var sectionName = "0 - Boss Trophies";

            EikthyTrophySize = Config.Bind(sectionName, nameof(EikthyTrophySize), 1.2f, string.Empty);
            ElderTrophySize = Config.Bind(sectionName, nameof(ElderTrophySize), 1.5f, string.Empty);
            BonemassTrophySize = Config.Bind(sectionName, nameof(BonemassTrophySize), 0.9f, string.Empty);
            ModerTrophySize = Config.Bind(sectionName, nameof(ModerTrophySize), 1.2f, string.Empty);
            YagluthTrophySize = Config.Bind(sectionName, nameof(YagluthTrophySize), 0.95f, string.Empty);
            QueenTrophySize = Config.Bind(sectionName, nameof(QueenTrophySize), 1f, string.Empty);

            //sectionName = "1 - Meadows";

            sectionName = "2 - Black Forest";

            TrollTrophySize = Config.Bind(sectionName, nameof(TrollTrophySize), 1f, string.Empty);

            sectionName = "3 - Swamp";

            AbominationTrophySize = Config.Bind(sectionName, nameof(AbominationTrophySize), 0.8f, string.Empty);
            LeechTrophySize = Config.Bind(sectionName, nameof(LeechTrophySize), 0.7f, string.Empty);
            WraithTrophySize = Config.Bind(sectionName, nameof(WraithTrophySize), 0.7f, string.Empty);

            sectionName = "4 - Mountains";

            StoneGolemTrophySize = Config.Bind(sectionName, nameof(StoneGolemTrophySize), 0.7f, string.Empty);

            sectionName = "5 - Plains";

            FulingBerserkerTrophySize = Config.Bind(sectionName, nameof(FulingBerserkerTrophySize), 0.8f, string.Empty);
            LoxTrophySize = Config.Bind(sectionName, nameof(LoxTrophySize), 0.7f, string.Empty);

            //sectionName = "6 - Mistlands";

            //sectionName = "7 - Deep North";

            //sectionName = "8 - Ashlands";

            sectionName = "9 - Ocean";

            SerpentTrophySize = Config.Bind(sectionName, nameof(SerpentTrophySize), 1f, string.Empty);
        }
    }
}