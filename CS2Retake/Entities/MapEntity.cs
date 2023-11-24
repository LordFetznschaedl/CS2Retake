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

namespace CS2Retake.Entities
{
    public class MapEntity
    {
        public string MapName { get; set; }

        public List<SpawnPointEntity> SpawnPoints { get; set; } = new List<SpawnPointEntity>();
        public Queue<ulong> PlayerQueue { get; set; } = new Queue<ulong>();

        private string _moduleDirectory { get; set; }


        public MapEntity(string mapName, string moduleDirectoy)
        {
            this.MapName = mapName;
            this._moduleDirectory = moduleDirectoy;
        }

        public SpawnPointEntity? GetRandomSpawn(CsTeam team, BombSiteEnum bombSite, bool hasToBeInBombZone)
        {
            if(!this.SpawnPoints.Any()) 
            {
                this.LoadSpawns();
            }

            var spawnChoices = this.SpawnPoints.Where(x => x.Team == team && x.SpawnUsedBy == null && x.BombSite == bombSite).ToList();

            if(!spawnChoices.Any())
            {
                this.Log($"No spawn choices found.");
                return null;
            }

            if(team == CsTeam.Terrorist && hasToBeInBombZone)
            {
                spawnChoices = spawnChoices.Where(x => x.IsInBombZone).ToList();

                if (!spawnChoices.Any())
                {
                    this.Log($"hasToBeInBombZone has not found any spawns.");
                    return null;
                }
            }

            var random = new Random();
            return spawnChoices.OrderBy(x => random.Next()).FirstOrDefault();
        }



        public void LoadSpawns()
        {
            var path = this.GetPath();

            if(!File.Exists(path)) 
            {
                return;
            }

            var jsonSpawnPoints = File.ReadAllText(path);

            if(string.IsNullOrEmpty(jsonSpawnPoints))
            {
                return;
            }

            this.SpawnPoints = JsonSerializer.Deserialize<List<SpawnPointEntity>>(jsonSpawnPoints) ?? new List<SpawnPointEntity>();
        }

        public void SaveSpawns() 
        {
            this.SpawnPoints.Where(spawnPoint => spawnPoint.SpawnId == Guid.Empty).ToList().ForEach(spawnPoint => spawnPoint.SpawnId = Guid.NewGuid());

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
            this.SpawnPoints.ForEach(spawn => spawn.SpawnUsedBy = null);
        }

        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{MessageUtils.ModuleName}:{this.GetType().Name}] {message}");
            Console.ResetColor();
        }
    }
}
