using HarmonyLib;

namespace SimpleSetAndCapeBonuses
{
    [HarmonyPatch]
    internal class GathererPatch
    {
        [HarmonyPatch(typeof(Pickable), nameof(Pickable.RPC_Pick))]
        public static void Prefix(Pickable __instance, long sender, ref int __state)
        {
            __state = __instance.m_amount;

            if (!__instance.m_nview.IsOwner())
            {
                return;
            }

            if (__instance.m_picked)
            {
                return;
            }

            Player player = Player.GetPlayer(sender);

            if (player == null)
            {
                player = Player.m_localPlayer;
            }

            if (player == null || !player.m_seman.HaveStatusEffect(PatchObjectDB.ragsSetBonus))
            {
                return;
            }

            if (IsForage(__instance))
            {
                if (SimpleSetAndCapeBonusesPlugin.ForagerSetBonusUsesRandomness.Value)
                {
                    __instance.m_amount += UnityEngine.Random.Range(0, 3);
                }
                else
                {
                    __instance.m_amount += 1;
                }
            }
        }

        [HarmonyPatch(typeof(Pickable), nameof(Pickable.RPC_Pick))]
        public static void Postfix(Pickable __instance, ref int __state)
        {
            __instance.m_amount = __state;
        }

        private static readonly string[] allowedPickables = new string[]
        {
                "Pickable_Branch",
                "Pickable_Stone",
                "Pickable_Flint",

                "Pickable_Mushroom",
                // intentional lower casing
                "Pickable_Mushroom_yellow",
                // intentional inclusion of unobtainable item for future mod
                "Pickable_Mushroom_blue",

                "Pickable_Dandelion",
                "Pickable_Thistle",

                "RaspberryBush",
                "BlueberryBush",
                "CloudberryBush",

                //"Pickable_Barley_Wild",
                //"Pickable_Flax_Wild",

                //"Pickable_Tar",
                //"Pickable_TarBig"
        };

        private static bool IsForage(Pickable pickup)
        {
            foreach (var item in allowedPickables)
            {
                // missing bracket is intentional in case its a clone of a clone
                if (pickup.name.StartsWith(item + "(Clone"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}