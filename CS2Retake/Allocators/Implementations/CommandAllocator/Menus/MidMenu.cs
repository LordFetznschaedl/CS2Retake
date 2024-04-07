using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators.Implementations.CommandAllocator.Configs;
using CS2Retake.Allocators.Implementations.CommandAllocator.Manager;
using CS2Retake.Utils;
using CSZoneNet.Plugin.CS2BaseAllocator.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Menus
{
    public class MidMenu
    {
        private static MidConfig _config { get; set; } = new MidConfig();

        private static MidMenu? _instance = null;
        public static MidMenu Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MidMenu();
                }
                return _instance;
            }
        }

        private MidMenu()
        {
            _config = AllocatorConfigManager.Load<MidConfig>("CommandAllocator", "Mid");
        }

        public void OpenPrimaryMenu(CCSPlayerController player, CsTeam team)
        {
            ChatMenu menu = new ChatMenu("Mid Primary Menu");

            var teamString = team == CsTeam.CounterTerrorist ? "CT" : "T";

            foreach (var primary in _config.AvailablePrimaries.FindAll(x => x.Team == team || x.Team == CsTeam.None))
            {
                menu.AddMenuOption($"{teamString}_{primary.WeaponName}", OnSelectPrimary);
            }

            MenuManager.CloseActiveMenu(player);
            MenuManager.OpenChatMenu(player, menu);
        }

        public void OpenSecondaryMenu(CCSPlayerController player, CsTeam team)
        {
            ChatMenu menu = new ChatMenu("Mid Secondary Menu");

            var teamString = team == CsTeam.CounterTerrorist ? "CT" : "T";

            foreach (var secondary in _config.AvailableSecondaries.FindAll(x => x.Team == team || x.Team == CsTeam.None))
            {
                menu.AddMenuOption($"{teamString}_{secondary.WeaponName}", OnSelectSecondary);
            }

            MenuManager.CloseActiveMenu(player);
            MenuManager.OpenChatMenu(player, menu);
        }

        private static void OnSelectPrimary(CCSPlayerController player, ChatMenuOption chatMenuOption)
        {
            var menuText = chatMenuOption.Text?.Split('_')?.LastOrDefault() ?? string.Empty;
            var weaponString = _config.AvailablePrimaries.FirstOrDefault(x => x.WeaponName.Equals(menuText))?.WeaponString;
            
            if (string.IsNullOrWhiteSpace(weaponString))
            {
                return;
            }

            var team = GetTeam(chatMenuOption.Text ?? string.Empty);

            MessageUtils.PrintToPlayerOrServer($"You have now selected {ChatColors.Green}{menuText}{ChatColors.White} as your primary for {ChatColors.Green}Mid{ChatColors.White} rounds!", player);

            CacheManager.Instance.AddOrUpdateMidPrimaryCache(player, weaponString, team);
            DBManager.Instance.InsertOrUpdateMidPrimaryWeaponString(player.SteamID, weaponString, (int)team);

            MidMenu.Instance.OpenSecondaryMenu(player, team);
        }

        private static void OnSelectSecondary(CCSPlayerController player, ChatMenuOption chatMenuOption)
        {
            var menuText = chatMenuOption.Text?.Split('_')?.LastOrDefault() ?? string.Empty;
            var weaponString = _config.AvailableSecondaries.FirstOrDefault(x => x.WeaponName.Equals(menuText))?.WeaponString;
            
            if (string.IsNullOrWhiteSpace(weaponString))
            {
                return;
            }

            var team = GetTeam(chatMenuOption.Text ?? string.Empty);

            MessageUtils.PrintToPlayerOrServer($"You have now selected {ChatColors.Green}{menuText}{ChatColors.White} as your secondary for {ChatColors.Green}Mid{ChatColors.White} rounds!", player);

            CacheManager.Instance.AddOrUpdateMidSecondaryCache(player, weaponString, team);
            DBManager.Instance.InsertOrUpdateMidSecondaryWeaponString(player.SteamID, weaponString, (int)team);
        }

        private static CsTeam GetTeam(string weaponString)
        {
            var teamString = weaponString?.Split('_')?.FirstOrDefault()?.ToUpper() ?? string.Empty;

            CsTeam team = CsTeam.None;

            switch (teamString)
            {
                case "CT":
                    return CsTeam.CounterTerrorist;
                case "T":
                    return CsTeam.Terrorist;
                default:
                    return CsTeam.None;
            }

        }
    }
}
