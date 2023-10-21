using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace RawFishWithoutCauldron
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class RawFishWithoutCauldronPlugin : BaseUnityPlugin
    {
        public const string GUID = "goldenrevolver.RawFishWithoutCauldron";
        public const string NAME = "Quickly Cut Raw Fish Without Cauldron";
        public const string VERSION = "1.1.0";

        internal static ManualLogSource LogSource { get; private set; }

        protected void Awake()
        {
            LogSource = this.Logger;

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }
}