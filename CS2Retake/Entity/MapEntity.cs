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

            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1485.635986f, 1799.516846f, -9.545544f), new QAngle(1.386139f, 134.525223f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-439.763672f, 2293.371826f, -119.998161f), new QAngle(3.124207f, -18.375731f, 0.000000f), BombSiteEnum.A, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-796.105164f, 2106.356689f, -108.181061f), new QAngle(-1.583791f, 4.306473f, 0.000000f), BombSiteEnum.A, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-596.710510f, 2397.936768f, -98.976906f), new QAngle(-2.903791f, -76.411491f, 0.000000f), BombSiteEnum.A, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1054.251831f, 1388.899536f, -112.143394f), new QAngle(-5.983798f, 3.471123f, 0.000000f), BombSiteEnum.A, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-252.275925f, 830.058960f, 32.031223f), new QAngle(3.102170f, 74.575157f, 0.000000f), BombSiteEnum.A, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-40.217503f, 324.087860f, 0.399642f), new QAngle(-0.967835f, -8.892918f, 0.000000f), BombSiteEnum.A, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(274.626312f, -153.398819f, -0.838978f), new QAngle(-0.593832f, 51.057110f, 0.000000f), BombSiteEnum.A, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1679.640503f, 2404.947510f, 7.239616f), new QAngle(5.742341f, -21.358704f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1687.914917f, 2786.731445f, 18.606092f), new QAngle(1.210338f, -56.558819f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.NeverWithBomb));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1588.554932f, 2005.770874f, -2.777470f), new QAngle(4.092337f, 30.780836f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1848.645020f, 1972.090820f, -2.131035f), new QAngle(2.156341f, 20.528877f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-2043.669922f, 2585.278320f, 33.407745f), new QAngle(-0.197655f, -84.873642f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(266.663605f, 2343.530273f, -119.982681f), new QAngle(-0.285863f, -150.125427f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-7.317451f, 2050.833984f, -123.852913f), new QAngle(-4.399863f, 173.288071f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-284.019714f, 2038.630005f, -123.948372f), new QAngle(-5.873852f, 168.689453f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-373.510162f, 1753.405396f, -124.746796f), new QAngle(-5.807847f, 121.477371f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-291.868286f, 964.811401f, -37.290321f), new QAngle(7.612102f, 117.406761f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-776.527527f, 1464.194824f, -111.968773f), new QAngle(0.000097f, -159.323166f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1414.269897f, -447.026794f, 128.996399f), new QAngle(3.014117f, 101.178787f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-21.254484f, 1524.878418f, -0.926502f), new QAngle(6.908079f, -171.260849f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1987.347656f, 2878.885498f, 30.486708f), new QAngle(-6.098335f, -17.200668f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-398.861359f, 1471.278198f, -126.425545f), new QAngle(-2.327384f, -174.727463f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-506.651733f, 1781.237793f, -120.306938f), new QAngle(-4.663777f, -60.267941f, 0.000000f), BombSiteEnum.A, CsTeam.CounterTerrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1765.031250f, 2497.615479f, 31.877214f), new QAngle(12.970195f, -43.465748f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1576.354492f, 2854.372070f, 9.996930f), new QAngle(7.611034f, -93.016663f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.NeverWithBomb));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1366.031250f, 2410.031250f, 3.734168f), new QAngle(21.717495f, -127.728264f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1287.251221f, 1957.706299f, 1.031250f), new QAngle(-0.951298f, -90.559593f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1330.236084f, 2888.478760f, 127.723854f), new QAngle(9.089360f, -88.418839f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1051.001831f, 3059.968750f, 131.166458f), new QAngle(11.537972f, -75.190254f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(896.486328f, 2649.090576f, 95.586090f), new QAngle(54.349720f, -151.312592f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-78.936737f, 2278.990967f, -127.294754f), new QAngle(5.501089f, -177.908478f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.OnlyWithBomb));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-284.401428f, 1415.865356f, -27.968744f), new QAngle(6.440482f, 122.653305f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.OnlyWithBomb));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1113.855347f, 1110.606201f, -29.921255f), new QAngle(3.514474f, 172.364594f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.OnlyWithBomb));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1670.240234f, 793.639648f, 30.497513f), new QAngle(3.683904f, 98.689842f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.OnlyWithBomb));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1554.546631f, 385.539520f, 0.574780f), new QAngle(-2.722472f, 104.265594f, 0.000000f), BombSiteEnum.B, CsTeam.CounterTerrorist, SpawnTypeEnum.OnlyWithBomb));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-211.968719f, 1179.521484f, -0.456734f), new QAngle(1.466331f, 42.409359f, 0.000000f), BombSiteEnum.A, CsTeam.CounterTerrorist, SpawnTypeEnum.OnlyWithBomb));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(600.362488f, 170.745193f, -0.378741f), new QAngle(-0.212230f, 76.644356f, 0.000000f), BombSiteEnum.A, CsTeam.CounterTerrorist, SpawnTypeEnum.OnlyWithBomb));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-413.015015f, 1489.461914f, -126.426064f), new QAngle(0.711776f, 92.183464f, 0.000000f), BombSiteEnum.A, CsTeam.CounterTerrorist, SpawnTypeEnum.OnlyWithBomb));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1595.582275f, 1804.091187f, 1.031250f), new QAngle(15.246216f, 166.052765f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1448.110107f, 2526.778320f, 52.682800f), new QAngle(23.977888f, -137.384933f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1532.901001f, 2964.468018f, 129.031250f), new QAngle(13.028666f, -143.574203f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(985.566589f, 2517.046143f, 96.226486f), new QAngle(0.030845f, -136.713562f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1089.616455f, 2492.709717f, 96.031250f), new QAngle(2.433239f, -71.048347f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1235.968750f, 2460.968750f, 97.832726f), new QAngle(3.449638f, -77.516235f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1235.999878f, 2348.031250f, 99.695267f), new QAngle(4.219637f, -75.852875f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1235.968750f, 2560.031250f, 99.378532f), new QAngle(-1.139531f, 110.896896f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(1050.102783f, 2586.931152f, 95.995239f), new QAngle(-1.478332f, -95.518692f, 0.000000f), BombSiteEnum.A, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1362.031250f, 2755.000244f, 18.076164f), new QAngle(0.831642f, -154.322739f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1430.031250f, 2676.278564f, 16.376022f), new QAngle(0.369664f, -159.589737f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1684.031250f, 2540.894043f, 7.525145f), new QAngle(0.739260f, -65.880714f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.Normal));
            //this.SpawnPoints.Add(new SpawnPointEntity(new Vector(-1366.031250f, 2565.995117f, 4.644805f), new QAngle(0.277256f, -118.102386f, 0.000000f), BombSiteEnum.B, CsTeam.Terrorist, SpawnTypeEnum.Normal));
        }

        private SpawnPointEntity? GetRandomSpawn(CsTeam team, BombSiteEnum bombSite)
        {
            if(!this.SpawnPoints.Any()) 
            {
                this.ReadSpawns();
            }

            var spawnChoices = this.SpawnPoints.Where(x => x.Team == team && !x.SpawnIsInUse && x.BombSite == bombSite).ToList();

            if(!spawnChoices.Any())
            {
                return null;
            }

            var random = new Random();


            return spawnChoices.ElementAt(random.Next(spawnChoices.Count - 1));
        }

        public void TeleportPlayerToSpawn(CCSPlayerController player, BombSiteEnum bombSite, int? spawnIndex = null)
        {
            SpawnPointEntity? spawn;
            if (!spawnIndex.HasValue)
            {
                spawn = this.GetRandomSpawn((CsTeam)player.TeamNum, bombSite);
            }
            else if (spawnIndex.Value > (this.SpawnPoints.Count - 1) || spawnIndex.Value < 0)
            {
                return;
            }
            else
            {
                spawn = this.SpawnPoints[spawnIndex.Value];
            }

            if (spawn == null)
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
