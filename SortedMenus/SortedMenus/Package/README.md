This mod sorts the cooking, crafting and skills menu by configurable parameters.

Since this does not actually change the UI itself, only the list used to create it, this is compatible with almost all UI mods.


## Cooking Menu

This feature sorts all food recipes at the cauldron. By default, it sorts them by the combined health and stamina gain, and breaks ties using the health gain. This creates a list that actually reflects the progression through the game. It also showcases how weak mixed foods are.

To correctly sort items that still need to be processed afterwards, like bread or cake, the mod first checks if an item can be processed at the (iron) cooking station or oven, and then uses the food values of the finished product instead of the raw one for the sorting.


## Crafting Menus

Did you ever notice how random the recipe order in the workbench and forge menu are? With this feature, they are sorted by name, but also
- armor sets are sorted based on the name of the chest piece and then placed together (no more searching for the 'drake helmet')
- ammo is always sorted to the top (metal arrows are sorted to the bottom, configurable)
- a few items, especially weapons, are sorted using custom overrides: for example the 'battleaxe' is sorted as 'iron battleaxe' (currently only available for English)

The armor set feature works for all base game and most modded armors. I tested this with [Southsil Armor](https://valheim.thunderstore.io/package/southsil/SouthsilArmor/) and [Judes Equipment](https://valheim.thunderstore.io/package/GoldenJude/Judes_Equipment/), but it should work with most mods that use a sensible internal naming structure for their items. If you notice that a mod does not work well with this system, let me know and I will work on it.

![image](https://staticdelivery.nexusmods.com/mods/3667/images/2270/2270-1676335287-506780628.png)![image](https://staticdelivery.nexusmods.com/mods/3667/images/2270/2270-1676335297-1086767525.png)![image](https://staticdelivery.nexusmods.com/mods/3667/images/2270/2270-1676335305-821085753.png)


## Skills Menu

This feature simply sorts your skills by the current level or by name.

Do be aware that the base game currently has a bug where the length of the level bar does not perfectly reflect your actual level (two skills at level 10 can have different lengths in the skills menu, independent of the actual level up progress), but the sorting is always correct.

![image](https://staticdelivery.nexusmods.com/mods/3667/images/2270/2270-1676335278-1719431038.png)


Source code available on github: https://github.com/Goldenrevolver/ValheimMiniMods