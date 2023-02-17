using HarmonyLib;
using System.Collections.Generic;

namespace SortedMenus
{
    [HarmonyPatch]
    internal class SkillsMenuPatches
    {
        [HarmonyPriority(Priority.VeryLow)]
        [HarmonyPatch(typeof(Skills), nameof(Skills.GetSkillList)), HarmonyPostfix]
        private static void GetUISkillListPatch(ref List<Skills.Skill> __result)
        {
            if (SortedMenusPlugin.HasSkillsMenuIncompatibleModInstalled())
            {
                return;
            }

            if (SortConfig.SkillsMenuSorting.Value != SortSkillsMenu.Disabled)
            {
                __result.Sort((a, b) => a.CompareSkillsByConfigSetting(b));
            }
        }

        [HarmonyPriority(Priority.VeryLow)]
        [HarmonyPatch(typeof(Skills), nameof(Skills.RaiseSkill)), HarmonyPostfix]
        private static void RaiseSkillPatch(Skills __instance, Skills.SkillType skillType)
        {
            if (__instance.m_player != Player.m_localPlayer)
            {
                return;
            }

            if (SortConfig.UpdateSkillsMenuOnChange.Value == SortedMenus.UpdateSkillsMenu.EnabledExceptRunSkill
                && skillType == Skills.SkillType.Run)
            {
                return;
            }

            UpdateSkillsMenu(__instance.m_player);
        }

        [HarmonyPriority(Priority.LowerThanNormal)]
        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
        private static void Postfix(Humanoid __instance, bool __result)
        {
            // if the item wasn't equipped, skip
            if (!__result)
            {
                return;
            }

            if (__instance != Player.m_localPlayer)
            {
                return;
            }

            UpdateSkillsMenu(Player.m_localPlayer);
        }

        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UnequipItem))]
        private static void Postfix(Humanoid __instance)
        {
            if (__instance != Player.m_localPlayer)
            {
                return;
            }

            UpdateSkillsMenu(Player.m_localPlayer);
        }

        private static bool safeToCallSetup = false;

        [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup)), HarmonyPostfix]
        private static void AfterFirstSkillsDialogSetup()
        {
            safeToCallSetup = true;
        }

        [HarmonyPatch(typeof(Game), nameof(Game.Logout)), HarmonyPrefix]
        internal static void ResetOnLogout()
        {
            safeToCallSetup = false;
        }

        internal static void UpdateSkillsMenu(Player player)
        {
            if (!safeToCallSetup
                || SortedMenusPlugin.HasSkillsMenuIncompatibleModInstalled()
                || SortConfig.UpdateSkillsMenuOnChange.Value == SortedMenus.UpdateSkillsMenu.Disabled
                || !InventoryGui.instance || !InventoryGui.instance.m_skillsDialog
                || !InventoryGui.instance.m_skillsDialog.gameObject
                || !InventoryGui.instance.m_skillsDialog.gameObject.activeSelf
                || !Player.m_localPlayer || player != Player.m_localPlayer)
            {
                return;
            }

            InventoryGui.instance.m_skillsDialog.Setup(player);
        }
    }
}