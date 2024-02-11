using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Menus
{
    public static class ChooserMenu
    {
        public static void OpenMenu(CCSPlayerController? player, IPlugin pluginInstance, bool enableRoundTypePistolMenu = true, bool enableRoundTypeMidMenu = true, bool enableRoundTypeFullBuyMenu = true)
        {
            var plugin = pluginInstance as BasePlugin;

            if(plugin == null)
            {
                return;
            }

            if(player == null || !player.IsValid || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid) 
            {
                return;
            }

            CenterHtmlMenu menu = new CenterHtmlMenu("Gun Menu");

            if(enableRoundTypePistolMenu) 
            {
                menu.AddMenuOption("T-Pistol", OnSelect);
                menu.AddMenuOption("CT-Pistol", OnSelect);
            }

            if(enableRoundTypeMidMenu) 
            {
                menu.AddMenuOption("T-Mid", OnSelect);
                menu.AddMenuOption("CT-Mid", OnSelect);
            }

            if(enableRoundTypeFullBuyMenu) 
            {
                menu.AddMenuOption("T-FullBuy", OnSelect);
                menu.AddMenuOption("CT-FullBuy", OnSelect);
            }

            MenuManager.OpenCenterHtmlMenu(plugin, player, menu);
        }

        private static void OnSelect(CCSPlayerController player, ChatMenuOption chatMenuOption)
        {
            var siteString = chatMenuOption?.Text?.Split('-')?.FirstOrDefault()?.ToUpper() ?? string.Empty;
            var roundTypeString = chatMenuOption?.Text?.Split('-')?.LastOrDefault()?.ToUpper() ?? string.Empty;

            CsTeam team = CsTeam.None;

            switch(siteString)
            {
                case "CT":
                    team = CsTeam.CounterTerrorist; 
                    break;
                case "T":
                    team = CsTeam.Terrorist;
                    break;
                default:
                    return;
            }

            if(team != CsTeam.Terrorist && team != CsTeam.CounterTerrorist)
            {
                return;
            }

            switch(roundTypeString)
            {
                case "PISTOL":
                    PistolMenu.OpenSecondaryMenu(team);
                    break;
                case "MID":
                    MidMenu.OpenPrimaryMenu(team);
                    break;
                case "FULLBUY":
                    FullBuyMenu.OpenPrimaryMenu(team);
                    break;
                default:
                    return;
            }


            Console.WriteLine($"ChatMenuOption selected: {chatMenuOption.Text}");
        }

    }
}
