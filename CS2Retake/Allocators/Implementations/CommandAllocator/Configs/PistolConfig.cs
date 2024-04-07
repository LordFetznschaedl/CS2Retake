using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators.Implementations.CommandAllocator.Entities;
using CSZoneNet.Plugin.CS2BaseAllocator.Configs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Configs
{
    public class PistolConfig : BaseAllocatorConfig
    {
        public List<WeaponEntity> AvailableSecondaries { get; set; } = new List<WeaponEntity>()
        {
            new WeaponEntity("Deagle", "weapon_deagle"),
            new WeaponEntity("P250", "weapon_p250"),
            new WeaponEntity("CZ75", "weapon_cz75a"),
            new WeaponEntity("Dual Berettas", "weapon_elite"),

            new WeaponEntity("USP-s", "weapon_usp_silencer", CsTeam.CounterTerrorist),
            new WeaponEntity("P2000", "weapon_hkp2000", CsTeam.CounterTerrorist),
            new WeaponEntity("FiveSeven", "weapon_fiveseven", CsTeam.CounterTerrorist),

            new WeaponEntity("Glock", "weapon_glock", CsTeam.Terrorist),
            new WeaponEntity("Tec-9", "weapon_tec9", CsTeam.Terrorist),
        };

        public PistolConfig()
        {
            this.Version = 1;
        }
    }
}
