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
        public SpawnTypeEnum SpawnType { get; set; } = SpawnTypeEnum.Undefined;
        public CsTeam Team { get; set; } = CsTeam.None;
        public BombSiteEnum BombSite { get; set; } = BombSiteEnum.Undefined;

        
        public float VectorX { get; set; }
        public float VectorY { get; set; }
        public float VectorZ { get; set; }

        public float QAngleX { get; set; }
        public float QAngleY { get; set; }
        public float QAngleZ { get; set; }

        [JsonIgnore]
        public Vector Vector => new Vector(this.VectorX, this.VectorY, this.VectorZ);
        [JsonIgnore]
        public QAngle QAngle => new QAngle(this.QAngleX, this.QAngleY, this.QAngleZ);

        [JsonIgnore]
        public bool SpawnIsInUse { get; set; } = false;


        public SpawnPointEntity(Vector vector, QAngle qAngle, CsTeam team, BombSiteEnum bombSite, SpawnTypeEnum spawnType = SpawnTypeEnum.Normal) 
        {
            this.VectorX = vector.X;
            this.VectorY = vector.Y;
            this.VectorZ = vector.Z;

            this.QAngleX = qAngle.X;
            this.QAngleY = qAngle.Y;
            this.QAngleZ = qAngle.Z;

            this.Team = team;
            this.BombSite = bombSite;
            this.SpawnType = spawnType;
        }

        public SpawnPointEntity(float vectorX, float vectorY, float vectorZ, float qAngleX, float qAngleY, float qAngleZ, CsTeam team, BombSiteEnum bombSite, SpawnTypeEnum spawnType = SpawnTypeEnum.Normal)
        {
            this.VectorX = vectorX;
            this.VectorY = vectorY;
            this.VectorZ = vectorZ;

            this.QAngleX = qAngleX;
            this.QAngleY = qAngleY;
            this.QAngleZ = qAngleZ;

            this.Team = team;
            this.BombSite = bombSite;
            this.SpawnType = spawnType;
        }
    }
}
