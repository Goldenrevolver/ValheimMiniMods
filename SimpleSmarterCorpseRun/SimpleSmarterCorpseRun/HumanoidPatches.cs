using HarmonyLib;
using static ItemDrop;

namespace SimpleSmarterCorpseRun
{
    internal class HumanoidPatches
    {
        [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.EquipItem))]
        public static class Equip_Patch
        {
            private static void Prefix(Humanoid __instance, ItemData item, bool __result, ref float __state)
            {
                if (!__result)
                {
                    return;
                }

                if (!(__instance is Player player) || player != Player.m_localPlayer)
                {
                    return;
                }

                __state = player.GetMaxCarryWeight();
            }

            private static void Postfix(Humanoid __instance, ItemData item, bool __result, ref float __state)
            {
                // if the item wasn't equipped, skip
                if (!__result)
                {
                    return;
                }

                if (!(__instance is Player player) || player != Player.m_localPlayer)
                {
                    return;
                }

                if (__state == player.GetMaxCarryWeight())
                {
                    // no carry weight change from equipping this item, so no need to update the buff
                    return;
                }

                if (CorpseRunMath.HaveCustomCorpseRunEffect(player, out SE_Stats oldCorpseRun))
                {
                    CorpseRunMath.UpdateCustomCorpseRunEffect(player, oldCorpseRun);
                }
            }
        }
    }
}