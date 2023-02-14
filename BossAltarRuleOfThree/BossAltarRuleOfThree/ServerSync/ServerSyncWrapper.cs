using BepInEx.Configuration;
using ServerSync;
using System;

namespace BossAltarRuleOfThree
{
    internal static class ServerSyncWrapper
    {
        internal static ConfigSync CreateRequiredConfigSync(string guid, string displayName, string version)
        {
            return new ConfigSync(guid) { DisplayName = displayName, CurrentVersion = version, MinimumRequiredVersion = version, ModRequired = true };
        }

        internal static ConfigEntry<bool> BindHiddenForceEnabledSyncLocker(this ConfigFile configFile, ConfigSync serverSyncInstance, string group, string name)
        {
            ConfigEntry<bool> configEntry = configFile.Bind(group, name, true, HiddenDisplay("It's required that the mod is installed on the server and all clients. Changing this setting does nothing, ServerSync is always enabled."));
            configEntry.Value = true;

            serverSyncInstance.AddLockingConfigEntry(configEntry);

            return configEntry;
        }

        internal static ConfigDescription HiddenDisplay(string description, AcceptableValueBase acceptableValues = null)
        {
            return new ConfigDescription(description, acceptableValues, new ConfigurationManagerAttributes() { Browsable = false, ReadOnly = true });
        }

        internal static ConfigEntry<T> BindSyncLocker<T>(this ConfigFile configFile, ConfigSync serverSyncInstance, string group, string name, T value, ConfigDescription description) where T : IConvertible
        {
            ConfigEntry<T> configEntry = configFile.Bind(group, name, value, description);

            serverSyncInstance.AddLockingConfigEntry(configEntry);

            return configEntry;
        }

        internal static ConfigEntry<T> BindSyncLocker<T>(this ConfigFile configFile, ConfigSync serverSyncInstance, string group, string name, T value, string description) where T : IConvertible
        {
            return BindSyncLocker(configFile, serverSyncInstance, group, name, value, new ConfigDescription(description));
        }

        internal static ConfigEntry<T> BindSynced<T>(this ConfigFile configFile, ConfigSync serverSyncInstance, string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = configFile.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = serverSyncInstance.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        internal static ConfigEntry<T> BindSynced<T>(this ConfigFile configFile, ConfigSync serverSyncInstance, string group, string name, T value, string description, bool synchronizedSetting = true)
        {
            return BindSynced(configFile, serverSyncInstance, group, name, value, new ConfigDescription(description), synchronizedSetting);
        }
    }
}