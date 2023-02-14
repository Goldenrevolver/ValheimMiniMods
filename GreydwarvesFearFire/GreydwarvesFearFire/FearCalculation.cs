using System.Collections.Generic;

namespace GreydwarvesFearFire
{
    internal class FearCalculation
    {
        private static readonly HashSet<string> moddedGreydwarves = new HashSet<string>() { "RRR_GDThornweaver", "Bitterstump_DoD" };

        private static bool IsModdedGreydwarf(string name)
        {
            return !name.ToLower().Contains("spawner")
                && (name.ToLower().Contains("greyling")
                || name.ToLower().Contains("greydwar")
                || moddedGreydwarves.Contains(name));

            // intentional missing 'f' in "greydwar" to also find uses of 'greydwarves'
        }

        internal static void UpdateFearLevel(MonsterAI monsterAI)
        {
            if (FearFireConfig.RequireKillingTheElder.Value && !ZoneSystem.instance.GetGlobalKey(GlobalVars.defeatedElderKey))
            {
                return;
            }

            if (!monsterAI.m_character)
            {
                return;
            }

            var fearLevel = GetFearLevel(monsterAI);

            if (fearLevel == null)
            {
                return;
            }

            bool noFearOverride = GetResistanceBasedNoFearOverride(monsterAI);

            if (noFearOverride)
            {
                SetFearLevel(monsterAI, FearLevel.NoFear);
            }
            else
            {
                SetFearLevel(monsterAI, fearLevel.Value);
            }
        }

        private static bool IsAtLeastWeakTo(HitData.DamageModifier modifier)
        {
            return modifier == HitData.DamageModifier.Weak || modifier == HitData.DamageModifier.VeryWeak;
        }

        private static bool GetResistanceBasedNoFearOverride(MonsterAI monsterAI)
        {
            var conf = FearFireConfig.RequireFireWeakness.Value;

            if (conf == FireWeakness.UsuallyWeakToFire)
            {
                var innatefireModifier = monsterAI.m_character.m_damageModifiers.GetModifier(HitData.DamageType.Fire);

                if (!IsAtLeastWeakTo(innatefireModifier))
                {
                    return true;
                }
            }
            else if (conf == FireWeakness.CurrentlyWeakToFire)
            {
                // this applies status effects
                var currentFireModifier = monsterAI.m_character.GetDamageModifier(HitData.DamageType.Fire);

                if (!IsAtLeastWeakTo(currentFireModifier))
                {
                    return true;
                }
            }

            return false;
        }

        private static FearLevel? GetFearLevel(MonsterAI monsterAI)
        {
            string name = Utils.GetPrefabName(monsterAI.gameObject);

            if (name == "Greyling")
            {
                return FearFireConfig.GreylingFearLevel.Value;
            }

            if (name == "Greydwarf")
            {
                return FearFireConfig.GreydwarfFearLevel.Value;
            }

            if (name == "Greydwarf_Shaman")
            {
                return FearFireConfig.GreydwarfShamanFearLevel.Value;
            }

            if (name == "Greydwarf_Elite")
            {
                return FearFireConfig.GreydwarfBruteFearLevel.Value;
            }

            if (IsModdedGreydwarf(name))
            {
                if (monsterAI.m_character.IsBoss())
                {
                    return FearFireConfig.ModdedBossGreydwarfFearLevel.Value;
                }
                else
                {
                    return FearFireConfig.ModdedGreydwarfFearLevel.Value;
                }
            }

            return null;
        }

        private static void SetFearLevel(MonsterAI monsterAI, FearLevel fearLevel)
        {
            switch (fearLevel)
            {
                case FearLevel.Avoid:
                    monsterAI.m_avoidFire = true;
                    monsterAI.m_afraidOfFire = false;
                    break;

                case FearLevel.Afraid:
                    monsterAI.m_avoidFire = true;
                    monsterAI.m_afraidOfFire = true;
                    break;

                case FearLevel.NoFear:
                    monsterAI.m_avoidFire = false;
                    monsterAI.m_afraidOfFire = false;
                    break;
            }
        }
    }
}