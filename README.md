# IShowSeed

Seeded runs are now a thing! Set seed in any gamemode via UI and make your runs consistent. Preview perks for each route via BepInEx logs on seed save. Check out the changelogs for more details.

## TODO
* Better UI to show the predictions
* Step-by-step perk prediction? like you choose which route you want, which perk you want, etc.
* Seed searcher to find a seed with preset perks & artifacts
* Seeded leaderboards


## UI

Press `ctrl + shift + S` at the main menu to set or clear the seed
<div align="left">
<img src="https://raw.githubusercontent.com/shishyando/WK_IShowSeed/main/img/menu.jpg" style="width: 538px; height: 410px; object-fit: contain;">
</div>


View seed ingame via Debug Menu (`F5`)
<div align="left">
<img src="https://raw.githubusercontent.com/shishyando/WK_IShowSeed/main/img/debug.jpg" style="width: 538px; height: 410px; object-fit: contain;">
</div>


Seed shown at the end of the run
<div align="left">
<img src="https://raw.githubusercontent.com/shishyando/WK_IShowSeed/main/img/stats.jpg" style="width: 538px; height: 410px; object-fit: contain;">
</div>

## Config

Configs are located in `WhiteKnuckleFolder/BepInEx/Config/shishyando.WK.IShowSeed.cfg`.

#### `PresetSeed` (UI is simpler)
Preset seed which will be applied to all runs. `0` for random.

#### `Gamemodes`
You can select the gamemodes which will be affected by the seed. Default is campaign + all endless (including, iron/hard/both)

#### `PersistBetweenGameRestarts`
If enabled, the seed will be remembered across game restarts. If not, preset seed will be cleared on game restart. Default is `true`.


## Found a bug? Something is broken?

You can find me on the [official White Knuckle Discord server](https://discord.com/channels/1278757582038630410/1411846375108513924) and provide the info / request any features. You can also make PRs on GitHub. I recommend intense testing via [WorldDumper plugin](https://github.com/shishyando/WK_WorldDumper).

## Technical Details

- Some session events that do not affect gameplay are disabled (all per-second events like announcements that can occur any second)
- Levels do not flip
- Some things are spawned by a trigger (e.g. when a player steps into an invisible zone)
    1) They only spawn roaches and hoppers AFAIK, so `UT_TriggerSpawn`s are pretty much harmless
    2) The result relies on the order in which you enter the triggers: you run the same path - you get the same spawns, you go the other way - spawns change.
