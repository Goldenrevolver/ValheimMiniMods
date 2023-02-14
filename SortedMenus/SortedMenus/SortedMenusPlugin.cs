using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using System.Reflection;

namespace SortedMenus
{
    [BepInPlugin("goldenrevolver.SortedMenus", NAME, VERSION)]
    public class SortedMenusPlugin : BaseUnityPlugin
    {
        public const string NAME = "Sorted Cooking, Crafting and Skills Menu";
        public const string VERSION = "1.1.0";

        private const string combineSkillsMod = "goldenrevolver.CombineSpearAndPolearmSkills";

        protected void Start()
        {
            SortConfig.LoadConfig(this);

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            if (HasCombinedSkillsModInstalled())
            {
                Helper.Log($"{NAME}: 'Combine Skills' is installed, disabling skills menu sorting");
            }
        }

        internal static bool HasCombinedSkillsModInstalled()
        {
            return Chainloader.PluginInfos.ContainsKey(combineSkillsMod);
        }
    }

    internal enum CraftingStationType
    {
        CookingPot,
        Forge,
        Other,
    }
}