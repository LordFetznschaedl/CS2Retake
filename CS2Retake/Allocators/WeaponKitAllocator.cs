using System.Text.Json;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators.Exceptions;
using CS2Retake.Allocators.Interfaces;
using CS2Retake.Entities;
using CS2Retake.Utils;

namespace CS2Retake.Allocators;

public class WeaponKitAllocator : IWeaponAllocator
{
    private List<WeaponKitEntity> _weaponKitEntityList = new();

    public WeaponKitAllocator(string moduleDirectoy)
    {
        _moduleDirectory = moduleDirectoy;
    }

    private string _moduleDirectory { get; }

    public (string primaryWeapon, string secondaryWeapon, KevlarEnum kevlar, bool kit) Allocate(
        CCSPlayerController player, RoundTypeEnum roundType = RoundTypeEnum.Undefined)
    {
        if (player == null || !player.IsValid) throw new AllocatorException("Player is null");

        if (!_weaponKitEntityList.Any()) LoadWeaponKits();

        var team = (CsTeam)player.TeamNum;

        var availableWeaponKitsForPlayer = GetWeaponKitEntities(team, roundType);

        if (!availableWeaponKitsForPlayer.Any())
            availableWeaponKitsForPlayer = GetWeaponKitEntities(team, RoundTypeEnum.Undefined);

        if (!availableWeaponKitsForPlayer.Any())
            throw new AllocatorException($"No Available Weapon Kits [PlayerTeam: {team}]");

        var random = new Random();
        var weaponKit = availableWeaponKitsForPlayer.OrderBy(x => random.Next()).FirstOrDefault();

        if (weaponKit == null) throw new AllocatorException("Assigned Weapon Kit is null");

        weaponKit.KitUsedAmount++;
        player.PrintToConsole($"WeaponKit: {weaponKit.KitName}");

        return (weaponKit.PrimaryWeapon, weaponKit.SecondaryWeapon, weaponKit.Kevlar,
            weaponKit.DefuseKit && team == CsTeam.CounterTerrorist);
    }

    public void ResetForNextRound()
    {
        _weaponKitEntityList.ForEach(x => x.KitUsedAmount = 0);
    }

    private void LoadWeaponKits()
    {
        var path = GetPath();

        var pathExists = Path.Exists(path);

        if (pathExists)
        {
            var jsonWeaponKits = File.ReadAllText(path);

            if (!string.IsNullOrEmpty(jsonWeaponKits))
                _weaponKitEntityList = JsonSerializer.Deserialize<List<WeaponKitEntity>>(jsonWeaponKits) ??
                                       new List<WeaponKitEntity>();
        }

        if (!_weaponKitEntityList.Any())
        {
            _weaponKitEntityList.Add(new WeaponKitEntity
            {
                KitName = "DefaultCT",
                PrimaryWeapon = "weapon_m4a1",
                SecondaryWeapon = "weapon_hkp2000",
                Team = CsTeam.CounterTerrorist,
                Kevlar = KevlarEnum.Kevlar
            });

            _weaponKitEntityList.Add(new WeaponKitEntity
            {
                KitName = "DefaultT",
                PrimaryWeapon = "weapon_ak47",
                SecondaryWeapon = "weapon_glock",
                Team = CsTeam.Terrorist,
                DefuseKit = false
            });
        }

        if (!pathExists) SaveWeaponKits();
    }

    private void SaveWeaponKits()
    {
        File.WriteAllText(GetPath(), JsonSerializer.Serialize(_weaponKitEntityList));
    }

    private string GetPath()
    {
        var path = Path.Join(_moduleDirectory, "configs");

        if (!Path.Exists(path)) Directory.CreateDirectory(path);

        path = Path.Join(path, "weaponKits.json");
        return path;
    }

    private List<WeaponKitEntity> GetWeaponKitEntities(CsTeam team, RoundTypeEnum roundType)
    {
        return _weaponKitEntityList.Where(x =>
            (x.Team == CsTeam.None || x.Team == team) && !x.KitLimitReached && roundType == x.RoundType).ToList();
    }
}