using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

            ChatMenu menu = new ChatMenu("Gun Menu");

            if(enableRoundTypePistolMenu) 
            {
                menu.AddMenuOption("T_Pistol", OnSelect);
                menu.AddMenuOption("CT_Pistol", OnSelect);
            }

            if(enableRoundTypeMidMenu) 
            {
                menu.AddMenuOption("T_Mid", OnSelect);
                menu.AddMenuOption("CT_Mid", OnSelect);
            }

            if(enableRoundTypeFullBuyMenu) 
            {
                menu.AddMenuOption("T_FullBuy", OnSelect);
                menu.AddMenuOption("CT_FullBuy", OnSelect);
            }

            MenuManager.OpenChatMenu(player, menu);
        }

        private static void OnSelect(CCSPlayerController player, ChatMenuOption chatMenuOption)
        {
            var siteString = chatMenuOption?.Text?.Split('_')?.FirstOrDefault()?.ToUpper() ?? string.Empty;
            var roundTypeString = chatMenuOption?.Text?.Split('_')?.LastOrDefault()?.ToUpper() ?? string.Empty;

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


            MenuManager.CloseActiveMenu(player);
            switch (roundTypeString)
            {
                case "PISTOL":
                    PistolMenu.Instance.OpenSecondaryMenu(player, team);
                    break;
                case "MID":
                    MidMenu.Instance.OpenPrimaryMenu(player, team);
                    break;
                case "FULLBUY":
                    FullBuyMenu.Instance.OpenPrimaryMenu(player, team);
                    break;
                default:
                    return;
            }
        }

    }
}
