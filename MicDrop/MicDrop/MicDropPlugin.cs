using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static MicDrop.MicDropPlugin;

namespace MicDrop
{
    [BepInPlugin("goldenrevolver.MicDrop", NAME, VERSION)]
    public class MicDropPlugin : BaseUnityPlugin
    {
        public const string NAME = "Mic Drop - Drop Item Tweaks";
        public const string VERSION = "1.0.0";

        internal static ConfigEntry<Toggle> StopMomentum;
        internal static ConfigEntry<DropHeight> DropPosition;
        internal static ConfigEntry<DropForward> DropInFront;

        internal static ConfigEntry<DropHeight> DropPositionWhileCrouching;
        internal static ConfigEntry<DropHeight> DropPositionWhileHoldingKeybind;
        internal static ConfigEntry<KeyboardShortcut> HeldKeybind;

        internal enum Toggle
        {
            Enabled,
            Disabled
        }

        internal enum DropHeight
        {
            Disabled,
            RightHand,
            Chest,
            Feet
        }

        internal enum DropForward
        {
            Disabled,
            ForChestAndFeet,
            Always
        }

        protected void Start()
        {
            string sectionName = "0 - General";

            StopMomentum = Config.Bind(sectionName, nameof(StopMomentum), Toggle.Enabled, "Whether to throw or just drop the item.");
            DropPosition = Config.Bind(sectionName, nameof(DropPosition), DropHeight.Chest, "Drop position while not crouching and not holding the keybind.");
            DropInFront = Config.Bind(sectionName, nameof(DropInFront), DropForward.Disabled, "Whether to drop the item one meter in front of the drop position like the base game.");

            sectionName = "1 - Conditions";

            DropPositionWhileCrouching = Config.Bind(sectionName, nameof(DropPositionWhileCrouching), DropHeight.Feet, "Drop position while crouching.");
            DropPositionWhileHoldingKeybind = Config.Bind(sectionName, nameof(DropPositionWhileHoldingKeybind), DropHeight.Disabled, "Drop position while holding the keybind.");
            HeldKeybind = Config.Bind(sectionName, nameof(HeldKeybind), new KeyboardShortcut(KeyCode.None));

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(ItemDrop))]
    internal static class PatchThrow
    {
        public static bool IsKeyHeld(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKey(shortcut.MainKey) && shortcut.Modifiers.All(Input.GetKey);
        }

        [HarmonyPatch(nameof(ItemDrop.OnPlayerDrop))]
        [HarmonyPostfix]
        public static void OnPlayerDrop_Postfix(ItemDrop __instance)
        {
            Player closestPlayer = Player.GetClosestPlayer(__instance.transform.position, 5f);

            if (!closestPlayer || closestPlayer != Player.m_localPlayer)
            {
                return;
            }

            DropHeight dropPosition = DropPosition.Value;

            if (HeldKeybind.Value.IsKeyHeld())
            {
                dropPosition = DropPositionWhileHoldingKeybind.Value;
            }
            else if (closestPlayer.IsCrouching())
            {
                dropPosition = DropPositionWhileCrouching.Value;
            }

            switch (dropPosition)
            {
                case DropHeight.Disabled:
                    return;

                case DropHeight.RightHand:
                    if (closestPlayer.m_visEquipment && closestPlayer.m_visEquipment.m_rightHand)
                    {
                        __instance.transform.position = closestPlayer.m_visEquipment.m_rightHand.position;
                    }
                    else
                    {
                        goto case DropHeight.Chest;
                    }
                    break;

                case DropHeight.Chest:
                    __instance.transform.position = closestPlayer.transform.position + closestPlayer.transform.up;
                    break;

                case DropHeight.Feet:
                    __instance.transform.position = closestPlayer.transform.position;
                    break;
            }

            switch (DropInFront.Value)
            {
                case DropForward.ForChestAndFeet:
                    if (dropPosition != DropHeight.RightHand)
                    {
                        goto case DropForward.Always;
                    }
                    break;

                case DropForward.Always:
                    __instance.transform.position += closestPlayer.transform.forward;
                    break;
            }

            if (StopMomentum.Value == Toggle.Enabled)
            {
                var rigidbody = __instance.GetComponent<Rigidbody>();

                if (rigidbody)
                {
                    __instance.transform.rotation = Quaternion.identity;
                    rigidbody.velocity = Vector3.zero;

                    __instance.StartCoroutine(StopThrow(rigidbody));
                }
            }
        }

        private static IEnumerator StopThrow(Rigidbody body)
        {
            int lifeTimeFrames = 3;

            while (--lifeTimeFrames > 0)
            {
                if (body)
                {
                    body.velocity = Vector3.zero;
                }

                yield return null;
            }
        }
    }
}