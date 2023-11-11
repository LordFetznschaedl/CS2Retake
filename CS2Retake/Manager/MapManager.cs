using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Entity;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CS2Retake.Manager
{
    public class MapManager
    {
        private static MapManager _instance;
        public string ModuleName { get; set; }
        public MapEntity CurrentMap { get; set; }

        public BombSiteEnum BombSite { get; set; } = BombSiteEnum.Undefined;
        public bool HasToBeInBombZone { get; set; } = false;

        public int TerroristRoundWinStreak { get; set; } = 0;

        public static MapManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new MapManager();
            }
            return _instance;
        }

        private MapManager() { }


        public void RandomBombSite()
        {
            this.BombSite = (BombSiteEnum)new Random().NextInt64(0, 1);
        }

        public void AddSpawn(CCSPlayerController player, CsTeam team, BombSiteEnum bombSite)
        {
            var isInBombZone = player.PlayerPawn.Value.InBombZone;
            var playerPawn = player.PlayerPawn.Value;

            if(playerPawn == null) 
            {
                this.Log($"playerPawn is null");
                return;
            }
            if (playerPawn.AbsOrigin == null)
            {
                this.Log($"playerPawn position is null");
                return;
            }
            if (playerPawn.AbsRotation == null)
            {
                this.Log($"playerPawn rotation is null");
                return;
            }

            var newSpawnPoint = new SpawnPointEntity(playerPawn.AbsOrigin, playerPawn.AbsRotation, bombSite, team, isInBombZone);

            this.CurrentMap.SpawnPoints.Add(newSpawnPoint);

            player.PrintToChat($"SpawnPoint added! BombSite: {bombSite} - Team: {team} - isInBombZone: {isInBombZone}");
        }

        public void TeleportPlayerToSpawn(CCSPlayerController player, BombSiteEnum bombSite, int? spawnIndex = null)
        {
            SpawnPointEntity? spawn;
            if (!spawnIndex.HasValue)
            {
                if(player.PlayerPawn.Value.WeaponServices == null)
                {
                    this.Log($"WeaponServicxe of Player is null!");
                    return;
                }

                var team = (CsTeam)player.TeamNum;
                
                if(team == CsTeam.Terrorist && this.HasToBeInBombZone) 
                {
                    this.HasToBeInBombZone = false;
                }
                
                spawn = this.CurrentMap.GetRandomSpawn(team, bombSite, team == CsTeam.CounterTerrorist ? false : this.HasToBeInBombZone);
            }
            else if (spawnIndex.Value > (this.CurrentMap.SpawnPoints.Count - 1) || spawnIndex.Value < 0)
            {
                return;
            }
            else
            {
                spawn = this.CurrentMap.SpawnPoints[spawnIndex.Value];
            }

            if (spawn == null)
            {
                this.Log($"Spawn is null. Moving player to Spectator");
                player.SwitchTeam(CsTeam.Spectator);
                return;
            }

            spawn.SpawnIsInUse = true;

            player.PrintToConsole($"[CS2Retake] Spawnpoint: {spawn.SpawnId}");

            player.PlayerPawn.Value.Teleport(spawn.Position, spawn.QAngle, new Vector(0f, 0f, 0f));

        }

        public void ResetForNextRound()
        {
            this.RandomBombSite();
            this.CurrentMap.ResetSpawnInUse();
            this.HasToBeInBombZone = true;
        }

        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{this.ModuleName}:{this.GetType().Name}] {message}");
            Console.ResetColor();
        }
    }
}
