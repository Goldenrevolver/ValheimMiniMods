using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BossAltarRuleOfThree
{
    [HarmonyPatch]
    internal static class YagluthPatches
    {
        internal const string rpc_updateYagluthItemStands = "BossAltarRuleOfThree_UpdateYagluthItemStands";

        [HarmonyPatch(typeof(ItemStand), nameof(ItemStand.Awake)), HarmonyPostfix]
        public static void ItemStand_Awake(ItemStand __instance)
        {
            if (!__instance.m_nview)
            {
                __instance.m_nview = __instance.m_netViewOverride ? __instance.m_netViewOverride : __instance.gameObject.GetComponent<ZNetView>();
            }

            if (!__instance.m_nview)
            {
                return;
            }

            __instance.m_nview.Register(rpc_updateYagluthItemStands, (l) => RPC_RequestRemoveItemStand(l, __instance));
        }

        public static void RPC_RequestRemoveItemStand(long _, ItemStand itemStand)
        {
            RemoveStand(itemStand);
        }

        [HarmonyPatch(typeof(OfferingBowl), nameof(OfferingBowl.Awake)), HarmonyPostfix]
        public static void OfferingBowl_Awake(OfferingBowl __instance)
        {
            if (__instance.gameObject.name != "offeraltar_goblinking" || __instance.m_bossPrefab.name != "GoblinKing")
            {
                return;
            }

            // using the plugin as the starter because its never unloaded or disabled
            BossAltarRuleOfThreePlugin.Instance.StartCoroutine(WaitThenUpdateItemStands(__instance));
        }

        private static IEnumerator WaitThenUpdateItemStands(OfferingBowl __instance)
        {
            yield return new WaitForSeconds(1);

            UpdateYagluthItemStands(__instance, false);
        }

        [HarmonyPatch(typeof(OfferingBowl), nameof(OfferingBowl.Interact)), HarmonyPrefix]
        public static void OfferingBowl_Interact(OfferingBowl __instance, Humanoid user, bool hold)
        {
            if (hold || __instance.IsBossSpawnQueued())
            {
                return;
            }

            if (!(user is Player player) || player != Player.m_localPlayer)
            {
                return;
            }

            if (__instance.m_useItemStands
                && __instance.gameObject.name == "offeraltar_goblinking"
                && __instance.m_bossPrefab.name == "GoblinKing")
            {
                UpdateYagluthItemStands(__instance, true);
            }
        }

        private static void UpdateYagluthItemStands(OfferingBowl __instance, bool isInteract)
        {
            List<ItemStand> activeList = __instance.CustomFindItemStands(Activity.OnlyActive, out ItemStand firstBaseHider, out ItemStand secondBaseHider, out int totalCount);

            if (activeList.Count != 5)
            {
                return;
            }

            ItemStand closestToFirst = null;
            float smallestDistToFirst = float.MaxValue;

            ItemStand closestToSecond = null;
            float smallestDistToSecond = float.MaxValue;

            foreach (var itemStand in activeList)
            {
                SetToCloser(firstBaseHider, ref smallestDistToFirst, ref closestToFirst, itemStand);
                SetToCloser(secondBaseHider, ref smallestDistToSecond, ref closestToSecond, itemStand);
            }

            if (!IsAllowedToRemoveStands(closestToFirst, closestToSecond, isInteract))
            {
                return;
            }

            if (isInteract)
            {
                closestToFirst.m_nview.InvokeRPC(ZNetView.Everybody, rpc_updateYagluthItemStands);
                closestToSecond.m_nview.InvokeRPC(ZNetView.Everybody, rpc_updateYagluthItemStands);
            }
            else
            {
                RemoveStand(closestToFirst);
                RemoveStand(closestToSecond);
            }
        }

        private static bool IsAllowedToRemoveStands(ItemStand firstStand, ItemStand secondStand, bool isInteract)
        {
            return isInteract || (!firstStand.HaveAttachment() && !secondStand.HaveAttachment());
        }

        private static bool RemoveStand(ItemStand itemStand)
        {
            foreach (Transform child in itemStand.gameObject.transform)
            {
                child.gameObject.SetActive(false);
            }

            // this drops the item if you are the owner
            itemStand.OnDestroyed();

            Object.Destroy(itemStand);

            return true;
        }

        private static void SetToCloser(ItemStand center, ref float closestDist, ref ItemStand closest, ItemStand candidate)
        {
            float newDist = Vector3.Distance(center.transform.position, candidate.transform.position);

            if (newDist < closestDist)
            {
                closestDist = newDist;
                closest = candidate;
            }
        }

        private enum Activity
        {
            OnlyActive,
            OnlyDisabled,
            Both,
        }

        // the biggest change to the base game 'FindItemStands' is that this also includes inactive game objects in the 'FindObjectsOfType' call
        private static List<ItemStand> CustomFindItemStands(this OfferingBowl offering, Activity activity, out ItemStand firstBaseHider, out ItemStand secondBaseHider, out int totalCount)
        {
            List<ItemStand> list = new List<ItemStand>();
            firstBaseHider = null;
            secondBaseHider = null;

            var allItemStands = Object.FindObjectsOfType<ItemStand>(true);
            totalCount = allItemStands.Length;

            foreach (ItemStand itemStand in allItemStands)
            {
                if (Vector3.Distance(offering.transform.position, itemStand.transform.position) > offering.m_itemstandMaxRange
                    || !itemStand.gameObject.name.StartsWith(offering.m_itemStandPrefix))
                {
                    continue;
                }

                if (!itemStand.gameObject.activeInHierarchy && itemStand.gameObject.name == offering.m_itemStandPrefix + " (3)")
                {
                    firstBaseHider = itemStand;
                }
                else if (!itemStand.gameObject.activeInHierarchy && itemStand.gameObject.name == offering.m_itemStandPrefix + " (4)")
                {
                    secondBaseHider = itemStand;
                }

                switch (activity)
                {
                    case Activity.OnlyActive:
                        if (itemStand.gameObject.activeInHierarchy)
                        {
                            list.Add(itemStand);
                        }
                        break;

                    case Activity.OnlyDisabled:
                        if (!itemStand.gameObject.activeInHierarchy)
                        {
                            list.Add(itemStand);
                        }
                        break;

                    case Activity.Both:
                        list.Add(itemStand);
                        break;
                }
            }

            return list;
        }
    }
}