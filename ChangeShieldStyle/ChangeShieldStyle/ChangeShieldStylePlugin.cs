using BepInEx;
using HarmonyLib;
using System.Reflection;

namespace ChangeShieldStyle
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class ChangeShieldStylePlugin : BaseUnityPlugin
    {
        public const string GUID = "goldenrevolver.ChangeShieldStyle";
        public const string NAME = "Change Style Of Existing Shield";
        public const string VERSION = "1.1.0";

        protected void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch]
    internal static class Patches
    {
        [HarmonyPatch(typeof(CraftingStation), nameof(CraftingStation.UseItem)), HarmonyPrefix]
        public static bool UpdateShieldStylePatch(CraftingStation __instance, Humanoid user, ItemDrop.ItemData item, ref bool __result)
        {
            var stationName = Utils.GetPrefabName(__instance.gameObject);

            if (stationName != "piece_workbench" && stationName != "forge")
            {
                return true;
            }

            return !TryUpdateShieldStyle(user, item, ref __result);
        }

        public static bool TryUpdateShieldStyle(Humanoid user, ItemDrop.ItemData item, ref bool __result)
        {
            if (!user.IsPlayer() || user != Player.m_localPlayer || !user.TakeInput())
            {
                return false;
            }

            if (item.m_shared.m_itemType != ItemDrop.ItemData.ItemType.Shield
                && item.m_shared.m_name != "$item_cape_linen")
            {
                return false;
            }

            if (item.m_shared.m_icons == null || item.m_shared.m_icons.Length == 0)
            {
                return false;
            }

            if (item.m_variant < 0 || item.m_variant >= item.m_shared.m_icons.Length)
            {
                return false;
            }

            item.m_variant = (item.m_variant + 1) % item.m_shared.m_icons.Length;

            // in the base game, IsEquipable is always true for a shield, but you never know if a mod patches it
            if (item.IsEquipable() && user.IsItemEquiped(item))
            {
                // updates the visual equipment, otherwise you need to unequip and reequip it yourself
                user.UnequipItem(item);
                user.EquipItem(item);
            }

            __result = true;
            return true;
        }
    }
}