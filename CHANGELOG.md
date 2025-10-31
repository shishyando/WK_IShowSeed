# Version 1.8.0 Major bugfix update 01/nov/25

### Added:
* `General.EnableRandomSeedReplayability`
If true, random-seeded runs can be replayed in a seeded run by entering the seed you had. Default is `false`.

### Bugfixes:
* Clearing the seed window would set each run's seed to zero, forcing you to play the same run all the time (btw with kinda bad perks) and not updating the leaderboards.

# Version 1.7.0 Auto server selection update 31/oct/25

### Added:
* Automatic server selection (added a proxy in Russia). Invalid overrides will be reset to best working server. Valid overrides stay.

# Version 1.6.0 Main menu button update 31/oct/25

### Added:
* Main menu button to open seed window (also still available via `ctrl + shift + S`)
<div align="left">
<img src="https://raw.githubusercontent.com/shishyando/WK_IShowSeed/main/img/menu.jpg" style="width: 700px; object-fit: contain;">
</div>

# Version 1.5.0 Seed search update 30/oct/25

### Added:
* Seed search via config:
```
# Format: `{route name}: {unstable_perk1}, {perk2}, {perk3}`, available route names: `default`, `shortcut_sink`, `shortcut_burner`
DesiredRouteDescription = shortcut_burner: perk_u_t2_adoptionday, perk_metabolicstasis, perk_rabbitdna
```

### Changed:
* Plugin now produces less log spam

### Bugfixes:
* `General.PersistBetweenGameRestarts = false` could work improperly keeping your seed set, should be fixed


# Version 1.4.1 Leaderboards update 26/oct/25

### Added:
* Seeded leaderboards
<div align="left">
<img src="https://raw.githubusercontent.com/shishyando/WK_IShowSeed/main/img/stats.jpg" style="width: 400px; object-fit: contain;">
</div>

### Bugfixes:
* Sorting by time was just... sorting all runs by time. Now it shows only finished runs.

# Version 1.4.0 Leaderboards update 26/oct/25

### Added:
* Seeded leaderboards
<div align="left">
<img src="https://raw.githubusercontent.com/shishyando/WK_IShowSeed/main/img/stats.jpg" style="width: 400px; object-fit: contain;">
</div>

### Bugfixes:
* Apparently nothing worked since 1.2.0 sink update? Should be fixed now.

### TODO
* Better UI to show the predictions
* Step-by-step perk prediction? like you choose which route you want, which perk you want, etc.
* Seed searcher to find a seed with preset perks & artifacts

# Version 1.3.0 Predictions update (preview) 14/oct/25

### Added:
* Every time you set the seed, a preview of campaign perks for each route is dumped in BepInEx logs

### Things to notice
* Normally, perk machines check which perks you have unlocked and, most importantly, which perks you already have when using the machine
* In main menu you do not choose any perks yet, so your first choice can affect your last perk suggestions
* This only happens if machines generated the same perk more times than it can stack (and you choose it all the time)

### TODO
* Better UI to show the predictions
* Step-by-step perk prediction? like you choose which route you want, which perk you want, etc.
* Seed searcher to find a seed with preset perks & artifacts
* Seeded leaderboards

# Version 1.2.0 Sink update 14/oct/25

### Changed:
* Perks on prev version will differ from perks on this version on the same seed

### Bugfixes:
* Before this update refreshing one perk machine would change the other perk machine's refreshed perks
* Also before this update if you go through the Sink shortcut, it would matter in which order you access the perk machines (there is 1 unstable and 1 stable perk machine and accessing)
* Both of these issues are fixed now and you get consistent perks no matter in what order you access the machines

### TODO
* Preview perks & artifacts in main menu (maybe even levels?)
* Seed searcher to find a seed with preset perks & artifacts
* Seeded leaderboards

# Version 1.1.2 Gamemodes update 12/oct/25

### Added:
* Gamemode selection (config only)
* Option to clear seed on game launch (config only)

### Changed:
* Perks on prev version will differ from perks on this version on the same seed
* Setting a bit better seed for WorldDumper for Vending Machines and Perk Pages

### Bugfixes:
* fixed a really annoying bug which did not break anything but made me deadinside thinking that nothing works the way it should

### TODO
* Seed searcher to find a seed with preset perks & artifacts
* Seeded leaderboards
* Preview perks & artifacts in main menu (maybe even levels?)

# Version 1.0.1 Initial release 5/oct/25

It works?
