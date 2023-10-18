# Changelog

## 1.3.3
- Added config option to also sort the hand crafting menu in your inventory. Disabled by default
## 1.3.2
- Hotfix: Fixed custom crafting station support causing the cauldron to not get treated as a cooking station
## 1.3.1
- Hotfix: Fixed hand crafting not working due to missing null check
## 1.3.0
- Fixed compatibility with the pagination feature of AAA Crafting
- Added config file to set whether a custom crafting station should be treated as a cooking or crafting station, or not get sorted at all
## 1.2.2
- Custom cooking stations from 'BoneAppetit' are now sorted as cooking menus rather than crafting menus
## 1.2.1
- Hotfix: Fixed caching breaking some crafting stations due to missing null check
## 1.2.0
- Performance: Sort order is now saved (cached) until you learn a new recipe or change config settings
- Improved sorting order for items that grant eitr and added config settings for them
## 1.1.2
- Fixed skills menu update breaking when using Project Auga (because it deletes it)
## 1.1.1
- Performance: Modded armor sort overwrite checks are skipped if there is already a custom overwrite for it
## 1.1.0
- Added file based system for adding item sort overwrites or changing item names entirely
## 1.0.0
- Initial release