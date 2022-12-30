using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ObtainableBlueMushrooms.PatchObjectDB;

namespace ObtainableBlueMushrooms
{
    internal class PlantableBlueMushroom
    {
        internal static int pieceNonSolidLayerMask;

        public static void InitPieceAndCultivator()
        {
            ItemDrop cultivator = cultivatorPrefab.GetComponent<ItemDrop>();

            var piece = InitBlueMushroomPiece();

            if (!cultivator.m_itemData.m_shared.m_buildPieces.m_pieces.Contains(piece.gameObject))
            {
                cultivator.m_itemData.m_shared.m_buildPieces.m_pieces.Add(piece.gameObject);
            }

            cultivator.m_itemData.m_shared.m_buildPieces.m_canRemovePieces = true;

            var onion = cultivator.m_itemData.m_shared.m_buildPieces.m_pieces.First((p) => p.name == "sapling_onion");

            var sphereCollider = piece.GetComponentInChildren<SphereCollider>();

            //Debug.LogWarning(sphereCollider.radius);
            // default is 0.3, which is a bit too big, it makes other mushrooms float when colliding with it
            sphereCollider.radius /= 2;

            //Debug.LogWarning(LayerMask.LayerToName(onion.layer));
            //Debug.LogWarning(LayerMask.LayerToName(blueMushroomPrefab.layer));
            // move them from the 'item' layer to the 'piece_nonsolid' layer, so they can collide with non ground in dungeons
            sphereCollider.gameObject.layer = onion.GetComponentInChildren<Collider>().gameObject.layer;

            var layer = LayerMask.LayerToName(sphereCollider.gameObject.layer);
            pieceNonSolidLayerMask = LayerMask.GetMask(new string[] { layer });

            piece.m_placeEffect.m_effectPrefabs = onion.GetComponent<Piece>().m_placeEffect.m_effectPrefabs;
        }

        private static Piece InitBlueMushroomPiece()
        {
            var piece = pickableBlueMushroomPrefab.GetComponent<Piece>() ?? pickableBlueMushroomPrefab.AddComponent<Piece>();

            piece.m_resources = new Piece.Requirement[] { new Piece.Requirement { m_resItem = blueMushroomItemDrop, m_amount = 2, m_recover = false } };

            piece.m_name = blueMushroomItemDrop.m_itemData.m_shared.m_name;
            piece.m_description = string.Empty;

            // m_blockingPieces is not used, because it only applies to the 'piece' mask, which we are not using
            //piece.m_blockingPieces = new List<Piece>() { piece };
            piece.m_blockingPieces = new List<Piece>();
            piece.m_blockRadius = 10f;

            piece.m_category = Piece.PieceCategory.Misc;

            piece.m_noInWater = true;

            piece.m_canBeRemoved = true;
            piece.m_primaryTarget = true;

            piece.m_allowedInDungeons = true;

            piece.m_onlyInBiome = Heightmap.Biome.Mountain;
            piece.m_icon = blueMushroomItemDrop.m_itemData.GetIcon();

            return piece;
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.UpdatePlacementGhost))]
    internal static class Player_UpdatePlacementGhost_Patch
    {
        private static void Postfix(Player __instance, GameObject ___m_placementGhost)
        {
            if (___m_placementGhost == null)
            {
                return;
            }

            if (___m_placementGhost.name == pickableBlueMushroomPrefabName)
            {
                // disable the building piece marker, that usually gets disabled by Piece.m_groundOnly or Piece.m_cultivatedGroundOnly
                __instance.m_placementMarkerInstance.SetActive(false);

                // we already have a reason to fail (like the biome), return
                if (__instance.m_placementStatus != 0)
                {
                    return;
                }

                // override successful placement status if we are not in a frost cave (biome is already checked)
                if (!__instance.InInterior() && !EnvMan.instance.CheckInteriorBuildingOverride())
                {
                    __instance.m_placementStatus = Player.PlacementStatus.WrongBiome;
                    __instance.SetPlacementGhostValid(false);
                    return;
                }

                Piece piece = __instance.m_placementGhost.GetComponent<Piece>();

                // check if there are already other blue mushrooms nearby (we can't use m_blockingPieces because that only checks the 'piece' layer, which the mushrooms are not on)
                if (__instance.PieceRayTest(out Vector3 vector, out _, out _, out _, out _, false))
                {
                    Collider[] array = Physics.OverlapSphere(vector, piece.m_blockRadius, PlantableBlueMushroom.pieceNonSolidLayerMask);

                    for (int i = 0; i < array.Length; i++)
                    {
                        Piece componentInParent = array[i].gameObject.GetComponentInParent<Piece>();

                        if (componentInParent == null || componentInParent == piece)
                        {
                            continue;
                        }

                        // is also blue mushroom
                        if (componentInParent.m_name == piece.m_name)
                        {
                            __instance.m_placementStatus = Player.PlacementStatus.MoreSpace;
                            __instance.SetPlacementGhostValid(false);
                            return;
                        }
                    }
                }
            }
        }
    }
}