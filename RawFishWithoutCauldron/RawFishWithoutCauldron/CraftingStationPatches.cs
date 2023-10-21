using HarmonyLib;

namespace RawFishWithoutCauldron
{
    [HarmonyPatch]
    internal class CraftingStationPatches
    {
        [HarmonyPatch(typeof(CookingStation), nameof(CookingStation.UseItem)), HarmonyPrefix]
        public static bool FilletFishPatch(CookingStation __instance, Humanoid user, ItemDrop.ItemData item, ref bool __result)
        {
            string stationName = Utils.GetPrefabName(__instance.gameObject);

            if (stationName != "piece_cookingstation" && stationName != "piece_cookingstation_iron")
            {
                return true;
            }

            return !FishCutter.TryFilletFish(user, item, ref __result);
        }

        [HarmonyPatch(typeof(CraftingStation), nameof(CraftingStation.UseItem)), HarmonyPrefix]
        public static bool FilletFishPatch(CraftingStation __instance, Humanoid user, ItemDrop.ItemData item, ref bool __result)
        {
            string stationName = Utils.GetPrefabName(__instance.gameObject);

            if (stationName != "piece_workbench")
            {
                return true;
            }

            return !FishCutter.TryFilletFish(user, item, ref __result);
        }
    }
}