using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Entity;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
       


        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{this.ModuleName}:{this.GetType().Name}] {message}");
            Console.ResetColor();
        }
    }
}
