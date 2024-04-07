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
    public class MidConfig : BaseAllocatorConfig
    {
        public List<WeaponEntity> AvailablePrimaries { get; set; } = new List<WeaponEntity>()
        {
            new WeaponEntity("P90", "weapon_p90"),
            new WeaponEntity("MP-5", "weapon_mp5sd"),
            new WeaponEntity("UMP-45", "weapon_ump45"),
            new WeaponEntity("PP-Bizon", "weapon_bizon"),
            new WeaponEntity("MP7", "weapon_mp7"),

            new WeaponEntity("MP-9", "weapon_mp9", CsTeam.CounterTerrorist),

            new WeaponEntity("Mac-10", "weapon_mac10", CsTeam.Terrorist),
        };

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

        public MidConfig()
        {
            this.Version = 1;
        }
    }
}
