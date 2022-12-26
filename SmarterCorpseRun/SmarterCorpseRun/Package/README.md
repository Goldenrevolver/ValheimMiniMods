This mod removes some annoyances and inconsistencies from the run back after a death, without removing the penalty entirely.

Smarter Corpse Run buff:

The corpse run buff will now calculate how much maximum carrying capacity you could have if you equipped the optimal items out of your inventory:
- grants no carrying capacity when you don't have any item to increase it (like Megingjord), so you don't accidentally become overburdened
- dynamically updates the granted carrying capacity when you reequip your gear, so you don't accidentally become overburdened
- compatibility for items other than just Megingjord
- compatibility for non utility items, including custom moddded equip slots
- compatibility for custom modded item equip buffs and set bonuses of one item
- optional compatibility for equipping multiple utility items if you use a mod for that
- does not check for set bonuses of more than one item

There are actually surprisingly few modded items that increase maximum carrying capacity, so I only tested with a custom test item and with the small and big backpack from [Judes Equipment](https://valheim.thunderstore.io/package/GoldenJude/Judes_Equipment/). If you have both in your inventory, the mod properly detects that they both use the same modded slot.


Smarter Tombstone:

After checking if your to-be combined inventory has enough slots, the tombstone will now do the weight check using the same maximum potential carrying capacity calculation for the to-be combined inventory that the 'Smarter Corpse Run buff' feature uses. It will as usual allow you to auto pick up your items if both succeed.

The 'Take All' function (just for tombstones) now first puts any stackable items from your inventory from your naked run back into your tombstone first, so, if possible, all positions stay like they were when you died (like a mini quick stack), and then takes them all.


Configuration:

Be sure to change the config to suit your modded item setup. For example, if you have a mod to equip multiple utility items, enable that check for my mod. Be aware of the difference between of that and an item that uses a custom modded equip slot and enable checking for non utility slots if needed (I actually have it enabled by default now, but just know that this is important).


Source code available on github: https://github.com/Goldenrevolver/ValheimMiniMods