using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators.Exceptions;
using CS2Retake.Allocators.Interfaces;
using CS2Retake.Entities;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CSZoneNet.Plugin.Utils.Enums;

namespace CS2Retake.Allocators
{
    public class WeaponKitAllocator : IWeaponAllocator
    {
        private List<WeaponKitEntity> _weaponKitEntityList = new List<WeaponKitEntity>();

        private string _moduleDirectory { get; set; }

        public WeaponKitAllocator(string moduleDirectoy)
        {
            this._moduleDirectory = moduleDirectoy;
        }

        public (string primaryWeapon, string secondaryWeapon, KevlarEnum kevlar, bool kit) Allocate(CCSPlayerController player, RoundTypeEnum roundType = RoundTypeEnum.Undefined)
        {
            if(player == null || !player.IsValid) 
            {
                throw new AllocatorException("Player is null");
            }

            if(!this._weaponKitEntityList.Any())
            {
                this.LoadWeaponKits();
            }

            var team = (CsTeam)player.TeamNum;

            var availableWeaponKitsForPlayer = this.GetWeaponKitEntities(team, roundType);

            if (!availableWeaponKitsForPlayer.Any())
            {
                availableWeaponKitsForPlayer = this.GetWeaponKitEntities(team, RoundTypeEnum.Undefined);
            }

            if(!availableWeaponKitsForPlayer.Any())
            {
                throw new AllocatorException($"No Available Weapon Kits [PlayerTeam: {team}]");
            }

            var random = new Random();
            var weaponKit = availableWeaponKitsForPlayer.OrderBy(x => random.Next()).FirstOrDefault();

            if (weaponKit == null) 
            {
                throw new AllocatorException("Assigned Weapon Kit is null");
            }

            weaponKit.KitUsedAmount++;
            player.PrintToConsole($"WeaponKit: {weaponKit.KitName}");

            return (weaponKit.PrimaryWeapon, weaponKit.SecondaryWeapon, weaponKit.Kevlar, weaponKit.DefuseKit && team == CsTeam.CounterTerrorist);
        }

        public void ResetForNextRound()
        {
            this._weaponKitEntityList.ForEach(x => x.KitUsedAmount = 0);
        }

        private void LoadWeaponKits()
        { 
            var path = this.GetPath();

            var pathExists = Path.Exists(path);

            if (pathExists)
            {
                var jsonWeaponKits = File.ReadAllText(path);

                if (!string.IsNullOrEmpty(jsonWeaponKits))
                {
                    this._weaponKitEntityList = JsonSerializer.Deserialize<List<WeaponKitEntity>>(jsonWeaponKits) ?? new List<WeaponKitEntity>();
                }
            }

            if(!this._weaponKitEntityList.Any())
            {
                this._weaponKitEntityList.Add(new WeaponKitEntity()
                {
                    KitName = "DefaultCT",
                    PrimaryWeapon = "weapon_m4a1",
                    SecondaryWeapon = "weapon_hkp2000",
                    Team = CsTeam.CounterTerrorist,
                    Kevlar = KevlarEnum.Kevlar,
                });

                this._weaponKitEntityList.Add(new WeaponKitEntity()
                {
                    KitName = "DefaultT",
                    PrimaryWeapon = "weapon_ak47",
                    SecondaryWeapon = "weapon_glock",
                    Team = CsTeam.Terrorist,
                    DefuseKit = false,
                });
            }

            if(!pathExists) 
            {
                this.SaveWeaponKits();
            }
        }

        private void SaveWeaponKits() 
        {
            File.WriteAllText(this.GetPath(), JsonSerializer.Serialize(this._weaponKitEntityList));
        }

        private string GetPath()
        {
            var path = Path.Join(this._moduleDirectory, $"configs");

            if (!Path.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Join(path, $"weaponKits.json");
            return path;
        }

        private List<WeaponKitEntity> GetWeaponKitEntities(CsTeam team, RoundTypeEnum roundType) => this._weaponKitEntityList.Where(x => (x.Team == CsTeam.None || x.Team == team) && !x.KitLimitReached && roundType == x.RoundType).ToList();
    }
}
