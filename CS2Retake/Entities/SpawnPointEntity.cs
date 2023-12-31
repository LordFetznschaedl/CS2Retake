using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Utils;

namespace CS2Retake.Entities;

public class SpawnPointEntity
{
    public SpawnPointEntity(Vector position, QAngle qAngle, BombSiteEnum bombSite, CsTeam team,
        bool isInBombZone = false)
    {
        PositionX = position.X;
        PositionY = position.Y;
        PositionZ = position.Z;

        QAngleX = qAngle.X;
        QAngleY = qAngle.Y;
        QAngleZ = qAngle.Z;

        Team = team;
        BombSite = bombSite;
        IsInBombZone = isInBombZone;

        SpawnId = Guid.NewGuid();
    }

    public SpawnPointEntity(float positionX, float positionY, float positionZ, float qAngleX, float qAngleY,
        float qAngleZ, CsTeam team, BombSiteEnum bombSite)
    {
        PositionX = positionX;
        PositionY = positionY;
        PositionZ = positionZ;

        QAngleX = qAngleX;
        QAngleY = qAngleY;
        QAngleZ = qAngleZ;

        Team = team;
        BombSite = bombSite;
    }

    public SpawnPointEntity()
    {
    }

    public Guid SpawnId { get; set; } = Guid.Empty;
    public CsTeam Team { get; set; } = CsTeam.None;
    public BombSiteEnum BombSite { get; set; } = BombSiteEnum.Undefined;
    public bool IsInBombZone { get; set; }

    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }

    public float QAngleX { get; set; }
    public float QAngleY { get; set; }
    public float QAngleZ { get; set; }

    [JsonIgnore] public Vector Position => new(PositionX, PositionY, PositionZ);

    [JsonIgnore] public QAngle QAngle => new(QAngleX, QAngleY, QAngleZ);

    [JsonIgnore] public CCSPlayerController? SpawnUsedBy { get; set; } = null;
}