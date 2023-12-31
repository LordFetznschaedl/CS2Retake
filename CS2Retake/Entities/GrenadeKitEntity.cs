using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Utils;

namespace CS2Retake.Entities;

public class GrenadeKitEntity
{
    public string KitName { get; set; }

    public List<GrenadeEnum> GrenadeList { get; set; } = new();

    //CsTeam.None = Both Teams
    public CsTeam Team { get; set; } = CsTeam.None;

    public RoundTypeEnum RoundType { get; set; } = RoundTypeEnum.Undefined;
    public int KitLimit { get; set; } = -1;


    [JsonIgnore] public int KitUsedAmount { get; set; } = 0;

    [JsonIgnore] public bool KitLimitReached => KitLimit == 0 || KitLimit == KitUsedAmount;
}