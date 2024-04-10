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
using System.Configuration;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Menus
{
    public class FullBuyMenu
    {
        private static FullBuyConfig _config { get; set; } = new FullBuyConfig();

        private static FullBuyMenu? _instance = null;

        public static FullBuyMenu Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FullBuyMenu();
                }
                return _instance;
            }
        }

        public FullBuyConfig Config
        {
            get
            {
                return _config;
            }
        }
         

        private FullBuyMenu()
        {
            var config = AllocatorConfigManager.Load<FullBuyConfig>("CommandAllocator", "FullBuy");

            if (config == null)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Error, $"The FullBuy configuration could not be parsed!");
                return;
            }

            if (_config.Version > config.Version)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Warning, $"The FullBuy configuration is out of date. Consider updating the config. [Current Version: {config.Version} - FullBuy Version: {_config.Version}]");
            }

            _config = config;
        }

        public void OpenPrimaryMenu(CCSPlayerController player, CsTeam team)
        {
            ChatMenu menu = new ChatMenu("Full Buy Primary Menu");

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
            ChatMenu menu = new ChatMenu("Full Buy Secondary Menu");

            var teamString = team == CsTeam.CounterTerrorist ? "CT" : "T";

            foreach (var secondary in _config.AvailableSecondaries.FindAll(x => x.Team == team || x.Team == CsTeam.None))
            {
                menu.AddMenuOption($"{teamString}_{secondary.WeaponName}", OnSelectSecondary);
            }

            MenuManager.CloseActiveMenu(player);
            MenuManager.OpenChatMenu(player, menu);
        }

        public void OpenAWPChanceMenu(CCSPlayerController player, CsTeam team)
        {
            ChatMenu menu = new ChatMenu("AWP Chance Menu");

            var teamString = team == CsTeam.CounterTerrorist ? "CT" : "T";
            var awpSettings = team == CsTeam.CounterTerrorist ? _config.AWPChanceCT : _config.AWPChanceT;

            menu.AddMenuOption($"{teamString}_0%", OnSelectAWPChance);
            foreach (var chance in awpSettings.Chances) 
            {
                menu.AddMenuOption($"{teamString}_{chance}%", OnSelectAWPChance);
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

            MessageUtils.PrintToPlayerOrServer($"You have now selected {ChatColors.Green}{menuText}{ChatColors.White} as your primary for {ChatColors.Green}FullBuy{ChatColors.White} rounds!", player);

            CacheManager.Instance.AddOrUpdateFullBuyPrimaryCache(player, weaponString, team);
            DBManager.Instance.InsertOrUpdateFullBuyPrimaryWeaponString(player.SteamID, weaponString, (int)team);

            FullBuyMenu.Instance.OpenSecondaryMenu(player, team);
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

            MessageUtils.PrintToPlayerOrServer($"You have now selected {ChatColors.Green}{menuText}{ChatColors.White} as your secondary for {ChatColors.Green}FullBuy{ChatColors.White} rounds!", player);

            CacheManager.Instance.AddOrUpdateFullBuySecondaryCache(player, weaponString, team);
            DBManager.Instance.InsertOrUpdateFullBuySecondaryWeaponString(player.SteamID, weaponString, (int)team);

            if(_config.EnableAWPChance)
            {
                FullBuyMenu.Instance.OpenAWPChanceMenu(player, team);
            }
            else
            {
                CacheManager.Instance.AddOrUpdateFullBuyAWPChanceCache(player, 0, team);
            }
            
        }

        private static void OnSelectAWPChance(CCSPlayerController player, ChatMenuOption chatMenuOption)
        {
            var chanceString = chatMenuOption.Text?.Split('_')?.LastOrDefault()?.Replace("%","") ?? string.Empty;

            if(string.IsNullOrWhiteSpace(chanceString)) 
            {
                return;
            }

            if(!int.TryParse(chanceString, out var chance))
            {
                return;
            }

            var team = GetTeam(chatMenuOption.Text ?? string.Empty);

            MessageUtils.PrintToPlayerOrServer($"You have now selected {ChatColors.Green}{chanceString}%{ChatColors.White} chance to receive an AWP for {ChatColors.Green}FullBuy{ChatColors.White} rounds!", player);

            CacheManager.Instance.AddOrUpdateFullBuyAWPChanceCache(player, chance, team);
            DBManager.Instance.InsertOrUpdateFullBuyAWPChance(player.SteamID, chance, (int)team);
        }

        private static CsTeam GetTeam(string weaponString)
        {
            var teamString = weaponString?.Split(separator: '_')?.FirstOrDefault()?.ToUpper() ?? string.Empty;

            CsTeam team = CsTeam.None;

            switch (teamString)
            {
                case "CT":
                    team = CsTeam.CounterTerrorist;
                    break;
                case "T":
                    team = CsTeam.Terrorist;
                    break;
            }

            return team;

        }
    }
}
