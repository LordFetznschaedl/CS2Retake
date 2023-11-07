using CS2Retake.Entity;
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

        public static MapLogic GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MapLogic();
            }
            return _instance;
        }

        private MapLogic() { }



        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{this.ModuleName}:{this.GetType().Name}] {message}");
            Console.ResetColor();
        }
    }
}
