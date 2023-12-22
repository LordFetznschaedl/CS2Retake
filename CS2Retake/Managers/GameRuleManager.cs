using CounterStrikeSharp.API.Core;
using CS2Retake.Managers.Base;
using CS2Retake.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Managers
{
    public class GameRuleManager : BaseManager, IGameRuleManager
    {
        public bool IsWarmup()
        {
            throw new NotImplementedException();
        }

        public override void ResetForNextRound(bool completeReset = true)
        {
            
        }
    }
}
