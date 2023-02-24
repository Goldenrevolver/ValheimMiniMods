using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HitData;
using static TreeEnemiesWeakToChopDamage.ChopConfig;

namespace TreeEnemiesWeakToChopDamage
{
    internal static class Helper
    {
        public static bool TryGetOldConfigValue<T>(this ConfigFile config, ConfigDefinition configDefinition, ref T oldValue, bool removeIfFound = true)
        {
            if (!TomlTypeConverter.CanConvert(typeof(T)))
            {
                throw new ArgumentException(string.Format("Type {0} is not supported by the config system. Supported types: {1}", typeof(T), string.Join(", ", (from x in TomlTypeConverter.GetSupportedTypes() select x.Name).ToArray())));
            }

            try
            {
                var iolock = AccessTools.FieldRefAccess<ConfigFile, object>("_ioLock").Invoke(config);
                var orphanedEntries = (Dictionary<ConfigDefinition, string>)AccessTools.PropertyGetter(typeof(ConfigFile), "OrphanedEntries").Invoke(config, new object[0]);

                lock (iolock)
                {
                    if (orphanedEntries.TryGetValue(configDefinition, out string oldValueString))
                    {
                        oldValue = (T)TomlTypeConverter.ConvertToValue(oldValueString, typeof(T));

                        if (removeIfFound)
                        {
                            orphanedEntries.Remove(configDefinition);
                        }

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                LogWarning($"Error getting orphaned entry: {e.StackTrace}");
            }

            return false;
        }

        internal static void Log(string message)
        {
            Debug.Log($"{TreeEnemiesWeakToChopDamagePlugin.NAME} {TreeEnemiesWeakToChopDamagePlugin.VERSION}: {(message != null ? message.ToString() : "null")}");
        }

        internal static void LogWarning(string message)
        {
            Debug.LogWarning($"{TreeEnemiesWeakToChopDamagePlugin.NAME} {TreeEnemiesWeakToChopDamagePlugin.VERSION}: {(message != null ? message.ToString() : "null")}");
        }

        internal static void LogChop(DamageModifiers mod, string infix, bool isGreyDwarf)
        {
            if (isGreyDwarf && !LogGreydwarfEnemyResistanceChanges.Value)
            {
                return;
            }

            if (!isGreyDwarf && !LogNonGreydwarfEnemyResistanceChanges.Value)
            {
                return;
            }

            Log($"Wooden enemy {infix} Chop: {mod.m_chop.ToDamageNumber()}, Lightning: {mod.m_lightning.ToDamageNumber()}, Slash: {mod.m_slash.ToDamageNumber()}");
        }

        private static string ToDamageNumber(this DamageModifier damageModifier)
        {
            switch (damageModifier)
            {
                case DamageModifier.Immune:
                case DamageModifier.Ignore:
                    return "0%";

                case DamageModifier.VeryResistant:
                    return "25%";

                case DamageModifier.Resistant:
                    return "50%";

                case DamageModifier.Normal:
                    return "100%";

                case DamageModifier.Weak:
                    return "150%";

                case DamageModifier.VeryWeak:
                    return "200%";

                default:
                    return "invalid";
            }
        }
    }
}