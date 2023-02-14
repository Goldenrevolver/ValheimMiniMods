using HarmonyLib;
using UnityEngine;
using static SneakCameraAndCollision.SneakConfig;

namespace SneakCameraAndCollision
{
    [HarmonyPatch]
    internal class CollisionPatches
    {
        private static float? standingSize = null;
        private static float? crouchedSize = null;

        [HarmonyPatch(typeof(Player), nameof(Player.FixedUpdate)), HarmonyPostfix]
        private static void FixedUpdate(Player __instance)
        {
            if (__instance != Player.m_localPlayer)
            {
                return;
            }

            if (!ChangeCollisionCrouchHeight.Value)
            {
                return;
            }

            var collider = __instance.GetCollider();

            if (!collider)
            {
                return;
            }

            if (standingSize == null)
            {
                standingSize = collider.height;
                CalculateCrouchedSize();
            }

            var isCrouched = __instance.IsCrouching() && __instance.IsOnGround();

            collider.height = isCrouched ? crouchedSize.Value : standingSize.Value;
            collider.center = new Vector3(0f, collider.height / 2f, 0f);
        }

        internal static void CalculateCrouchedSize()
        {
            if (standingSize == null || CrouchHeightMultiplier == null)
            {
                return;
            }

            crouchedSize = standingSize.Value * Mathf.Clamp(CrouchHeightMultiplier.Value, HeightMultMin, HeightMultMax);
        }
    }
}