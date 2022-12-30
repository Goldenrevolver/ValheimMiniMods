using static ObtainableBlueMushrooms.ObtainableBlueMushroomsPlugin;
using static ObtainableBlueMushrooms.PatchObjectDB;

namespace ObtainableBlueMushrooms
{
    internal class FoodAndRecipeChanges
    {
        internal static void AddFoodAndRecipeChanges(ObjectDB __instance)
        {
            foreach (var recipe in __instance.m_recipes)
            {
                if (recipe?.m_item == null)
                {
                    continue;
                }

                var shared = recipe.m_item.m_itemData?.m_shared;

                if (shared == null)
                {
                    continue;
                }

                if (ReplaceMushroomsInWolfSkewer.Value && shared.m_name == "$item_wolf_skewer")
                {
                    foreach (var item in recipe.m_resources)
                    {
                        if (item.m_resItem.m_itemData.m_shared.m_name == "$item_mushroomcommon")
                        {
                            item.m_resItem = blueMushroomItemDrop;
                            item.m_amount = 1;
                        }
                    }
                }

                if (shared.m_name == "$item_onionsoup")
                {
                    if (MiniFoodRebalance.Value)
                    {
                        shared.m_foodStamina = 65;
                        shared.m_food = 21;
                        shared.m_foodBurnTime = 1500;
                        shared.m_foodRegen = 2;
                    }

                    if (AddMushroomToOnionSoup.Value)
                    {
                        var reqs = new Piece.Requirement[2];

                        reqs[0] = new Piece.Requirement() { m_resItem = blueMushroomItemDrop };
                        reqs[1] = new Piece.Requirement() { m_resItem = __instance.GetItemPrefab("Onion").GetComponent<ItemDrop>(), m_amount = 3 };

                        recipe.m_resources = reqs;
                    }
                }

                if (MiniFoodRebalance.Value && shared.m_name == "$item_eyescream")
                {
                    shared.m_foodStamina = 60;
                    shared.m_food = 20;
                    shared.m_foodBurnTime = 1500;
                    shared.m_foodRegen = 1;

                    //recipe.m_amount = 1;

                    foreach (var item in recipe.m_resources)
                    {
                        if (item.m_resItem.m_itemData.m_shared.m_name == "$item_greydwarfeye")
                        {
                            item.m_amount = 2;
                        }

                        //if (item.m_resItem.m_itemData.m_shared.m_name == "$item_freezegland")
                        //{
                        //    item.m_amount = 1;
                        //}
                    }
                }
            }
        }
    }
}