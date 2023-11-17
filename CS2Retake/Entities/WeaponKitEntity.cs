using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CS2Retake.Entities
{
    public class WeaponKitEntity
    {
        public string KitName { get; set; }
        public string PrimaryWeapon { get; set; }
        public string SecondaryWeapon { get; set; }
        public bool DefuseKit { get; set; } = true;
        public KevlarEnum Kevlar { get; set; } = KevlarEnum.KevlarHelmet;
        public CsTeam Team { get; set; } = CsTeam.None;
        public RoundTypeEnum RoundType { get; set; } = RoundTypeEnum.Undefined;
        public int KitLimit { get; set; } = -1;

        [JsonIgnore]
        public int KitUsedAmount { get; set; } = 0;

        [JsonIgnore]
        public bool KitLimitReached => this.KitLimit == 0 || this.KitLimit <= this.KitUsedAmount;
    }
}
