using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators;
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

        private RoundTypeEnum _roundType = RoundTypeEnum.Undefined;

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

            this._weaponKitAllocator.Allocate(player);

        }

        public override void ResetForNextRound(bool completeReset = true)
        {
            
        }
    }
}
