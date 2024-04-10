# CS2Retake 2.1.0
  
Implementation of a Retake plugin for CS2 using CounterStrikeSharp  
<https://docs.cssharp.dev/>  
  
---
# reuirements:  
- min. CounterStrikeSharp API Version: 205  
- GameType: Casual or Competitive
  
---
# usage:  
| Command         | Parameter                                  | Description                                                           | Permissions      |
|-----------------|--------------------------------------------|-----------------------------------------------------------------------|------------------|
| !guns           |                                            | opens up the gun menu for the allocator system                        |                  |
| !retakeinfo     |                                            | prints the plugin information                                         |                  |
| !retakespawn    | <index (integer)>                          | teleports the player to the given spawn index                         | @cs2retake/admin |
| !retakewrite    |                                            | writes the spawns of the current map to the corresponding map config  | @cs2retake/admin |
| !retakeread     |                                            | reads the spawns of the current map from the corresponding map config | @cs2retake/admin |
| !retakescramble |                                            | scrambles the teams instantly                                         | @cs2retake/admin |
| !retaketeleport | <X (float)> <Y (float)> <Z (float)>        | teleports player to the given coordinates                             | @cs2retake/admin |
| !retakeaddspawn | <2/3 - 2 = T; 3 = CT> <0/1 - 0 = A; 1 = B> | creates a new spawn                                                   | @cs2retake/admin |
  
---
# installation:  
Extract the `addons` folder to the `/csgo/` directory of the dedicated server.  

---
# release 2.1.0:  
- [x] modular weapon allocator system
- [x] creating, saving and reading spawns
- [x] player spawn in spawnpoints 
- [x] scramble teams
- [x] basic autoplant (fast plant, player needs to plant himself) 
- [x] assigning permissions for the commands
- [x] spawn loading system
- [x] weapon allocator system (kit based)
- [x] nade allocator system (kit based)
- [x] on ct win -> switch cts to t and the ts to ct
- [x] auto assign teams -> deny choosing team -> switch team automatically
- [x] KevlarHelmet being only given as Kevlar without Helmet
- [x] config system
- [x] auto plant -> changable to fast plant if prefered in plugin base config

---
# future releases:  
- [ ] editor system for spawns
- [ ] change scramble command to do the scramble after round ends and before round starts
- [ ] multi language support

---
# plugin base config 
location: addons\counterstrikesharp\configs\plugins\CS2Retake.json
```
{
  //PlantType Options: AutoPlant, FastPlant
  "PlantType": "AutoPlant",

  //RoundTypeMode Options: Sequence, Specific, Random
  "RoundTypeMode": "Sequence",

  //Configuration for RoundTypeMode Sequence
  //Played from top to bottom
  //AmountOfRounds -1 is for all remaining rounds in the map
  //Available RoundType Options: FullBuy, Pistol, Mid, Undefined
  "RoundTypeSequence": [
    {
      "RoundType": "Pistol",
      "AmountOfRounds": 5
    },
    {
      "RoundType": "Mid",
      "AmountOfRounds": 3
    },
    {
      "RoundType": "FullBuy",
      "AmountOfRounds": -1
    }
  ],
  
  //RoundTypeSpecific is for a non changing roundtype
  //This will only work if RoundTypeMode is Specific
  //RoundTypeSpecific Options: FullBuy, Pistol, Mid, Undefined
  "RoundTypeSpecific": "FullBuy",

  //Change this to change the way how the allocator system works
  //More allocators coming in the future
  //Allocator Options: Command
  "Allocator": "Command",

  "SecondsUntilBombPlantedCheck": 5,
  "SpotAnnouncerEnabled": true,
  "EnableQueue": true,
  "EnableScramble": true,
  "EnableSwitchOnRoundWin": true,
  "ScrambleAfterSubsequentTerroristRoundWins": 5,
  "MaxPlayers": 10,
  "TeamBalanceRatio": 0.499,
  "EnableThankYouMessage": false,
  "EnableDebug": false,
  "ConfigVersion": 5
}
```

---
# special thanks:  
[splewis](https://github.com/splewis): This plugin is inspired by [his retake plugin for CSGO](https://github.com/splewis/csgo-retakes)

Discord:
- @gorwok for creating spawns on de_mirage
- @cowhitface for creating spawns on de_overpass, de_inferno, de_vertigo, de_nuke, de_anubis & de_ancient

Twitter:
- @t1ckrate for helping moderate the issues on this repo 


