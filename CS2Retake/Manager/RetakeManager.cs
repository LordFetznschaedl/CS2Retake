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

namespace CS2Retake.Manager
{
    public class RetakeManager
    {
        private static RetakeManager _instance;
        public string ModuleName { get; set; }
        public bool BombHasBeenAssigned { get; set; } = false;

        public ulong SteamIdOfBombCarrier { get; set; } = ulong.MinValue;

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

        public void GiveBombToTerroristInBombZone(CCSPlayerController player)
        {
            if(player == null || !player.IsValid)
            {
                this.Log($"Player not valid");
                return;
            }
            
            if(player.SteamID != SteamIdOfBombCarrier)
            {
                return;
            }

            player.GiveNamedItem($"planted_c4");
        }

        public void PlantBomb()
        {
            
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

        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{this.ModuleName}:{this.GetType().Name}] {message}");
            Console.ResetColor();
        }

    }
}
