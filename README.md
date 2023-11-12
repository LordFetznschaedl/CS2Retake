# CS2Retake  NOT YET RELEASED - STILL WIP
  
WIP Implementation of a Retake plugin for CS2 using CounterStrikeSharp  
<https://docs.cssharp.dev/>  
  
---
# usage:  
| Command         | Parameter                                  | Description                                                           | Permissions                         |
|-----------------|--------------------------------------------|-----------------------------------------------------------------------|-------------------------------------|
| !retakeninfo    |                                            | prints the plugin information                                         |                                     |
| !retakespawn    | <index (integer)>                          | teleports the player to the given spawn index                         | @cs2retake/admin, @cs2retake/editor |
| !retakewrite    |                                            | writes the spawns of the current map to the corresponding map config  | @cs2retake/admin, @cs2retake/editor |
| !retakeread     |                                            | reads the spawns of the current map from the corresponding map config | @cs2retake/admin, @cs2retake/editor |
| !retakescramble |                                            | scrambles the teams instantly                                         | @css/generic, @cs2retake/admin      |
| !retaketeleport | <X (float)> <Y (float)> <Z (float)>        | teleports player to the given coordinates                             | @cs2retake/admin, @cs2retake/editor |
| !retakeaddspawn | <2/3 - 2 = T; 3 = CT> <0/1 - 0 = A; 1 = B> | creates a new spawn                                                   | @cs2retake/admin, @cs2retake/editor |
  
---
# installation:  
Extract the `addons` folder to the `/csgo/` directory of the dedicated server.  

---
# todo - release 1.0.0:  
- [x] creating, saving and reading spawns
- [x] player spawn in spawnpoints 
- [x] scramble teams
- [x] basic autoplant 
- [ ] assigning permissions for the commands
- [ ] weapon assign system
- [ ] auto assign teams
- [ ] spawn loading system -> verify loading

---
# todo - future releases:  
- [ ] config system
- [ ] editor system for spawns
- [ ] auto plant -> fire bomb_planted event



