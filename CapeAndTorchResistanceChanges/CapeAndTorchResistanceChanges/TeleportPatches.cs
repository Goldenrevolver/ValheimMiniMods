using HarmonyLib;
using static CapeAndTorchResistanceChanges.TeleportHelper;

namespace CapeAndTorchResistanceChanges
{
    [HarmonyPatch(typeof(Player), nameof(Player.UpdateTeleport))]
    internal static class PatchPlayer
    {
        public static void Prefix(Player __instance, ref bool __state)
        {
            __state = __instance.m_teleporting;

            if (CapeAndTorchResistanceChangesPlugin.TeleportInstantlyUpdatesWeather.Value == TeleportChange.Disabled)
            {
                return;
            }

            if (!__instance.m_distantTeleport)
            {
                return;
            }

            if (!__instance.m_teleporting)
            {
                return;
            }

            if (__instance.m_teleportTimer == 0f)
            {
                RemoveAndBackupTransition(ref EnvMan.instance.m_wetTransitionDuration, ref oldWetTransitionDuration);
                RemoveAndBackupTransition(ref EnvMan.instance.m_windTransitionDuration, ref oldWindTransitionDuration);
                RemoveAndBackupTransition(ref EnvMan.instance.m_transitionDuration, ref oldTransitionDuration);
            }
        }

        public static void Postfix(Player __instance, ref bool __state)
        {
            if (!__instance.m_distantTeleport)
            {
                return;
            }

            if (__state && !__instance.m_teleporting)
            {
                if (CapeAndTorchResistanceChangesPlugin.TeleportGrantsTemporaryWetAndColdImmunity.Value)
                {
                    __instance.m_seman.AddStatusEffect(PatchObjectDB.teleportBuff);
                }

                if (CapeAndTorchResistanceChangesPlugin.TeleportInstantlyUpdatesWeather.Value != TeleportChange.Disabled)
                {
                    ResetTransition(ref EnvMan.instance.m_wetTransitionDuration, ref oldWetTransitionDuration, 15);
                    ResetTransition(ref EnvMan.instance.m_windTransitionDuration, ref oldWindTransitionDuration, 10);
                    ResetTransition(ref EnvMan.instance.m_transitionDuration, ref oldTransitionDuration, 10);
                }

                if (CapeAndTorchResistanceChangesPlugin.TeleportInstantlyUpdatesWeather.Value == TeleportChange.InstantlyUpdateWeatherAndClearWetAndColdDebuff)
                {
                    __instance.m_seman.RemoveStatusEffect("Wet");
                    __instance.m_seman.RemoveStatusEffect("Cold");
                    __instance.m_seman.RemoveStatusEffect("Freezing");
                }
            }
        }
    }

    public enum TeleportChange
    {
        Disabled = 0,
        InstantlyUpdateWeather = 1,
        InstantlyUpdateWeatherAndClearWetAndColdDebuff = 2,
    }

    internal static class TeleportHelper
    {
        internal static float oldWetTransitionDuration = float.Epsilon;
        internal static float oldTransitionDuration = float.Epsilon;
        internal static float oldWindTransitionDuration = float.Epsilon;

        internal static void ResetTransition(ref float transitionDuration, ref float preTeleportDuration, float defaultValue)
        {
            if (transitionDuration == float.Epsilon)
            {
                if (preTeleportDuration != float.Epsilon)
                {
                    transitionDuration = preTeleportDuration;
                    preTeleportDuration = float.Epsilon;
                }
                else
                {
                    // we lost the value somehow, just take the default one of the base game
                    transitionDuration = defaultValue;
                }
            }
        }

        internal static void RemoveAndBackupTransition(ref float transitionDuration, ref float backup)
        {
            if (transitionDuration != float.Epsilon)
            {
                backup = transitionDuration;
                transitionDuration = float.Epsilon;
            }
        }
    }
}