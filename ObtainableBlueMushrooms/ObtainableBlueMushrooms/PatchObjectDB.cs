using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ObtainableBlueMushrooms.ObtainableBlueMushroomsPlugin;

namespace ObtainableBlueMushrooms
{
    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
    internal static class PatchObjectDB
    {
        internal const string pickableBlueMushroomPrefabName = "Pickable_Mushroom_blue";
        internal static GameObject pickableBlueMushroomPrefab;

        private const string cultivatorPrefabName = "Cultivator";
        internal static ItemDrop cultivatorPrefab;

        private const string hammerPrefabName = "Hammer";
        internal static ItemDrop hammerPrefab;

        private const string blueMushroomItemDropName = "MushroomBlue";
        internal static ItemDrop blueMushroomItemDrop;

        public static void Postfix(ObjectDB __instance)
        {
            if (SceneManager.GetActiveScene().name != "main")
            {
                return;
            }

            foreach (var gameObject in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
            {
                if (gameObject.name == pickableBlueMushroomPrefabName)
                {
                    pickableBlueMushroomPrefab = (GameObject)gameObject;
                }

                if (gameObject.name == cultivatorPrefabName && ((GameObject)gameObject).TryGetComponent(out ItemDrop cultivator))
                {
                    cultivatorPrefab = cultivator;
                }

                if (gameObject.name == hammerPrefabName && ((GameObject)gameObject).TryGetComponent(out ItemDrop hammer))
                {
                    hammerPrefab = hammer;
                }
            }

            blueMushroomItemDrop = __instance.GetItemPrefab(blueMushroomItemDropName).GetComponent<ItemDrop>();

            if (AllowCavePlantingChanges())
            {
                PlantableBlueMushroom.InitPieceAndAddToBuildPieces();
            }

            FoodAndRecipeChanges.AddFoodAndRecipeChanges(__instance);
        }
    }
}