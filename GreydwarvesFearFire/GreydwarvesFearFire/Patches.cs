using HarmonyLib;

namespace GreydwarvesFearFire
{
    [HarmonyPatch]
    internal static class Patches
    {
        [HarmonyPatch(typeof(MonsterAI), nameof(MonsterAI.UpdateAI)), HarmonyPrefix]
        private static void PatchMonsterAIUpdate(MonsterAI __instance)
        {
            if (GlobalVars.RequiresFearAIUpdate || FearFireConfig.RequireFireWeakness.Value == FireWeakness.CurrentlyWeakToFire)
            {
                GlobalVars.WaitingForFirstCreatureUpdate = false;
                FearCalculation.UpdateFearLevel(__instance);
            }
        }

        [HarmonyPatch(typeof(MonsterAI), nameof(MonsterAI.Start)), HarmonyPostfix]
        private static void PatchMonsterAIStart(MonsterAI __instance)
        {
            FearCalculation.UpdateFearLevel(__instance);
        }

        [HarmonyPatch(typeof(Character), nameof(Character.OnDeath)), HarmonyPostfix]
        private static void DetectElderDeath(Character __instance)
        {
            if (__instance.m_defeatSetGlobalKey == GlobalVars.defeatedElderKey)
            {
                GlobalVars.RequiresFearAIUpdate = true;
            }
        }
    }
}