# CS2Retake  NOT YET RELEASED - STILL WIP
  
WIP Implementation of a Retake plugin for CS2 using CounterStrikeSharp  
<https://docs.cssharp.dev/>  
  
---
# usage:  
!retakeninfo - to print the plugin information  
!retakespawn <index> - teleports you to the spawn index given as an argument  
!retakewrite - writes the spawns of the current map to the corresponding map config  
!retakeread - reads the spawns of the current map from the corresponding map config  
!retakescramble - scrambles the teams instantly  
!retaketeleport <position X float> <position Y float> <position Z float> - teleports player to the given coordinates  
!retakeaddspawn <2/3 - 2 = T; 3 = CT> <0/1 - 0 = A; 1 = B> - creates a new spawn
  
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



