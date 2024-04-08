using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators.Implementations.CommandAllocator.Configs;
using CS2Retake.Allocators.Implementations.CommandAllocator.Manager;
using CS2Retake.Managers;
using CS2Retake.Utils;
using CSZoneNet.Plugin.CS2BaseAllocator.Configs;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Menus
{
    public class PistolMenu
    {
        private static PistolConfig _config { get; set; } = new PistolConfig();


        private static PistolMenu? _instance = null;
        public static PistolMenu Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PistolMenu();
                }
                return _instance;
            }
        }

        private PistolMenu()
        {
            _config = AllocatorConfigManager.Load<PistolConfig>("CommandAllocator", "Pistol");
        }

        public void OpenSecondaryMenu(CCSPlayerController player, CsTeam team)
        {
            ChatMenu menu = new ChatMenu("Pistol Menu");

            var teamString = team == CsTeam.CounterTerrorist ? "CT" : "T";

            foreach (var secondary in _config.AvailableSecondaries.FindAll(x => x.Team == team || x.Team == CsTeam.None))
            {
                menu.AddMenuOption($"{teamString}_{secondary.WeaponName}", OnSelectSecondary);
            }

            MenuManager.OpenChatMenu(player, menu);
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

            MessageUtils.PrintToPlayerOrServer($"You have now selected {ChatColors.Green}{menuText}{ChatColors.White} as your pistol for {ChatColors.Green}Pistol{ChatColors.White} rounds!", player);

            MenuManager.CloseActiveMenu(player);
            CacheManager.Instance.AddOrUpdatePistolCache(player, weaponString, team);
            DBManager.Instance.InsertOrUpdatePistolWeaponString(player.SteamID, weaponString, (int)team);
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
