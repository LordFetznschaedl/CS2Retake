using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators;
using CS2Retake.Allocators.Exceptions;
using CS2Retake.Managers.Base;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Managers
{
    public class WeaponManager : BaseManager
    {
        private static WeaponManager? _instance = null;

        public string ModuleDirectory { get; set; }

        private WeaponKitAllocator _weaponKitAllocator;

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

            (string primaryWeapon, string secondaryWeapon, KevlarEnum kevlar, bool kit) allocationData;
            try
            {
                allocationData = this._weaponKitAllocator.Allocate(player);
            }
            catch(AllocatorException ex)
            {
                this.Log($"An error happened while assigning the weapons. Message: {ex.Message}");
                MessageUtils.PrintToPlayerOrServer($"An error occurred while assigning your weapons. Using fallback weapons!");

                allocationData = ("weapon_ssg08", "weapon_p250", KevlarEnum.KevlarHelmet, (CsTeam)player.TeamNum == CsTeam.CounterTerrorist);
            }

            player.GiveNamedItem("weapon_knife");
            player.GiveNamedItem(allocationData.primaryWeapon);
            player.GiveNamedItem(allocationData.secondaryWeapon);

            if (allocationData.kit)
            {
                player.GiveNamedItem("item_cutters");
            }

            switch (allocationData.kevlar)
            {
                case KevlarEnum.Kevlar:
                    player.GiveNamedItem("item_kevlar");
                    break;
                case KevlarEnum.KevlarHelmet:
                    player.GiveNamedItem("item_assaultsuit");
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

            weaponService.MyWeapons.Where(weapon => weapon.IsValid && weapon.Value.IsValid).ToList().ForEach(weapon => weapon.Value.Remove());
        }

        public void RandomRoundType()
        {
            this.RoundType = (RoundTypeEnum)new Random().Next(0, Enum.GetNames(typeof(RoundTypeEnum)).Length);
        }

        public override void ResetForNextRound(bool completeReset = true)
        {
            if(completeReset) 
            {
                
            }

            this.RandomRoundType();
        }
    }
}
