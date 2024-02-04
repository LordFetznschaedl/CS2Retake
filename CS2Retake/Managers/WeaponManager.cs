using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators;
using CS2Retake.Allocators.Exceptions;
using CS2Retake.Allocators.Interfaces;
using CS2Retake.Configs;
using CS2Retake.Managers.Base;
using CS2Retake.Managers.Interfaces;
using CS2Retake.Utils;
using Microsoft.Extensions.Logging;

namespace CS2Retake.Managers
{
    public class WeaponManager : BaseManager, IWeaponManager
    {
        private static WeaponManager? _instance = null;

        private IAllocator _allocator;
        private IWeaponAllocator _weaponKitAllocator;
        private IGrenadeAllocator _grenadeKitAllocator;



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

        private WeaponManager() { }

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

            if(this._weaponKitAllocator == null) 
            {
                this._weaponKitAllocator = new WeaponKitAllocator(RuntimeConfig.ModuleDirectory);
            }
            if(this._grenadeKitAllocator == null)
            {
                this._grenadeKitAllocator = new GrenadeKitAllocator(RuntimeConfig.ModuleDirectory);
            }

            (string primaryWeapon, string secondaryWeapon, KevlarEnum kevlar, bool kit) weaponAllocationData = (string.Empty, string.Empty, KevlarEnum.None, false);
            List<GrenadeEnum> grenadeAllocationList = new List<GrenadeEnum>();
            try
            {
                if (this._allocator != null)
                {
                    var allocationData = this._allocator.Allocate(player, roundType);
                    weaponAllocationData = (allocationData.primaryWeapon, allocationData.secondaryWeapon, allocationData.kevlar, allocationData.kit);
                    grenadeAllocationList = allocationData.grenades;
                }
                else
                {
                    if (this._weaponKitAllocator != null)
                    {
                        weaponAllocationData = this._weaponKitAllocator.Allocate(player, roundType);
                    }
                    if (this._grenadeKitAllocator != null)
                    {
                        grenadeAllocationList = this._grenadeKitAllocator.Allocate(player, roundType);
                    }
                }

            }
            catch(AllocatorException ex)
            {
                MessageUtils.Log(LogLevel.Error,$"An error happened while assigning the weapons. Message: {ex.Message}");
                MessageUtils.PrintToPlayerOrServer($"An error occurred while assigning your weapons. Using fallback weapons!");

                weaponAllocationData = (string.Empty, "weapon_deagle", KevlarEnum.KevlarHelmet, (CsTeam)player.TeamNum == CsTeam.CounterTerrorist);
            }


            if (player?.PlayerPawn?.Value?.ItemServices == null)
            {
                MessageUtils.Log(LogLevel.Error,$"Player has no item service");
                return;
            }

            var itemService = new CCSPlayer_ItemServices(player.PlayerPawn.Value.ItemServices.Handle);

            foreach (var grenade in grenadeAllocationList)
            {
                var enumMemberValue = EnumUtils.GetEnumMemberAttributeValue(grenade);

                if (!string.IsNullOrWhiteSpace(enumMemberValue))
                {
                    player.GiveNamedItem(enumMemberValue);
                }

            }

            if (!string.IsNullOrWhiteSpace(weaponAllocationData.secondaryWeapon))
            {
                player.GiveNamedItem(weaponAllocationData.secondaryWeapon);
            }
            if (!string.IsNullOrWhiteSpace(weaponAllocationData.primaryWeapon))
            {
                player.GiveNamedItem(weaponAllocationData.primaryWeapon);
            }
           

            if (weaponAllocationData.kit)
            {
                itemService.HasDefuser = true;
            }

            switch (weaponAllocationData.kevlar)
            {
                case KevlarEnum.Kevlar:
                    player.GiveNamedItem(CsItem.Kevlar);
                    break;
                case KevlarEnum.KevlarHelmet:
                    player.GiveNamedItem(CsItem.AssaultSuit);
                    itemService.HasHelmet = true;
                    break;
            }

            
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

        public override void ResetForNextRound(bool completeReset = true)
        {
            if(completeReset) 
            {
                
            }

            RoundTypeManager.Instance.HandleRoundType();
            this._allocator?.ResetForNextRound();
            this._weaponKitAllocator?.ResetForNextRound();
            this._grenadeKitAllocator?.ResetForNextRound();
        }

        public override void ResetForNextMap(bool completeReset = true)
        {

        }
    }
}
