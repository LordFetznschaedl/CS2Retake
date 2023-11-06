using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Modules.Utils;

namespace CS2Retake.Logic
{
    public class RetakeLogic
    {
        private static RetakeLogic _instance;
        public string ModuleName { get; set; }

        public static RetakeLogic GetInstance()
        {
            if(_instance == null)
            {
                _instance = new RetakeLogic();
            }
            return _instance;
        }

        private RetakeLogic() {}

        public void ScrambleTeams()
        {
            var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller");

            if(!playerList.Any())
            {
                this.Log($"No Players have been found!");
                return;
            }

            var nonSpectatingValidPlayers = playerList.Where(x => x.IsValid && (x.TeamNum == (int)CsTeam.Terrorist || x.TeamNum == (int)CsTeam.CounterTerrorist)).ToList();

            if(!nonSpectatingValidPlayers.Any()) 
            {
                this.Log($"No valid non spectating players have been found!");
                return;
            }


        }

        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{this.ModuleName}:{this.GetType().Name}] {message}");
            Console.ResetColor();
        }

    }
}
