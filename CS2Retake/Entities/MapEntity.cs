using System.Text.Json;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Utils;

namespace CS2Retake.Entities;

public class MapEntity
{
    public MapEntity(string mapName, string moduleDirectoy)
    {
        MapName = mapName;
        _moduleDirectory = moduleDirectoy;
    }

    public string MapName { get; set; }

    public List<SpawnPointEntity> SpawnPoints { get; set; } = new();
    public Queue<ulong> PlayerQueue { get; set; } = new();

    private string _moduleDirectory { get; }

    public SpawnPointEntity? GetRandomSpawn(CsTeam team, BombSiteEnum bombSite, bool hasToBeInBombZone)
    {
        if (!SpawnPoints.Any()) LoadSpawns();

        var spawnChoices = SpawnPoints.Where(x => x.Team == team && x.SpawnUsedBy == null && x.BombSite == bombSite)
            .ToList();

        if (!spawnChoices.Any())
        {
            Log("No spawn choices found.");
            return null;
        }

        if (team == CsTeam.Terrorist && hasToBeInBombZone)
        {
            spawnChoices = spawnChoices.Where(x => x.IsInBombZone).ToList();

            if (!spawnChoices.Any())
            {
                Log("hasToBeInBombZone has not found any spawns.");
                return null;
            }
        }

        var random = new Random();
        return spawnChoices.OrderBy(x => random.Next()).FirstOrDefault();
    }


    public void LoadSpawns()
    {
        var path = GetPath();

        if (!File.Exists(path)) return;

        var jsonSpawnPoints = File.ReadAllText(path);

        if (string.IsNullOrEmpty(jsonSpawnPoints)) return;

        SpawnPoints = JsonSerializer.Deserialize<List<SpawnPointEntity>>(jsonSpawnPoints) ??
                      new List<SpawnPointEntity>();
    }

    public void SaveSpawns()
    {
        SpawnPoints.Where(spawnPoint => spawnPoint.SpawnId == Guid.Empty).ToList()
            .ForEach(spawnPoint => spawnPoint.SpawnId = Guid.NewGuid());

        File.WriteAllText(GetPath(), JsonSerializer.Serialize(SpawnPoints));
    }

    private string GetPath()
    {
        var path = Path.Join(_moduleDirectory, "spawns");

        if (!Path.Exists(path)) Directory.CreateDirectory(path);

        path = Path.Join(path, $"{MapName}.json");

        return path;
    }

    public void ResetSpawnInUse()
    {
        SpawnPoints.ForEach(spawn => spawn.SpawnUsedBy = null);
    }

    private void Log(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{MessageUtils.ModuleName}:{GetType().Name}] {message}");
        Console.ResetColor();
    }
}