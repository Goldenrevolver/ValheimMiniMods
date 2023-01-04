This mod adds water and cold debuff resistance and changes the bonuses of the capes and torches to use these new resistances. It can also change teleporting to instantly update the weather.

This is a companion mod to my other new mod [Simple New Set and Cape Bonuses](https://valheim.thunderstore.io/package/Goldenrevolver/Simple_New_Set_and_Cape_Bonuses/), and includes my other two new mods [Troll Armor Set Bonus Without Cape, Cape Adds Wet Resistance](https://valheim.thunderstore.io/package/Goldenrevolver/Troll_Armor_Set_Bonus_Without_Cape_But_Cape_Adds_Wet_Resistance/) and [Teleport Instantly Updates Weather And Removes Wet Debuff](https://valheim.thunderstore.io/package/Goldenrevolver/Teleport_Instantly_Updates_Weather_And_Removes_Wet_Debuff/).


## Water resistance:

- being resistant to water makes you dry off faster
- being very resistant means you can only get wet from swimming and not the rain
- immunity makes you immune to the wet debuff


## Cold resistance:

This splits off the cold and freezing debuff immunity from the frost damage resistance stat into a new stat. Having frost damage resistance still makes you immune to the debuffs though, so the bonuses from the frost resistance mead, the Wolf armor chest piece and Fenris armor chest piece are now more valuable while wearing only a partial cold resistance cape. With these changes, frost damage dealing enemies are now more dangerous if you are not prepared.

With the new cold resistance:
- being resistant to cold makes you immune to the cold debuff
- being very resistant also makes you immune to the freezing debuff while not wet
- immunity makes you always immune to the cold and freezing debuffs


## Cape changes:

Deer hide cape:
- 3% increased movement speed

Troll hide cape:
- 5% increased movement speed
- resistant to wet debuff
- additionally it's no longer a part of the troll armor set, making it a 3 piece set

Linen cape:
- 10% increased movement speed
- resistant to cold (immune to cold debuff, still vulnerable to freezing debuff)

Wolf fur cape:
- very resistant to cold (immune to cold, immune to freezing while not wet)
- no longer grants frost resistance

Lox cape:
- immunity to cold (always immune to cold and freezing)
- no longer grants frost resistance
- this makes it an upgrade to the wolf fur cape (configurable)

Feather cape
- very resistant to cold (immune to cold, immune to freezing while not wet)
- no longer grants frost resistance


## Torch bonuses:

The torch now grants resistance to cold (immune to cold debuff, still vulnerable to freezing debuff), and resistance to water (you dry off faster).
It optionally now also loses more durability while outdoors in a cold, freezing or wet biome (but less than in [Fire Is Hot](https://www.nexusmods.com/valheim/mods/155)).


## Teleporting changes:

Now with the wolf fur cape only granting very resistant to frost, the wet debuff is an actual consideration again, and with it comes the base game issue of weather taking a few seconds to update after teleporting, because the transition period used for normal biome transition still gets applied.

So this also includes the changes from my other new mod [Teleport Instantly Updates Weather And Removes Wet Debuff](https://valheim.thunderstore.io/package/Goldenrevolver/Teleport_Instantly_Updates_Weather_And_Removes_Wet_Debuff/):
- Teleporting with a portal now immediately updates the weather at the new location, so you don't get wet from rain that doesn't exist
- Optionally removes wet, cold and freezing debuffs at the end of the teleport
- Optionally grants wet, cold and freezing debuff immunity for 5 seconds at the end of a teleport, if you want to disable the instant teleporting weather update for compatibility reasons


## Compatibility:

This mod is compatible with [Raven Cape](https://valheim.thunderstore.io/package/Tequila/Raven_Cape/). If you have both installed, then you can disable the frost resistance and enable cold resistance in their configuration file/ menu.

This mod includes a custom version of [Custom Armor Stats](https://www.nexusmods.com/valheim/mods/1162) and is therefore incompatible with it.
It is also incompatible with [Fire Is Hot](https://www.nexusmods.com/valheim/mods/155) and [Toasty Torches](https://www.nexusmods.com/valheim/mods/971/) by design.


Source code available on github: https://github.com/Goldenrevolver/ValheimMiniMods