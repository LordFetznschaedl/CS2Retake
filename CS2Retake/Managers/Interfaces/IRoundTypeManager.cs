using CounterStrikeSharp.API.Modules.Entities.Constants;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Managers.Interfaces
{
    public interface IRoundTypeManager
    {
        public RoundTypeEnum RoundType { get; }
        public void HandleRoundType();
    }
}
