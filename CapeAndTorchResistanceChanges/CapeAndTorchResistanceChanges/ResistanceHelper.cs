using static CapeAndTorchResistanceChanges.NewResistances;
using static HitData;

namespace CapeAndTorchResistanceChanges
{
    internal static class ResistanceHelper
    {
        internal static bool IsAtLeastResistant(DamageModifier damageModifier)
        {
            switch (damageModifier)
            {
                case DamageModifier.Immune:
                case DamageModifier.Ignore:
                case DamageModifier.Resistant:
                case DamageModifier.VeryResistant:
                    return true;

                default:
                    return false;
            }
        }

        internal static DamageModifier GetNewDamageTypeModifier(NewDamageTypes type, Player player)
        {
            DamageModPair modPair = new DamageModPair();

            foreach (var equipedItem in player.m_inventory.GetEquippedItems())
            {
                foreach (var newMod in equipedItem.m_shared.m_damageModifiers)
                {
                    if ((int)newMod.m_type != (int)type)
                    {
                        continue;
                    }

                    if (ShouldOverride(modPair.m_modifier, newMod.m_modifier))
                    {
                        modPair = newMod;
                    }
                }
            }

            foreach (StatusEffect statusEffect in player.m_seman.m_statusEffects)
            {
                if (!(statusEffect is SE_Stats stat))
                {
                    continue;
                }

                foreach (var newMod in stat.m_mods)
                {
                    if ((int)newMod.m_type != (int)type)
                    {
                        continue;
                    }

                    if (ShouldOverride(modPair.m_modifier, newMod.m_modifier))
                    {
                        modPair = newMod;
                    }
                }
            }

            return modPair.m_modifier;
        }

        // this is about equal to HitData.DamageModifiers.ShouldOverride, but resistance can no longer override immunity
        private static readonly int[] modifierCompare = new int[] { 6, 3, 5, 1, 0, 2, 4 };

        // here, very weak overrides normal resistance
        // private static readonly int[] modifierCompare2 = new int[] { 6, 4, 5, 1, 0, 2, 3 };

        private static bool ShouldOverride(DamageModifier oldModifier, DamageModifier newModifier)
        {
            if (TryShouldOverride(oldModifier, newModifier, out bool shouldOverride))
            {
                return shouldOverride;
            }
            else
            {
                return ShouldOverrideOld(oldModifier, newModifier);
            }
        }

        internal static bool TryShouldOverride(DamageModifier oldModifier, DamageModifier newModifier, out bool shouldOverride)
        {
            int oldMod = (int)oldModifier;
            int newMod = (int)newModifier;

            // in the base game, this should always be the case, but mods are mods
            // also more safe than Enum.IsDefined because valheim is still in development
            if (oldMod >= 0 && oldMod <= 6 && newMod >= 0 && newMod <= 6)
            {
                shouldOverride = modifierCompare[newMod] < modifierCompare[oldMod];
                return true;
            }

            shouldOverride = false;
            return false;
        }

        // this is HitData.DamageModifiers.ShouldOverride
        private static bool ShouldOverrideOld(HitData.DamageModifier oldMod, HitData.DamageModifier newMod)
        {
            return oldMod != HitData.DamageModifier.Ignore && (newMod == HitData.DamageModifier.Immune || ((oldMod != HitData.DamageModifier.VeryResistant || newMod != HitData.DamageModifier.Resistant) && (oldMod != HitData.DamageModifier.VeryWeak || newMod != HitData.DamageModifier.Weak)));
        }

        //[HarmonyPatch(typeof(DamageModifiers), nameof(DamageModifiers.ShouldOverride))]
        //[HarmonyPostfix]
        //private static void ShouldOverrideModifier_Post(DamageModifier a, DamageModifier b, ref bool __result)
        //{
        //    if (TryShouldOverride(a, b, out bool shouldOverride))
        //    {
        //        __result = shouldOverride;
        //    }
        //}
    }
}