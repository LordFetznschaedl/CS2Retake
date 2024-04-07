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

        public void OpenSecondaryMenu(CCSPlayerController player)
        {
            ChatMenu menu = new ChatMenu("Pistol Menu");

            foreach (var primary in _config.AvailableSecondaries.FindAll(x => x.Team == player.Team || x.Team == CsTeam.None))
            {
                menu.AddMenuOption(primary.WeaponName, OnSelectSecondary);
            }

            MenuManager.OpenChatMenu(player, menu);
        }

        private static void OnSelectSecondary(CCSPlayerController player, ChatMenuOption chatMenuOption)
        {
            var weaponString = _config.AvailableSecondaries.FirstOrDefault(x => x.WeaponName.Equals(chatMenuOption.Text))?.WeaponString;

            if (string.IsNullOrWhiteSpace(weaponString))
            {
                return;
            }

            MessageUtils.PrintToPlayerOrServer($"You have now selected {ChatColors.Green}{chatMenuOption.Text}{ChatColors.White} as your pistol for {ChatColors.Green}Pistol{ChatColors.White} rounds!", player);

            CacheManager.Instance.AddOrUpdatePistolCache(player, weaponString);
        }
    }
}
