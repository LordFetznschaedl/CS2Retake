using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators.Implementations.CommandAllocator.Configs;
using CSZoneNet.Plugin.CS2BaseAllocator.Configs;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Menus
{
    public static class PistolMenu
    {
        private static PistolConfig _config { get; set; }

        static PistolMenu()
        {
            _config = AllocatorConfigManager.Load<PistolConfig>("CommandAllocator", "Pistol");
        }

        public static void OpenSecondaryMenu(CsTeam team)
        {

        }
    }
}
