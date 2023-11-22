using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators;
using CS2Retake.Allocators.Exceptions;
using CS2Retake.Allocators.Interfaces;
using CS2Retake.Managers.Base;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Managers
{
    public class WeaponManager : BaseManager
    {
        private static WeaponManager? _instance = null;

        public string ModuleDirectory { get; set; }

        private IAllocator _allocator;
        private IWeaponAllocator _weaponKitAllocator;
        private IGrenadeAllocator _grenadeKitAllocator;

        public RoundTypeEnum RoundType { get; private set; } = RoundTypeEnum.Undefined;

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


        public void AssignWeapon(CCSPlayerController player)
        {
            if(player == null || !player.IsValid) 
            {
                this.Log($"Player is null or not valid");
                return;
            }

            var team = (CsTeam)player.TeamNum;
            if (team != CsTeam.Terrorist && team != CsTeam.CounterTerrorist)
            {
                this.Log($"Player not in Terrorist or CounterTerrorist Team");
                return;
            }

            if(this._weaponKitAllocator == null) 
            {
                this._weaponKitAllocator = new WeaponKitAllocator(this.ModuleDirectory);
            }
            if(this._grenadeKitAllocator == null)
            {
                this._grenadeKitAllocator = new GrenadeKitAllocator(this.ModuleDirectory);
            }

            (string primaryWeapon, string secondaryWeapon, KevlarEnum kevlar, bool kit) weaponAllocationData = (string.Empty, string.Empty, KevlarEnum.None, false);
            List<GrenadeEnum> grenadeAllocationList = new List<GrenadeEnum>();
            try
            {
                if (this._allocator != null)
                {
                    var allocationData = this._allocator.Allocate(player, this.RoundType);
                    weaponAllocationData = (allocationData.primaryWeapon, allocationData.secondaryWeapon, allocationData.kevlar, allocationData.kit);
                    grenadeAllocationList = allocationData.grenades;
                }
                else
                {
                    if (this._weaponKitAllocator != null)
                    {
                        weaponAllocationData = this._weaponKitAllocator.Allocate(player, this.RoundType);
                    }
                    if (this._grenadeKitAllocator != null)
                    {
                        grenadeAllocationList = this._grenadeKitAllocator.Allocate(player, this.RoundType);
                    }
                }

            }
            catch(AllocatorException ex)
            {
                this.Log($"An error happened while assigning the weapons. Message: {ex.Message}");
                MessageUtils.PrintToPlayerOrServer($"An error occurred while assigning your weapons. Using fallback weapons!");

                weaponAllocationData = (string.Empty, "weapon_deagle", KevlarEnum.KevlarHelmet, (CsTeam)player.TeamNum == CsTeam.CounterTerrorist);
            }

            

            if (player.PlayerPawn.Value.ItemServices == null)
            {
                this.Log($"Player has no item service");
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

            if (!string.IsNullOrWhiteSpace(weaponAllocationData.primaryWeapon))
            {
                player.GiveNamedItem(weaponAllocationData.primaryWeapon);
            }
            if (!string.IsNullOrWhiteSpace(weaponAllocationData.secondaryWeapon))
            {
                player.GiveNamedItem(weaponAllocationData.secondaryWeapon);
            }

            if (weaponAllocationData.kit)
            {
                itemService.HasDefuser = true;
            }

            switch (weaponAllocationData.kevlar)
            {
                case KevlarEnum.Kevlar:
                    player.GiveNamedItem("item_kevlar");
                    break;
                case KevlarEnum.KevlarHelmet:
                    player.GiveNamedItem("item_assaultsuit");
                    itemService.HasHelmet = true;
                    break;
            }

            
        }

        public void RemoveWeapons(CCSPlayerController player)
        {
            if (player == null || !player.IsValid)
            {
                this.Log($"Player is null or not valid");
                return;
            }

            var weaponService = player.PlayerPawn.Value.WeaponServices;
            if (weaponService == null)
            {
                this.Log($"WeaponService of player is null");
                return;
            }

            weaponService.MyWeapons.Where(weapon => weapon.IsValid && weapon.Value.IsValid && !weapon.Value.DesignerName.Contains("knife")).ToList().ForEach(weapon => weapon.Value.Remove());
        }

        public void RandomRoundType()
        {
            this.RoundType = (RoundTypeEnum)new Random().Next(0, Enum.GetNames(typeof(RoundTypeEnum)).Length-1);
        }

        public override void ResetForNextRound(bool completeReset = true)
        {
            if(completeReset) 
            {
                
            }

            this.RandomRoundType();
            this._allocator?.ResetForNextRound();
            this._weaponKitAllocator?.ResetForNextRound();
            this._grenadeKitAllocator?.ResetForNextRound();
        }
    }
}
