using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators.Implementations.CommandAllocator.Configs;
using CSZoneNet.Plugin.CS2BaseAllocator.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Menus
{
    public class MidMenu
    {
        private static MidConfig _config { get; set; }

        static MidMenu()
        {
            _config = AllocatorConfigManager.Load<MidConfig>("CommandAllocator", "Mid");
        }

        public static void OpenPrimaryMenu(CsTeam team)
        {

        }

        public static void OpenSecondaryMenu(CsTeam team)
        {

        }
    }
}
