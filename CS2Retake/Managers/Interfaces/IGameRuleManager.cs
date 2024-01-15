using CounterStrikeSharp.API.Core;
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


    }
}
