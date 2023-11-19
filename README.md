# CS2Retake  NOT YET RELEASED - STILL WIP
  
WIP Implementation of a Retake plugin for CS2 using CounterStrikeSharp  
<https://docs.cssharp.dev/>  
  
---
# usage:  
| Command         | Parameter                                  | Description                                                           | Permissions      |
|-----------------|--------------------------------------------|-----------------------------------------------------------------------|------------------|
| !retakeninfo    |                                            | prints the plugin information                                         |                  |
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
# todo - release 1.0.0:  
- [x] creating, saving and reading spawns
- [x] player spawn in spawnpoints 
- [x] scramble teams
- [x] basic autoplant (fast plant, player needs to plant himself) 
- [x] assigning permissions for the commands
- [x] spawn loading system
- [x] weapon allocator system (kit based)
- [x] nade allocator system (kit based)
- [x] on ct win -> switch cts to t and the ts to ct

---
# todo - future releases:  
- [ ] auto assign teams -> deny choosing team -> switch team automatically
- [ ] config system
- [ ] editor system for spawns
- [ ] auto plant -> in a way that the bomb is defusable xD
- [ ] modular weapon allocator system
- [ ] change scramble command to do the scramble after round ends and before round starts



