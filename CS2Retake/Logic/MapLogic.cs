using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Entity;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CS2Retake.Logic
{
    public class MapLogic
    {
        private static MapLogic _instance;
        public string ModuleName { get; set; }
        public MapEntity CurrentMap { get; set; }

        public BombSiteEnum BombSite { get; set; } = BombSiteEnum.Undefined;

        public int TerroristRoundWinStreak { get; set; } = 0;

        public static MapLogic GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MapLogic();
            }
            return _instance;
        }

        private MapLogic() { }


        public void RandomBombSite()
        {
            this.BombSite = (BombSiteEnum)new Random().NextInt64(0, 1);
        }
       
        public void DetectSpawnsInBombZone(CCSPlayerController player)
        {
            

            foreach(var spawnPoint in this.CurrentMap.SpawnPoints.Select((value, index) => new { index, value })) 
            {
                
                
                this.CurrentMap.TeleportPlayerToSpawn(player, BombSiteEnum.Undefined, spawnPoint.index);

                

                if (player.PlayerPawn.Value.InBombZone)
                {
                    spawnPoint.value.IsInBombSite = true;
                }

                var message = $"Detecting if spawnpoint is in BombSite: Spawn {spawnPoint.index + 1} of {this.CurrentMap.SpawnPoints.Count} - IsInBombSite: {spawnPoint.value.IsInBombSite} - InBombZone: {player.PlayerPawn.Value.InBombZone}";
                this.Log(message);
                player.PrintToChat(message);
            }

            this.CurrentMap.WriteSpawns();
        }

        

        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{this.ModuleName}:{this.GetType().Name}] {message}");
            Console.ResetColor();
        }
    }
}
