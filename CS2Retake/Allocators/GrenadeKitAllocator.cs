using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators.Exceptions;
using CS2Retake.Allocators.Interfaces;
using CS2Retake.Entities;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CS2Retake.Allocators
{
    public class GrenadeKitAllocator : IGrenadeAllocator
    {
        private List<GrenadeKitEntity> _grenadeKitEntityList = new List<GrenadeKitEntity>();

        private string _moduleDirectory { get; set; }

        public GrenadeKitAllocator(string moduleDirectoy)
        {
            this._moduleDirectory = moduleDirectoy;
        }

        public List<GrenadeEnum> Allocate(CCSPlayerController player, RoundTypeEnum roundType = RoundTypeEnum.Undefined)
        {
            if (player == null || !player.IsValid)
            {
                throw new AllocatorException("Player is null");
            }

            if (!this._grenadeKitEntityList.Any())
            {
                this.LoadGrenadeKits();
            }

            var team = (CsTeam)player.TeamNum;

            var availableGrenadeKitsForPlayer = this.GetGrenadeKitEntities(team, roundType);

            if (!availableGrenadeKitsForPlayer.Any())
            {
                availableGrenadeKitsForPlayer = this.GetGrenadeKitEntities(team, RoundTypeEnum.Undefined);
            }

            if (!availableGrenadeKitsForPlayer.Any())
            {
                throw new AllocatorException("No Available Grenade Kits");
            }

            var random = new Random();
            var grenadeKit = availableGrenadeKitsForPlayer.OrderBy(x => random.Next()).FirstOrDefault();

            if (grenadeKit == null)
            {
                throw new AllocatorException("Assigned Grenade Kit is null");
            }

            grenadeKit.KitUsedAmount++;
            player.PrintToConsole($"GrenadeKit: {grenadeKit.KitName}");

            return grenadeKit.GrenadeList;
        }

        public void ResetForNextRound()
        {
            this._grenadeKitEntityList.ForEach(x => x.KitUsedAmount = 0);
        }

        private void LoadGrenadeKits()
        {
            var path = this.GetPath();

            var pathExists = Path.Exists(path);

            if (pathExists)
            {
                var jsonWeaponKits = File.ReadAllText(path);

                if (!string.IsNullOrEmpty(jsonWeaponKits))
                {
                    this._grenadeKitEntityList = JsonSerializer.Deserialize<List<GrenadeKitEntity>>(jsonWeaponKits) ?? new List<GrenadeKitEntity>();
                }
            }

            if (!this._grenadeKitEntityList.Any())
            {
                this._grenadeKitEntityList.Add(new GrenadeKitEntity()
                {
                    KitName = "DefaultAll",
                    GrenadeList = new List<GrenadeEnum>() { GrenadeEnum.Flashbang, GrenadeEnum.HighExplosive, },
                    Team = CsTeam.None,
                });
            }

            if (!pathExists)
            {
                this.SaveGrenadeKits();
            }
        }

        private void SaveGrenadeKits()
        {
            File.WriteAllText(this.GetPath(), JsonSerializer.Serialize(this._grenadeKitEntityList));
        }

        private string GetPath()
        {
            var path = Path.Join(this._moduleDirectory, $"configs");

            if (!Path.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Join(path, $"grenadeKits.json");
            return path;
        }

        private List<GrenadeKitEntity> GetGrenadeKitEntities(CsTeam team, RoundTypeEnum roundType) => this._grenadeKitEntityList.Where(x => (x.Team == CsTeam.None || x.Team == team) && !x.KitLimitReached && roundType == x.RoundType).ToList();
    }
}
