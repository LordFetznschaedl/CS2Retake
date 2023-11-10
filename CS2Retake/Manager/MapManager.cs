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

namespace CS2Retake.Manager
{
    public class MapManager
    {
        private static MapManager _instance;
        public string ModuleName { get; set; }
        public MapEntity CurrentMap { get; set; }

        public BombSiteEnum BombSite { get; set; } = BombSiteEnum.Undefined;

        public int TerroristRoundWinStreak { get; set; } = 0;

        public static MapManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MapManager();
            }
            return _instance;
        }

        private MapManager() { }


        public void RandomBombSite()
        {
            this.BombSite = (BombSiteEnum)new Random().NextInt64(0, 1);
        }
      

        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{this.ModuleName}:{this.GetType().Name}] {message}");
            Console.ResetColor();
        }
    }
}
