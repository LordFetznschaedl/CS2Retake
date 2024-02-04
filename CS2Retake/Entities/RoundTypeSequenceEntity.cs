using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSZoneNet.Plugin.Utils.Enums;

namespace CS2Retake.Entities
{
    public class RoundTypeSequenceEntity
    {
        public RoundTypeEnum RoundType { get; set; } = RoundTypeEnum.Undefined;
        public int AmountOfRounds { get; set; } = 5;

        public RoundTypeSequenceEntity(RoundTypeEnum roundType, int amountOfRounds)
        {
            this.RoundType = roundType;
            this.AmountOfRounds = amountOfRounds;
        }

        public RoundTypeSequenceEntity(){}
    }
}
