using HarmonyLib;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CapeAndTorchResistanceChanges.CapeAndTorchResistanceChangesPlugin;
using static CapeAndTorchResistanceChanges.NewResistances;

namespace CapeAndTorchResistanceChanges
{
    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.DrainEquipedItemDurability))]
    internal static class Humanoid_DrainEquipedItemDurability_Patch
    {
        private static void Postfix(Humanoid __instance, ref ItemDrop.ItemData item, ref float dt)
        {
            if (EnableTorchChanges.Value != TorchChanges.ResistanceChangesAndDurability)
            {
                return;
            }

            if (!__instance.IsPlayer())
            {
                return;
            }

            if (!(__instance is Player player))
            {
                return;
            }

            if (item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Torch)
            {
                bool isInShelter = player.InShelter();
                bool isEnvFreezing = EnvMan.instance.IsFreezing();
                bool isEnvCold = EnvMan.instance.IsCold();
                bool isEnvWet = EnvMan.instance.IsWet();

                bool shouldGetColdOrFreezing = (isEnvCold || isEnvFreezing) && !isInShelter;
                bool shouldGetWet = isEnvWet && !player.m_underRoof;

                if (shouldGetWet || shouldGetColdOrFreezing)
                {
                    item.m_durability -= item.m_shared.m_durabilityDrain * dt;
                }
            }
        }
    }

    [HarmonyPatch(typeof(ObjectDB))]
    internal static class PatchObjectDB
    {
        internal static SE_Stats teleportBuff;

        private static readonly string[] trollSet = new string[] { "HelmetTrollLeather", "ArmorTrollLeatherChest", "ArmorTrollLeatherLegs" };

        private static HitData.DamageModifier ParseWaterResistance(WaterResistance res)
        {
            switch (res)
            {
                case WaterResistance.Resistant:
                    return HitData.DamageModifier.Resistant;

                case WaterResistance.Immune:
                    return HitData.DamageModifier.Immune;

                case WaterResistance.ImmuneExceptSwimming:
                    return HitData.DamageModifier.VeryResistant;

                default:
                    return HitData.DamageModifier.Normal;
            }
        }

        [HarmonyPatch(nameof(ObjectDB.Awake))]
        [HarmonyPostfix]
        public static void Awake_Postfix(ObjectDB __instance)
        {
            if (SceneManager.GetActiveScene().name != "main")
            {
                return;
            }

            var buff = ScriptableObject.CreateInstance<SE_Stats>();
            buff.name = "GoldensTeleportBuff";
            buff.m_ttl = 5; // duration
            buff.m_mods = new System.Collections.Generic.List<HitData.DamageModPair>
            {
                new HitData.DamageModPair() { m_type = (HitData.DamageType)NewDamageTypes.Cold, m_modifier = HitData.DamageModifier.Immune },
                new HitData.DamageModPair() { m_type = (HitData.DamageType)NewDamageTypes.Water, m_modifier = HitData.DamageModifier.Immune }
            };

            teleportBuff = buff;
            __instance.m_StatusEffects.Add(buff);

            foreach (GameObject gameObject in __instance.m_items)
            {
                if (EnableTorchChanges.Value != TorchChanges.Disabled && gameObject.name == "Torch")
                {
                    var shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;

                    shared.m_damageModifiers = new System.Collections.Generic.List<HitData.DamageModPair>
                        {
                            new HitData.DamageModPair() { m_type = (HitData.DamageType)NewDamageTypes.Cold, m_modifier = HitData.DamageModifier.Resistant },
                            new HitData.DamageModPair() { m_type = (HitData.DamageType)NewDamageTypes.Water, m_modifier = HitData.DamageModifier.Resistant }
                        };
                }

                bool msChanges = EnableCapeChanges.Value == CapeChanges.MovementSpeedBuffs || EnableCapeChanges.Value == CapeChanges.Both;
                bool resChanges = EnableCapeChanges.Value == CapeChanges.ResistanceChanges || EnableCapeChanges.Value == CapeChanges.Both;

                if (gameObject.name == "CapeTrollHide")
                {
                    var shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;

                    if (EnableTrollArmorSetBonusChange.Value)
                    {
                        shared.m_setName = string.Empty;
                        shared.m_setSize = 0;
                        shared.m_setStatusEffect = null;
                    }

                    if (msChanges)
                    {
                        shared.m_movementModifier = 0.05f;
                    }

                    if (resChanges)
                    {
                        shared.m_damageModifiers = new System.Collections.Generic.List<HitData.DamageModPair>
                        {
                            new HitData.DamageModPair() { m_type = (HitData.DamageType)NewDamageTypes.Water, m_modifier = ParseWaterResistance(TrollCapeWaterResistance.Value) }
                        };
                    }
                }

                if (EnableTrollArmorSetBonusChange.Value)
                {
                    if (trollSet.Contains(gameObject.name))
                    {
                        gameObject.GetComponent<ItemDrop>().m_itemData.m_shared.m_setSize = 3;
                    }
                }

                if (EnableCapeChanges.Value == CapeChanges.Disabled)
                {
                    continue;
                }

                if (gameObject.name == "CapeDeerHide")
                {
                    if (msChanges)
                    {
                        gameObject.GetComponent<ItemDrop>().m_itemData.m_shared.m_movementModifier = 0.03f;
                    }
                }

                if (gameObject.name == "CapeLinen")
                {
                    var shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;

                    if (msChanges)
                    {
                        shared.m_movementModifier = 0.1f;
                    }

                    if (resChanges)
                    {
                        shared.m_damageModifiers = new System.Collections.Generic.List<HitData.DamageModPair>
                        {
                            new HitData.DamageModPair() { m_type = (HitData.DamageType)NewDamageTypes.Cold, m_modifier = HitData.DamageModifier.Resistant }
                        };
                    }
                }

                if (gameObject.name == "CapeWolf")
                {
                    var shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;

                    if (resChanges)
                    {
                        shared.m_damageModifiers = new System.Collections.Generic.List<HitData.DamageModPair>
                        {
                            new HitData.DamageModPair() { m_type = (HitData.DamageType)NewDamageTypes.Cold, m_modifier = HitData.DamageModifier.VeryResistant }
                        };
                    }
                }

                if (gameObject.name == "CapeLox")
                {
                    var shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;

                    if (resChanges)
                    {
                        var mod = LoxCapeColdResistance.Value == ColdResistance.Immune ? HitData.DamageModifier.Immune : HitData.DamageModifier.VeryResistant;

                        shared.m_damageModifiers = new System.Collections.Generic.List<HitData.DamageModPair>
                        {
                            new HitData.DamageModPair() { m_type = (HitData.DamageType)NewDamageTypes.Cold, m_modifier = mod }
                        };
                    }
                }

                if (gameObject.name == "CapeFeather")
                {
                    var shared = gameObject.GetComponent<ItemDrop>().m_itemData.m_shared;

                    if (resChanges)
                    {
                        shared.m_damageModifiers = new System.Collections.Generic.List<HitData.DamageModPair>
                        {
                            new HitData.DamageModPair() { m_type = (HitData.DamageType)NewDamageTypes.Cold, m_modifier = HitData.DamageModifier.VeryResistant }
                        };
                    }
                }
            }
        }
    }
}