using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Entities
{
    public class ChanceEntity
    {
        public string WeaponName { get; set; }
        public string WeaponString { get; set; }
        public CsTeam Team { get; set; } = CsTeam.None;

        public List<int> Chances { get; set; }

        public int Limit { get; set; } = -1;

        public ChanceEntity() { }

        public ChanceEntity(string weaponName, string weaponString, List<int> chances, CsTeam team = CsTeam.None)
        {
            this.WeaponName = weaponName;
            this.WeaponString = weaponString;
            this.Chances = chances;

            this.Team = team;
        }

    }
}
