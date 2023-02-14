using HarmonyLib;
using System.Collections.Generic;

namespace SortedMenus
{
    [HarmonyPatch]
    internal class OverwritePatches
    {
        internal static Dictionary<string, string> importedNameOverwrites = new Dictionary<string, string>();
        internal static Dictionary<string, string> importedSortOverwrites = new Dictionary<string, string>();
        internal static readonly Dictionary<string, string> baseGameArmorSortOverwrites = new Dictionary<string, string>();

        [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Start)), HarmonyPostfix, HarmonyPriority(Priority.Last + 1)]
        private static void FejdStartup_Start_Postfix()
        {
            OverwriteFileLoader.LoadOverwrites();

            LoadBaseGameArmorSortOverwrites();
        }

        [HarmonyPatch(typeof(Localization), nameof(Localization.Translate)), HarmonyPostfix]
        public static void Localization_Translate_Postfix(string word, ref string __result)
        {
            if (!SortConfig.EnableCustomNameOverrides.Value)
            {
                return;
            }

            if (importedNameOverwrites.ContainsKey(word))
            {
                __result = importedNameOverwrites[word];
            }
        }

        private static readonly string[] uniformVanillaArmorSets = new string[] { "bronze", "carapace", "iron", "fenris", "leather", "mage", "trollleather", "root" };

        private static void LoadBaseGameArmorSortOverwrites()
        {
            baseGameArmorSortOverwrites.Clear();

            foreach (var set in uniformVanillaArmorSets)
            {
                string chest = $"$item_chest_{set}";

                baseGameArmorSortOverwrites[$"$item_legs_{set}"] = chest;
                baseGameArmorSortOverwrites[$"$item_helmet_{set}"] = chest;
            }

            baseGameArmorSortOverwrites["$item_legs_pgreaves"] = "$item_chest_pcuirass";
            baseGameArmorSortOverwrites["$item_helmet_padded"] = "$item_chest_pcuirass";

            baseGameArmorSortOverwrites["$item_legs_wolf"] = "$item_chest_wolf";
            baseGameArmorSortOverwrites["$item_helmet_drake"] = "$item_chest_wolf";

            baseGameArmorSortOverwrites["$item_legs_rags"] = "$item_chest_rags";

            baseGameArmorSortOverwrites["$item_cape_deerhide"] = "$item_chest_leather";
            baseGameArmorSortOverwrites["$item_cape_trollhide"] = "$item_chest_trollleather";

            // different crafting station, but still
            baseGameArmorSortOverwrites["$item_cape_wolf"] = "$item_chest_wolf";

            // for upgrading
            baseGameArmorSortOverwrites["$item_cape_odin"] = "$item_helmet_odin";
        }
    }
}