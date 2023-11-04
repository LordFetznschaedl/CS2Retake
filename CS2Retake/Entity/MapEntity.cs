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

namespace CS2Retake.Entity
{
    public class MapEntity
    {
        public string MapName { get; set; }

        public List<SpawnPointEntity> _spawnPoints { get; set; } = new List<SpawnPointEntity>();

        private string _moduleDirectory { get; set; }


        public MapEntity(string mapName, string moduleDirectoy)
        {
            this.MapName = mapName;
            this._moduleDirectory = moduleDirectoy;

            this._spawnPoints.Add(new SpawnPointEntity(new Vector(1485.635986f, 1799.516846f, - 9.545544f), new QAngle(1.386139f, 134.525223f, 0.000000f), CsTeam.Terrorist, BombSiteEnum.A, SpawnTypeEnum.Normal));
            this._spawnPoints.Add(new SpawnPointEntity(new Vector(-439.763672f, 2293.371826f, - 119.998161f), new QAngle(3.124207f, - 18.375731f, 0.000000f), CsTeam.CounterTerrorist, BombSiteEnum.A, SpawnTypeEnum.Normal));
        }

        public SpawnPointEntity GetRandomSpawn(CsTeam team)
        {
            if(!this._spawnPoints.Any()) 
            {
                this.ReadSpawns();
            }

            var spawnChoices = this._spawnPoints.Where(x => x.Team == team && !x.SpawnIsInUse).ToList();

            if(!spawnChoices.Any())
            {
                throw new Exception("No spawns are available");
            }

            var random = new Random();


            return spawnChoices.ElementAt(random.Next(spawnChoices.Count - 1));
        }

        public void ReadSpawns()
        {
            var jsonSpawnPoints = File.ReadAllText(this.GetPath());

            if(string.IsNullOrEmpty(jsonSpawnPoints))
            {
                return;
            }

            this._spawnPoints = JsonSerializer.Deserialize<List<SpawnPointEntity>>(jsonSpawnPoints) ?? new List<SpawnPointEntity>();
        }

        public void WriteSpawns() 
        {
            File.WriteAllText(this.GetPath(), JsonSerializer.Serialize(this._spawnPoints));
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
    }
}
