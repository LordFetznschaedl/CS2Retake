using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Entity
{
    public class PlayerEntity
    {
        public ulong SteamId { get; set; }

        public PlayerEntity(ulong steamId)
        {
            this.SteamId = steamId;
        }
    }
}
