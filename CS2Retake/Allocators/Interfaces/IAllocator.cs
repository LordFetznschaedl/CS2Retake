using CounterStrikeSharp.API.Core;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Interfaces
{
    public interface IAllocator
    {
        public (string primaryWeapon, string secondaryWeapon, KevlarEnum kevlar, bool kit) Allocate(CCSPlayerController player, RoundTypeEnum roundType = RoundTypeEnum.Undefined);

        public void ResetForNextRound();
    }
}
