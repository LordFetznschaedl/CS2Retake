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

        private FullBuyMenu()
        {
            _config = AllocatorConfigManager.Load<FullBuyConfig>("CommandAllocator", "FullBuy");
        }

        public void OpenPrimaryMenu(CCSPlayerController player)
        {
            ChatMenu menu = new ChatMenu("Full Buy Primary Menu");

            foreach (var primary in _config.AvailablePrimaries.FindAll(x => x.Team == player.Team || x.Team == CsTeam.None))
            {
                menu.AddMenuOption(primary.WeaponName, OnSelectPrimary);
            }

            MenuManager.OpenChatMenu(player, menu);
        }

        public void OpenSecondaryMenu(CCSPlayerController player)
        {
            ChatMenu menu = new ChatMenu("Full Buy Secondary Menu");

            foreach (var primary in _config.AvailableSecondaries.FindAll(x => x.Team == player.Team || x.Team == CsTeam.None))
            {
                menu.AddMenuOption(primary.WeaponName, OnSelectSecondary);
            }

            MenuManager.OpenChatMenu(player, menu);
        }

        private void OnSelectPrimary(CCSPlayerController player, ChatMenuOption chatMenuOption)
        {
            var weaponString = _config.AvailablePrimaries.FirstOrDefault(x => x.WeaponName.Equals(chatMenuOption.Text))?.WeaponString;

            if(string.IsNullOrWhiteSpace(weaponString))
            {
                return;
            }

            CacheManager.Instance.AddOrUpdateFullBuyPrimaryCache(player, weaponString);

            MessageUtils.PrintToPlayerOrServer($"You have now selected {ChatColors.Green}{chatMenuOption.Text}{ChatColors.White} as your primary for {ChatColors.Green}FullBuy{ChatColors.White} rounds!", player);

            OpenSecondaryMenu(player);
        }

        private void OnSelectSecondary(CCSPlayerController player, ChatMenuOption chatMenuOption)
        {
            var weaponString = _config.AvailableSecondaries.FirstOrDefault(x => x.WeaponName.Equals(chatMenuOption.Text))?.WeaponString;

            if (string.IsNullOrWhiteSpace(weaponString))
            {
                return;
            }

            CacheManager.Instance.AddOrUpdateFullBuySecondaryCache(player, weaponString);

            MessageUtils.PrintToPlayerOrServer($"You have now selected {ChatColors.Green}{chatMenuOption.Text}{ChatColors.White} as your secondary for {ChatColors.Green}FullBuy{ChatColors.White} rounds!", player);
        }
    }
}
