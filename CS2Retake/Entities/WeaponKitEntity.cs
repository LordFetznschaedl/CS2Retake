using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Entities
{
    public class WeaponKitEntity
    {
        public string KitName { get; set; }
        public string PrimaryWeapon { get; set; }
        public string SecondaryWeapon { get; set; }
        public bool DefuseKit { get; set; } = true;
        public bool Kevlar { get; set; } = true;
        public CsTeam Team { get; set; } = CsTeam.None;
        public RoundTypeEnum RoundType { get; set; } = RoundTypeEnum.Undefined;
        public int Tickets { get; set; } = 1;
    }
}
