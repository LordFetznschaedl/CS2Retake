using CounterStrikeSharp.API.Modules.Menu;
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
    public static class FullBuyMenu
    {
        private static FullBuyConfig _config { get; set; }

        static FullBuyMenu()
        {
            _config = AllocatorConfigManager.Load<FullBuyConfig>("CommandAllocator", "FullBuy");
        }

        public static void OpenPrimaryMenu(CsTeam team)
        {
            
        }

        public static void OpenSecondaryMenu(CsTeam team)
        {
            
        }
    }
}
