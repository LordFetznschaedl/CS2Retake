using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators;
using CS2Retake.Allocators.Exceptions;
using CS2Retake.Configs;
using CS2Retake.Managers.Base;
using CS2Retake.Managers.Interfaces;
using CS2Retake.Utils;
using Microsoft.Extensions.Logging;
using CSZoneNet.Plugin.Utils.Enums;
using CSZoneNet.Plugin.CS2BaseAllocator.Interfaces;
using CS2Retake.Allocators.Factory;
using CounterStrikeSharp.API.Modules.Entities;

namespace CS2Retake.Managers
{
    public class WeaponManager : BaseManager, IWeaponManager
    {
        private static WeaponManager? _instance = null;

        private IBaseAllocator _allocator;

        public IPlugin PluginInstance { get; set; } = null;


        public static WeaponManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WeaponManager();
                }
                return _instance;
            }
        }

        private WeaponManager() 
        {
           
        }

        public void AssignWeapons()
        {
            Utilities.GetPlayers().FindAll(x => x.TeamNum == (int)CsTeam.Terrorist || x.TeamNum == (int)CsTeam.CounterTerrorist).ForEach(x => {
                this.RemoveWeapons(x);
                this.AssignWeapon(x);
                });
        }

        public void AssignWeapon(CCSPlayerController player)
        {
            if(player == null || !player.IsValid) 
            {
                MessageUtils.Log(LogLevel.Error, $"Player is null or not valid");
                return;
            }

            if (player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid)
            {
                MessageUtils.Log(LogLevel.Warning, $"PlayerPawn is null or not valid. This might be because of a disconnected player.");
                return;
            }

            var team = (CsTeam)player.TeamNum;
            if (team != CsTeam.Terrorist && team != CsTeam.CounterTerrorist)
            {
                MessageUtils.Log(LogLevel.Error, $"Player not in Terrorist or CounterTerrorist Team");
                return;
            }

            var roundType = RoundTypeManager.Instance.RoundType;


            if(!this.HandleAllocatorCreation())
            {
                MessageUtils.Log(LogLevel.Error, $"Error while Handling the creation of an allocator");
                return;
            }


            var allocationData = this._allocator.Allocate(player, roundType);



            if (player?.PlayerPawn?.Value?.ItemServices == null)
            {
                MessageUtils.Log(LogLevel.Error,$"Player has no item service");
                return;
            }

            var itemService = new CCSPlayer_ItemServices(player.PlayerPawn.Value.ItemServices.Handle);

            foreach (var grenade in allocationData.grenades)
            {
                var enumMemberValue = EnumUtils.GetEnumMemberAttributeValue(grenade);

                if (!string.IsNullOrWhiteSpace(enumMemberValue))
                {
                    player.GiveNamedItem(enumMemberValue);
                }

            }

            if (!string.IsNullOrWhiteSpace(allocationData.secondaryWeapon))
            {
                player.GiveNamedItem(allocationData.secondaryWeapon);
            }
            if (!string.IsNullOrWhiteSpace(allocationData.primaryWeapon))
            {
                player.GiveNamedItem(allocationData.primaryWeapon);
            }
           
            if(allocationData.zeus)
            {
                player.GiveNamedItem(CsItem.Taser);
            }

            if (allocationData.kit && player.Team == CsTeam.CounterTerrorist)
            {
                itemService.HasDefuser = true;
            }

            switch (allocationData.kevlar)
            {
                case KevlarEnum.Kevlar:
                    player.GiveNamedItem(CsItem.Kevlar);
                    break;
                case KevlarEnum.KevlarHelmet:
                    player.GiveNamedItem(CsItem.AssaultSuit);
                    itemService.HasHelmet = true;
                    break;
            }

            player.ExecuteClientCommand($"slot3; slot2; slot1");
        }

        public void RemoveWeapons(CCSPlayerController player)
        {
            if (player == null || !player.IsValid)
            {
                MessageUtils.Log(LogLevel.Error, $"Player is null or not valid");
                return;
            }

            if (player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid)
            {
                MessageUtils.Log(LogLevel.Warning, $"PlayerPawn is null or not valid. This might be because of a disconnected player.");
                return;
            }

            var weaponService = player?.PlayerPawn?.Value?.WeaponServices ?? null;

            if (weaponService == null)
            {
                MessageUtils.Log(LogLevel.Error, $"WeaponService of player is null");
                return;
            }

            var playerWeaponService = new CCSPlayer_WeaponServices(weaponService.Handle);

            if (playerWeaponService == null)
            {
                MessageUtils.Log(LogLevel.Error, $"PlayerWeaponService is null");
                return;
            }

            playerWeaponService.MyWeapons.Where(weapon => weapon != null && weapon.IsValid && weapon.Value != null && weapon.Value.IsValid && !weapon.Value.DesignerName.Contains("knife")).ToList().ForEach(weapon => weapon.Value?.Remove());

            var playerPawn = player?.PlayerPawn?.Value;

            if(playerPawn == null || !playerPawn.IsValid) 
            {
                MessageUtils.Log(LogLevel.Error, $"PlayerPawn is null or not valid");
                return;
            }

            playerPawn.ArmorValue = 0;

            var itemService = player?.PlayerPawn?.Value?.ItemServices ?? null;

            if(itemService == null)
            {
                MessageUtils.Log(LogLevel.Error, $"Player has no item service");
                return;
            }

            var playerItemService = new CCSPlayer_ItemServices(itemService.Handle);
            
            if(playerItemService == null)
            {
                MessageUtils.Log(LogLevel.Error, $"PlayerItemService is null");
                return;
            }

            playerItemService.HasHelmet = false;
        }

        public void OnGunsCommand(CCSPlayerController? player)
        {
            if (player == null || !player.IsValid)
            {
                MessageUtils.Log(LogLevel.Error, $"Player is null or not valid");
                return;
            }

            if (player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid)
            {
                MessageUtils.Log(LogLevel.Warning, $"PlayerPawn is null or not valid. This might be because of a disconnected player.");
                return;
            }

            if (!this.HandleAllocatorCreation())
            {
                MessageUtils.Log(LogLevel.Error, $"Error while Handling the creation of an allocator");
                return;
            }

            this._allocator.OnGunsCommand(player);
        }

        public void OnPlayerConnected(CCSPlayerController? player)
        {
            if (player == null || !player.IsValid)
            {
                MessageUtils.Log(LogLevel.Error, $"Player is null or not valid");
                return;
            }

            if (player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid)
            {
                MessageUtils.Log(LogLevel.Warning, $"PlayerPawn is null or not valid. This might be because of a disconnected player.");
                return;
            }

            if (!this.HandleAllocatorCreation())
            {
                MessageUtils.Log(LogLevel.Error, $"Error while Handling the creation of an allocator");
                return;
            }

            this._allocator.OnPlayerConnected(player);
        }

        public void OnPlayerDisconnected(CCSPlayerController? player)
        {
            if (player == null || !player.IsValid)
            {
                MessageUtils.Log(LogLevel.Error, $"Player is null or not valid");
                return;
            }

            if (player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value.IsValid)
            {
                MessageUtils.Log(LogLevel.Warning, $"PlayerPawn is null or not valid. This might be because of a disconnected player.");
                return;
            }

            if (!this.HandleAllocatorCreation())
            {
                MessageUtils.Log(LogLevel.Error, $"Error while Handling the creation of an allocator");
                return;
            }

            this._allocator.OnPlayerDisconnected(player);
        }

        public override void ResetForNextMap(bool completeReset = true)
        {
            if (!this.HandleAllocatorCreation())
            {
                MessageUtils.Log(LogLevel.Error, $"Error while Handling the creation of an allocator");
                return;
            }

            this._allocator.ResetForNextRound();
        }

        private bool HandleAllocatorCreation()
        {
            if (this._allocator == null)
            {
                AllocatorFactory factory = new AllocatorFactory();
                this._allocator = factory.GetAllocator(RuntimeConfig.Allocator, this.PluginInstance);
            }

            return this._allocator != null;
        }

        public override void ResetForNextRound(bool completeReset = true)
        {
            if(completeReset) 
            {
                
            }

            RoundTypeManager.Instance.HandleRoundType();
            //this._allocator?.ResetForNextRound();

        }


    }
}
