using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators.Implementations.CommandAllocator.Entities;
using CSZoneNet.Plugin.CS2BaseAllocator.Configs.Base;


namespace CS2Retake.Allocators.Implementations.CommandAllocator.Configs
{
    public class FullBuyConfig : BaseAllocatorConfig
    {
        public List<WeaponEntity> AvailablePrimaries { get; set; } = new List<WeaponEntity>()
        {
            new WeaponEntity("M4A4", "weapon_m4a1", CsTeam.CounterTerrorist),
            new WeaponEntity("M4A1-s", "weapon_m4a1_silencer", CsTeam.CounterTerrorist),
            new WeaponEntity("Famas", "weapon_famas", CsTeam.CounterTerrorist),
            new WeaponEntity("AUG", "weapon_aug", CsTeam.CounterTerrorist),

            new WeaponEntity("AK-47", "weapon_ak47", CsTeam.Terrorist),
            new WeaponEntity("Galil", "weapon_galilar", CsTeam.Terrorist),
            new WeaponEntity("SG-553", "weapon_sg556", CsTeam.Terrorist),

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

        public ChanceEntity AWPChanceCT { get; set; } = new ChanceEntity()
        {
            Team = CsTeam.CounterTerrorist,
            WeaponName = "AWP",
            WeaponString = "weapon_awp",
            Limit = 1,
            Chances = new List<int>() { 10, 20, 30, 40, 50 },
        };

        public ChanceEntity AWPChanceT { get; set; } = new ChanceEntity()
        {
            Team = CsTeam.Terrorist,
            WeaponName = "AWP",
            WeaponString = "weapon_awp",
            Limit = 1,
            Chances = new List<int>() { 10, 20, 30, 40, 50 },
        };

        public bool EnableAWPChance { get; set; } = true;

        public FullBuyConfig()
        {
            this.Version = 1;
        }
    }
}
