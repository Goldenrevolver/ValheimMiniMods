using UnityEngine;

namespace SortedMenus
{
    internal static class SkillExtension
    {
        internal static int CompareSkillsByConfigSetting(this Skills.Skill thisSkill, Skills.Skill otherSkill)
        {
            if (SortConfig.SkillsMenuSorting.Value == SortSkillsMenu.ByLevel)
            {
                // sort highest to lowest
                int comp = -thisSkill.CompareSkillLevel(otherSkill);

                if (comp != 0)
                {
                    return comp;
                }
            }

            return thisSkill.GetSkillTranslation().CompareTo(otherSkill.GetSkillTranslation());
        }

        internal static int CompareSkillLevel(this Skills.Skill thisSkill, Skills.Skill otherSkill)
        {
            int compare = Mathf.FloorToInt(thisSkill.m_level).CompareTo(Mathf.FloorToInt(otherSkill.m_level));

            if (compare != 0)
            {
                return compare;
            }

            return thisSkill.m_accumulator.CompareTo(otherSkill.m_accumulator);
        }

        internal static string GetSkillTranslation(this Skills.Skill skill)
        {
            return Localization.instance.Localize("$skill_" + skill.m_info.m_skill.ToString().ToLower());
        }
    }
}