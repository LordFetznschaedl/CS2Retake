using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CS2Retake.Entity
{
    public class SpawnPointEntity
    {
        public Guid SpawnId { get; set; } = Guid.Empty;
        public CsTeam Team { get; set; } = CsTeam.None;
        public BombSiteEnum BombSite { get; set; } = BombSiteEnum.Undefined;
        public bool IsInBombZone { get; set; } = false;

        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }

        public float QAngleX { get; set; }
        public float QAngleY { get; set; }
        public float QAngleZ { get; set; }

        [JsonIgnore]
        public Vector Position => new Vector(this.PositionX, this.PositionY, this.PositionZ);
        [JsonIgnore]
        public QAngle QAngle => new QAngle(this.QAngleX, this.QAngleY, this.QAngleZ);

        [JsonIgnore]
        public CCSPlayerController? SpawnUsedBy { get; set; } = null;

        public SpawnPointEntity(Vector position, QAngle qAngle, BombSiteEnum bombSite, CsTeam team, bool isInBombZone = false)
        {
            this.PositionX = position.X;
            this.PositionY = position.Y;
            this.PositionZ = position.Z;

            this.QAngleX = qAngle.X;
            this.QAngleY = qAngle.Y;
            this.QAngleZ = qAngle.Z;

            this.Team = team;
            this.BombSite = bombSite;
            this.IsInBombZone = isInBombZone;

            this.SpawnId= Guid.NewGuid();
        }

        public SpawnPointEntity(float positionX, float positionY, float positionZ, float qAngleX, float qAngleY, float qAngleZ, CsTeam team, BombSiteEnum bombSite)
        {
            this.PositionX = positionX;
            this.PositionY = positionY;
            this.PositionZ = positionZ;

            this.QAngleX = qAngleX;
            this.QAngleY = qAngleY;
            this.QAngleZ = qAngleZ;

            this.Team = team;
            this.BombSite = bombSite;
        }

        public SpawnPointEntity()
        {
        }
    }
}
