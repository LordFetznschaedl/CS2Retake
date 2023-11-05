using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace CS2Retake.Entity
{
    public class MapEntity
    {
        public string MapName { get; set; }

        public List<SpawnPointEntity> SpawnPoints { get; set; } = new List<SpawnPointEntity>();
        public Queue<ulong> PlayerQueue { get; set; } = new Queue<ulong>();

        private string _moduleDirectory { get; set; }
        private string _moduleName { get; set; }


        public MapEntity(string mapName, string moduleDirectoy, string moduleName)
        {
            this.MapName = mapName;
            this._moduleDirectory = moduleDirectoy;
            this._moduleName = moduleName;

            this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1485.635986f, 1799.516846f, - 9.545544f), new QAngle(1.386139f, 134.525223f, 0.000000f), CsTeam.Terrorist, BombSiteEnum.A, SpawnTypeEnum.Normal));
            this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-439.763672f, 2293.371826f, - 119.998161f), new QAngle(3.124207f, - 18.375731f, 0.000000f), CsTeam.CounterTerrorist, BombSiteEnum.A, SpawnTypeEnum.Normal));
        }

        private SpawnPointEntity? GetRandomSpawn(CsTeam team)
        {
            if(!this.SpawnPoints.Any()) 
            {
                this.ReadSpawns();
            }

            var spawnChoices = this.SpawnPoints.Where(x => x.Team == team && !x.SpawnIsInUse).ToList();

            if(!spawnChoices.Any())
            {
                return null;
            }

            var random = new Random();


            return spawnChoices.ElementAt(random.Next(spawnChoices.Count - 1));
        }

        public void TeleportPlayerToSpawn(CCSPlayerController player, int? spawnIndex = null)
        {
            SpawnPointEntity? spawn;
            if(!spawnIndex.HasValue)
            {
                spawn = this.GetRandomSpawn((CsTeam)player.TeamNum);
            }
            else if(spawnIndex.Value > (this.SpawnPoints.Count - 1) || spawnIndex.Value < 0)
            {
                return;
            }
            else
            {
                spawn = this.SpawnPoints[spawnIndex.Value];
            }

            if(player.IsBot)
            {
                return;
            }

            if(spawn == null)
            {
                this.Log($"Spawn is null. Moving player to Spectator");
                player.SwitchTeam(CsTeam.Spectator);
                return;
            }

            spawn.SpawnIsInUse = true;

            player.PlayerPawn.Value.Teleport(spawn.Position, spawn.QAngle, new Vector(0f, 0f, 0f));
            
        }

        public void ReadSpawns()
        {
            var jsonSpawnPoints = File.ReadAllText(this.GetPath());

            if(string.IsNullOrEmpty(jsonSpawnPoints))
            {
                return;
            }

            this.SpawnPoints = JsonSerializer.Deserialize<List<SpawnPointEntity>>(jsonSpawnPoints) ?? new List<SpawnPointEntity>();
        }

        public void WriteSpawns() 
        {
            File.WriteAllText(this.GetPath(), JsonSerializer.Serialize(this.SpawnPoints));
        }

        private string GetPath()
        {
            var path = Path.Join(this._moduleDirectory, $"spawns");

            if(!Path.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Join(path, $"{this.MapName}.json");

            return path;
        }

        public void ResetSpawnInUse()
        {
            this.SpawnPoints.ForEach(spawn => spawn.SpawnIsInUse = false);
        }

        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{this._moduleName}:{this.GetType().Name}] {message}");
            Console.ResetColor();
        }
    }
}
