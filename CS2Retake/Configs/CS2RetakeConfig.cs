using CounterStrikeSharp.API.Core;
using CS2Retake.Entities;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CSZoneNet.Plugin.Utils.Enums;

namespace CS2Retake.Configs
{
    public class CS2RetakeConfig : BasePluginConfig
    {
        public PlantTypeEnum PlantType { get; set; } = PlantTypeEnum.AutoPlant;
        public RoundTypeModeEnum RoundTypeMode { get; set; } = RoundTypeModeEnum.Sequence;

        public List<RoundTypeSequenceEntity> RoundTypeSequence { get; set; } = new List<RoundTypeSequenceEntity>() 
        { 
            new RoundTypeSequenceEntity(RoundTypeEnum.Pistol, 5),
            new RoundTypeSequenceEntity(RoundTypeEnum.Mid, 3),
            new RoundTypeSequenceEntity(RoundTypeEnum.FullBuy, -1),  
        };

        public RoundTypeEnum RoundTypeSpecific { get; set; } = RoundTypeEnum.FullBuy;

        public float SecondsUntilBombPlantedCheck { get; set; } = 5.0f;

        public bool SpotAnnouncerEnabled { get; set; } = true;

        public bool EnableQueue { get; set; } = true;
        public bool EnableScramble { get; set; } = true;
        public bool EnableSwitchOnRoundWin { get; set; } = true;

        public int ScrambleAfterSubsequentTerroristRoundWins { get; set; } = 5;

        public int MaxPlayers { get; set; } = 10;
        public float TeamBalanceRatio { get; set; } = 0.499f;

        public bool EnableThankYouMessage { get; set; } = true;




        public bool EnableDebug { get; set; } = false;
        public CS2RetakeConfig() {
            this.Version = 4;
        }
    }
}
