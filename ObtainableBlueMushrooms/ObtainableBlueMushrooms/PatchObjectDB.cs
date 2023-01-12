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
        internal static GameObject cultivatorPrefab;

        private const string hammerPrefabName = "Hammer";
        internal static GameObject hammerPrefab;

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

                if (gameObject.name == cultivatorPrefabName)
                {
                    cultivatorPrefab = (GameObject)gameObject;
                }

                if (gameObject.name == hammerPrefabName)
                {
                    hammerPrefab = (GameObject)gameObject;
                }
            }

            blueMushroomItemDrop = __instance.GetItemPrefab(blueMushroomItemDropName).GetComponent<ItemDrop>();

            if (EnablePlantingInCaves.Value && !HasPlugin("advize.PlantEverything"))
            {
                PlantableBlueMushroom.InitPieceAndAddToBuildPieces();
            }

            FoodAndRecipeChanges.AddFoodAndRecipeChanges(__instance);
        }
    }
}