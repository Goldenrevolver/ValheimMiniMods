using HarmonyLib;
using static InstantlyDestroyBoatsAndCarts.InstantlyDestroyBoatsAndCartsPlugin;

namespace InstantlyDestroyBoatsAndCarts
{
    internal class BoatWagonPatches
    {
        [HarmonyPatch(typeof(Piece), nameof(Piece.Awake))]
        public static class Awake_Patch
        {
            private static void Postfix(Piece __instance)
            {
                Ship ship = __instance.GetComponentInChildren<Ship>();
                Vagon vagon = __instance.GetComponentInChildren<Vagon>();

                if (ship || vagon)
                {
                    __instance.m_canBeRemoved = true;
                }
            }
        }

        [HarmonyPatch(typeof(Piece), nameof(Piece.CanBeRemoved))]
        public static class CanBeRemoved_Patch
        {
            private static bool Prefix(Piece __instance, ref bool __result)
            {
                Ship ship = __instance.GetComponentInChildren<Ship>();
                Vagon cart = __instance.GetComponentInChildren<Vagon>();

                if (!ship && !cart)
                {
                    return true;
                }

                __result = false;

                var config = AllowDestroyFor.Value;

                if (ship != null && (config == AllowDestroy.OnlyBoats || config == AllowDestroy.Both))
                {
                    Container container = __instance.GetComponentInChildren<Container>();

                    if (!CanRemoveContainer(container))
                    {
                        return false;
                    }

                    // base game check from ship.CanBeRemoved(): check if players are on board
                    __result = ship.m_players.Count == 0;
                    return false;
                }

                if (cart != null && (config == AllowDestroy.OnlyCarts || config == AllowDestroy.Both))
                {
                    if (!CanRemoveContainer(cart.m_container))
                    {
                        return false;
                    }

                    __result = cart.InUse();
                    return false;
                }

                return false;
            }
        }

        // custom version of Container.CanRemove with a null check and config check
        private static bool CanRemoveContainer(Container container)
        {
            if (!container)
            {
                return false;
            }

            var isPrivate = container.m_privacy == Container.PrivacySetting.Private;

            if ((isPrivate || PreventWhenContainerIsNotEmpty.Value) && container.GetInventory().NrOfItems() > 0)
            {
                return false;
            }

            return true;
        }
    }
}