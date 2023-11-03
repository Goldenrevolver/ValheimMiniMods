using HarmonyLib;
using UnityEngine;
using static SneakCameraAndCollision.SneakConfig;

namespace SneakCameraAndCollision
{
    [HarmonyPatch]
    internal class CameraPatches
    {
        [HarmonyPatch(typeof(GameCamera), nameof(GameCamera.GetCameraBaseOffset)), HarmonyPostfix]
        private static void GetCameraBaseOffset(Player player, ref Vector3 __result)
        {
            if (player != Player.m_localPlayer)
            {
                return;
            }

            if (LowerCameraWhen.Value == CrouchPosition.Disabled)
            {
                return;
            }

            // if the base game or another mod changed the default, abort
            if (__result != player.m_eye.transform.position - player.transform.position)
            {
                return;
            }

            bool isCrouching = player.IsCrouching() && player.IsOnGround();

            if (!isCrouching)
            {
                return;
            }

            bool isCrouchWalking = player.IsSneaking();

            if (LowerCameraWhen.Value == CrouchPosition.Crouching
                || (LowerCameraWhen.Value == CrouchPosition.CrouchWalking && isCrouchWalking)
                || (LowerCameraWhen.Value == CrouchPosition.CrouchStanding && !isCrouchWalking))
            {
                __result += Vector3.up * -Mathf.Clamp(CameraHeightReduction.Value, CameraMin, CameraMax);
            }
        }
    }
}