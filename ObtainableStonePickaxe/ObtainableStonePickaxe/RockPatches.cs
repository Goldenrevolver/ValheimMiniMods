using HarmonyLib;
using static ObtainableStonePickaxe.ObtainableStonePickaxePlugin;

namespace ObtainableStonePickaxe
{
    [HarmonyPatch]
    internal static class RockPatches
    {
        // arbitrary negative number, so you cannot destroy anything other than the ground without patches to permit it
        internal const int stonePickaxeTier = -18;

        [HarmonyPatch(typeof(ZLog), nameof(ZLog.LogWarning))]
        internal static class PatchLog
        {
            // suppress the log spam they added
            public static bool Prefix(object o)
            {
                return !(o is string s && s == $"No stat for mine tier: {stonePickaxeTier}");
            }
        }

        [HarmonyPatch(typeof(Destructible), nameof(Destructible.RPC_Damage))]
        internal static class PatchDestructible
        {
            public static void Prefix(Destructible __instance, HitData hit, ref int __state)
            {
                // || __instance.m_minToolTier > 0
                if (hit == null || hit.m_toolTier != stonePickaxeTier)
                {
                    return;
                }

                __state = __instance.m_minToolTier;

                if (__instance.m_spawnWhenDestroyed == null)
                {
                    if (__instance.m_onDestroyed != null)
                    {
                        var dropOnDestroyed = __instance.GetComponent<DropOnDestroyed>();

                        if (dropOnDestroyed != null)
                        {
                            var dropsOnlyStones = DropsOnlyStones(dropOnDestroyed.m_dropWhenDestroyed);

                            DebugLog($"Destructable {__instance.name}: {dropsOnlyStones}");

                            if (dropsOnlyStones)
                            {
                                __instance.m_minToolTier = stonePickaxeTier;
                            }

                            return;
                        }
                    }
                }
                else
                {
                    var mineRock5 = __instance.m_spawnWhenDestroyed.GetComponent<MineRock5>();

                    if (mineRock5 != null)
                    {
                        var dropsOnlyStones = DropsOnlyStones(mineRock5.m_dropItems);

                        DebugLog($"Destructable MineRock5 {mineRock5.name}: {dropsOnlyStones}");

                        if (dropsOnlyStones)
                        {
                            __instance.m_minToolTier = stonePickaxeTier;
                        }

                        return;
                    }

                    var mineRock = __instance.m_spawnWhenDestroyed.GetComponent<MineRock>();

                    if (mineRock != null)
                    {
                        var dropsOnlyStones = DropsOnlyStones(mineRock.m_dropItems);

                        DebugLog($"Destructable MineRock {mineRock.name}: {dropsOnlyStones}");

                        if (dropsOnlyStones)
                        {
                            __instance.m_minToolTier = stonePickaxeTier;
                        }

                        return;
                    }
                }

                //if (__instance.m_autoCreateFragments)
                //{
                //    Debug.LogWarning($"Destructable {__instance.name} with m_autoCreateFragments");
                //}
            }

            public static void Postfix(Destructible __instance, HitData hit, ref int __state)
            {
                if (hit != null && hit.m_toolTier == stonePickaxeTier)
                {
                    __instance.m_minToolTier = __state;
                }
            }
        }

        [HarmonyPatch(typeof(MineRock), nameof(MineRock.RPC_Hit))]
        internal static class PatchMineRock
        {
            public static void Prefix(MineRock __instance, HitData hit, ref int __state)
            {
                if (hit != null && hit.m_toolTier == stonePickaxeTier)
                {
                    __state = __instance.m_minToolTier;

                    var dropsOnlyStones = DropsOnlyStones(__instance.m_dropItems);

                    DebugLog($"MineRock {__instance.name}: {dropsOnlyStones}");

                    if (dropsOnlyStones)
                    {
                        __instance.m_minToolTier = stonePickaxeTier;
                    }
                }
            }

            public static void Postfix(MineRock __instance, HitData hit, ref int __state)
            {
                if (hit != null && hit.m_toolTier == stonePickaxeTier)
                {
                    __instance.m_minToolTier = __state;
                }
            }
        }

        [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.DamageArea))]
        internal static class PatchMineRock5
        {
            public static void Prefix(MineRock5 __instance, HitData hit, ref int __state)
            {
                if (hit != null && hit.m_toolTier == stonePickaxeTier)
                {
                    __state = __instance.m_minToolTier;

                    var dropsOnlyStones = DropsOnlyStones(__instance.m_dropItems);

                    DebugLog($"MineRock5 {__instance.name}: {dropsOnlyStones}");

                    if (dropsOnlyStones)
                    {
                        __instance.m_minToolTier = stonePickaxeTier;
                    }
                }
            }

            public static void Postfix(MineRock5 __instance, HitData hit, ref int __state)
            {
                if (hit != null && hit.m_toolTier == stonePickaxeTier)
                {
                    __instance.m_minToolTier = __state;
                }
            }
        }

        /// <summary>
        /// Whether the drop table can only drop stone (and at least one), does not check chances
        /// </summary>
        public static bool DropsOnlyStones(DropTable dropTable)
        {
            if (dropTable == null)
            {
                return false;
            }

            bool dropsStone = false;

            foreach (var item in dropTable.m_drops)
            {
                if (item.m_item == null)
                {
                    continue;
                }

                var drop = item.m_item.GetComponent<ItemDrop>();

                if (drop == null)
                {
                    continue;
                }

                if (drop.m_itemData.m_shared.m_name == "$item_stone")
                {
                    dropsStone = true;
                }
                else
                {
                    return false;
                }
            }

            return dropsStone;
        }
    }
}