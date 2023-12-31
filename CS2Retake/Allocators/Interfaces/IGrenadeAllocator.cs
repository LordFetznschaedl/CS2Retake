using CounterStrikeSharp.API.Core;
using CS2Retake.Allocators.Interfaces.Base;
using CS2Retake.Utils;

namespace CS2Retake.Allocators.Interfaces;

public interface IGrenadeAllocator : IBaseAllocator
{
    public List<GrenadeEnum> Allocate(CCSPlayerController player, RoundTypeEnum roundType = RoundTypeEnum.Undefined);
}