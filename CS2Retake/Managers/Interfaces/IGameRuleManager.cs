using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Managers.Interfaces
{
    public interface IGameRuleManager
    {
        public CCSGameRules? GameRules { get; set; }
        public bool IsWarmup { get; }
        public float WarmupEnd { get; }
        public bool BombPlanted { get; set; }
        public bool BombDefused { get; set; }
        public int TotalRoundsPlayed { get; }

        public void TerminateRound(RoundEndReason reason = RoundEndReason.RoundDraw);
    }
}
