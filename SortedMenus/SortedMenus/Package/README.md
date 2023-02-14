This mod sorts the cooking, crafting and skills menu by configurable parameters.

Since this does not actually change the UI itself, only the list used to create it, this is compatible with almost all UI mods.


## 1 - Cooking Menu

This feature sorts all food recipes at the cauldron. By default, it sorts them by the combined health and stamina gain, and breaks ties using the health gain. This creates a list that actually reflects the progression through the game (it also showcases how inefficient mixed foods are).

To correctly sort items that still need to be processed afterwards, like bread or pie, the mod first checks if an item can be processed at the (iron) cooking station or oven, and then uses the food values of the finished product instead of the raw one for the sorting.


## 2 - Crafting Menus

Did you ever notice how random the recipe order in the workbench and forge menu are?

With this feature, they are sorted by name. Additionally, armor sets are sorted based on the name of the chest piece and then kept together (no more searching for the 'drake helmet') and ammo is always sorted to the top (metal arrows are sorted to the bottom, configurable).

The armor set feature works for all base game and most modded armors. I tested this with [Southsil Armor](https://valheim.thunderstore.io/package/southsil/SouthsilArmor/) and [Judes Equipment](https://valheim.thunderstore.io/package/GoldenJude/Judes_Equipment/), but it should work with most mods that use a sensible internal naming structure for their items. If you notice that a mod does not work well with this system, let me know and I will work on it, or you can always fix it yourself with the next feature.

![image](https://staticdelivery.nexusmods.com/mods/3667/images/2270/2270-1676385586-546054334.png)![image](https://staticdelivery.nexusmods.com/mods/3667/images/2270/2270-1676385596-1416742755.png)![image](https://staticdelivery.nexusmods.com/mods/3667/images/2270/2270-1676385603-1785674697.png)\


### 2.1 - Custom Name or Sort Overwrites

Even with the custom armor sorting rules, a few items, especially weapons, would be sorted into weird places. So this mod includes a sort overwrite system as well as a lightweight item rename system to customize your sorting experience using json files.

A few examples from the default english file:
- the 'Battleaxe' and 'Banded shield' are renamed to 'Iron battleaxe' and 'Iron banded shield'
- the silver tier weapons stay sorted together (without actual renaming)
- 'Porcupine' is sorted as a black metal mace and all instances of 'blackmetal' are replaced with 'black metal'
- the mistlands tier weapons stay sorted together (without actual renaming)

Of course not every language conveniently puts the material at the beginning of the name to make sorting easy (for example using 'Axe out of Iron' instead of 'Iron Axe'), so this system does not work for everyone. But feel free to send me overwrite files for your native language if it works for it.


## 3 - Skills Menu

This feature simply sorts your skills by the current level or by name.

Do be aware that the base game currently has a bug where the length of the level bar does not perfectly reflect your actual level (e.g. two skills at level 10 can have different lengths in the skills menu, independent of the actual level up progress), but the sorting is always correct.

![image](https://staticdelivery.nexusmods.com/mods/3667/images/2270/2270-1676335278-1719431038.png)


Source code available on github: https://github.com/Goldenrevolver/ValheimMiniMods