# IShowSeed

Seeded runs are now a thing! Set seed in any gamemode via UI and make your runs consistent.

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

#### `PresetSeed` (UI is simpler)
Preset seed which will be applied to all runs. `0` for random.

## Things to notice

- Some session events that do not affect gameplay are disabled (all per-second events like announcements that can occur any second)
- Levels do not flip
- Some things are spawned by a trigger (e.g. when a player steps into an invisible zone)
    1) They only spawn roaches and hoppers AFAIK, so `UT_TriggerSpawn`s are pretty much harmless
    2) The result relies on the order in which you enter the triggers: you run the same path - you get the same spawns, you go the other way - spawns change.
- The mod is quite complex, your FPS may be lower than usual. My lowest FPS with this mod and another heavy one was around 150, so it should be fine, but you are warned.
- Even though the mod was tested, you still may encounter a deadlock/game freeze. In this case please contact me via official White Knuckle discord server.

## Found a bug? Something is broken?

You can find me on the official White Knuckle Discord server and provide the info / request any features. You can also make PRs on GitHub. I recommend intense testing via [WorldDumper plugin](https://github.com/shishyando/WK_WorldDumper).
