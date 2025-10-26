# IShowSeed

Seeded runs, now with leaderboards! Set seed in any gamemode via UI and make your runs consistent. Preview perks for each route via BepInEx logs on seed save. Check out the changelog for more details.

## TODO
* Better UI to show the predictions
* Step-by-step perk prediction? like you choose which route you want, which perk you want, etc.
* Seed searcher to find a seed with preset perks & artifacts

## UI

Press `ctrl + shift + S` at the main menu to set or clear the seed
<div align="left">
<img src="https://raw.githubusercontent.com/shishyando/WK_IShowSeed/main/img/menu.jpg" style="width: 800px; object-fit: contain;">
</div>

Seed is shown at the end of the run and leaderboards are updated
<div align="left">
<img src="https://raw.githubusercontent.com/shishyando/WK_IShowSeed/main/img/stats.jpg" style="width: 800px; object-fit: contain;">
</div>

Leaderboards are also updated before the run
<div align="left">
<img src="https://raw.githubusercontent.com/shishyando/WK_IShowSeed/main/img/leaderboards.jpg" style="width: 800px; object-fit: contain;">
</div>

View seed ingame via Debug Menu (`F5`)
<div align="left">
<img src="https://raw.githubusercontent.com/shishyando/WK_IShowSeed/main/img/debug.jpg" style="width: 800px; object-fit: contain;">
</div>

## Config

Configs are located in `WhiteKnuckleFolder/BepInEx/Config/shishyando.WK.IShowSeed.cfg`.

#### `PresetSeed` (UI is simpler)
Preset seed which will be applied to all runs. `0` for random.

#### `Gamemodes`
You can select the gamemodes which will be affected by the seed. Default is campaign + all endless (including, iron/hard/both)

#### `PersistBetweenGameRestarts`
If enabled, the seed will be remembered across game restarts. If not, preset seed will be cleared on game restart. Default is `true`.

### `Uri`
Uri for leaderboards sync. In case of any leaderboard issues, try downloading the latest version of the mod and restting this setting to default (done via editing the config).

### `TimeoutSeconds`
Timeout for all leaderboards requests. The server should respond fast enough, so in case of any issues check [Uri description](#uri)

## Found a bug? Something is broken?

You can find me on the [official White Knuckle Discord server](https://discord.com/channels/1278757582038630410/1411846375108513924) and provide the info / request any features. You can also make PRs on GitHub. I recommend intense testing via [WorldDumper plugin](https://github.com/shishyando/WK_WorldDumper).

## Technical Details

- Some session events that do not affect gameplay are disabled (all per-second events like announcements that can occur any second)
- Levels do not flip
- Some things are spawned by a trigger (e.g. when a player steps into an invisible zone)
    1) They only spawn roaches and hoppers AFAIK, so `UT_TriggerSpawn`s are pretty much harmless
    2) The result relies on the order in which you enter the triggers: you run the same path - you get the same spawns, you go the other way - spawns change.
