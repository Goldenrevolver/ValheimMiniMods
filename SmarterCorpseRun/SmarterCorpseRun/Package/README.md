This mod removes some annoyances and inconsistencies from the run back after a death, without removing the penalty entirely.

## Smarter Corpse Run buff:

The corpse run buff will now calculate how much maximum carrying capacity you could have if you equipped the optimal items out of your inventory:
- grants no carrying capacity when you don't have any item to increase it (like Megingjord), so you don't accidentally become overburdened
- dynamically updates the granted carrying capacity when you reequip your gear (aka the buff gives less when your gear is reequipped), so you don't accidentally become overburdened
- compatibility for items other than just Megingjord
- compatibility for non utility items, including custom moddded equip slots
- compatibility for custom modded item equip buffs and set bonuses of one item
- optional compatibility for equipping multiple utility items if you use a mod for that
- works with modded "set" bonuses that require only one item, but not ones that require more than one

There are actually surprisingly few modded items that increase maximum carrying capacity, so I only tested with a custom test item, and with the small and big backpack from [Judes Equipment](https://valheim.thunderstore.io/package/GoldenJude/Judes_Equipment/). As an example, when I had both in my inventory, the mod properly detected that they both use the same modded slot, so 'Corpse Run' gave me 200 carrying capacity for the big backpack and 150 for megingjord. When I reequipped megingjord it now gave 150 less, and when I then equipped the small backpack it gave me 100 less, because the optimal item for that slot was still the big backpack. In short, my maximum carrying capacity stayed at 300+200+150=650 the entire time, as intended.


## Smarter Tombstone:

After checking if your to-be combined inventory has enough slots, the tombstone will now do the weight check using the same maximum potential carrying capacity calculation for the to-be combined inventory that the 'Smarter Corpse Run buff' feature uses. It will as usual allow you to auto pick up your items if both succeed.

The 'Take All' function (just for tombstones) now first puts any stackable items from your inventory from your naked run back into your tombstone, so, if possible, all positions stay like they were when you died (like a mini quick stack, excluding custom data items for compatibility), and then takes them all.


## Configuration:

Be sure to change the config to suit your modded item setup. For example, if you have a mod to equip multiple utility items, enable that check for my mod. Be aware of the difference between of that and an item that uses a custom modded equip slot and enable checking for non utility slots if needed (I actually have it enabled by default now, but just know that this is important).


Source code available on github: https://github.com/Goldenrevolver/ValheimMiniMods