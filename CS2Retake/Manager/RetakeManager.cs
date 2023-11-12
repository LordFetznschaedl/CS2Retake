using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Modules.Utils;
using System.Security.Cryptography;
using CS2Retake.Entity;
using CounterStrikeSharp.API.Modules.Entities;

namespace CS2Retake.Manager
{
    public class RetakeManager
    {
        private static RetakeManager _instance;
        public string ModuleName { get; set; }
        public bool BombHasBeenAssigned { get; set; } = false;


        public static RetakeManager GetInstance()
        {
            if(_instance == null)
            {
                _instance = new RetakeManager();
            }
            return _instance;
        }

        private RetakeManager() {}

        public void ScrambleTeams()
        {
            var nonSpectatingValidPlayers = this.GetPlayerControllers().Where(x => x.IsValid && (x.TeamNum == (int)CsTeam.Terrorist || x.TeamNum == (int)CsTeam.CounterTerrorist)).ToList();

            if(!nonSpectatingValidPlayers.Any()) 
            {
                this.Log($"No valid non spectating players have been found!");
                return;
            }

            var random = new Random();
            nonSpectatingValidPlayers = nonSpectatingValidPlayers.OrderBy(x => random.Next()).ToList();

            for(int i = 0; i < nonSpectatingValidPlayers.Count; i++) 
            {
                nonSpectatingValidPlayers[i].SwitchTeam(i % 2 == 0 ? CsTeam.CounterTerrorist : CsTeam.Terrorist);
            }
        }


        public void PlantBomb()
        {
            var random = new Random();
            var plantSpawn = MapManager.GetInstance().CurrentMap.SpawnPoints.Where(spawn => spawn.SpawnUsedBy != null && spawn.IsInBombZone).OrderBy(x => random.Next()).FirstOrDefault();

            if(plantSpawn == null)
            {
                this.Log($"No valid plant spawn found!");
                return;
            }

            var player = plantSpawn.SpawnUsedBy;

            if(player == null)
            {
                this.Log($"Player that uses the valid plant spawn is null");
                return;
            }

            player.GiveNamedItem("planted_c4");

            var plantedBomb = this.FindPlantedBomb();

            if(plantedBomb == null)
            {
                this.Log($"No planted bomb was found!");
                return;
            }

            var playerPawn = player.PlayerPawn.Value;

            if(playerPawn == null)
            {
                this.Log($"Player pawn is null");
                return;
            }
            if(playerPawn.AbsRotation == null)
            {
                this.Log($"Player pawn rotation is null");
                return;
            }
            if(playerPawn.AbsOrigin == null)
            {
                this.Log($"Player pawn position is null");
                return;
            }
            

            plantedBomb.Teleport(playerPawn.AbsOrigin, playerPawn.AbsRotation, new Vector(0f, 0f, 0f));

            plantedBomb.BombTicking = true;

            //var bombPlantedEventPtr = NativeAPI.CreateEvent("bomb_planted", false);
            //NativeAPI.SetEventPlayerController(bombPlantedEventPtr, "userid", player.Handle);
            //NativeAPI.SetEventInt(bombPlantedEventPtr, "site", plantedBomb.BombSite);
            //NativeAPI.SetEventEntity(bombPlantedEventPtr, "userid_pawn", player.PlayerPawn.Value.Handle);
            //NativeAPI.FireEvent(bombPlantedEventPtr, false);
           
        }

        public void ConfigureForRetake()
        {   
            Server.ExecuteCommand($"execifexists cs2retake/retake.cfg");
        }

        private List<CCSPlayerController> GetPlayerControllers() 
        {
            var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").ToList();

            if (!playerList.Any())
            {
                this.Log($"No Players have been found!");
            }

            return playerList;
        }

        private CPlantedC4? FindPlantedBomb()
        {
            var plantedBombList = Utilities.FindAllEntitiesByDesignerName<CPlantedC4>("planted_c4");

            if (!plantedBombList.Any())
            {
                this.Log("No planted bomb entities have been found!");
                return null;
            }

            return plantedBombList.FirstOrDefault();
        }

        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{this.ModuleName}:{this.GetType().Name}] {message}");
            Console.ResetColor();
        }

    }
}
