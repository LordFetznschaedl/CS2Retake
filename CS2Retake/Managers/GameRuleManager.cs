using CS2Retake.Managers.Base;
using CS2Retake.Managers.Interfaces;

namespace CS2Retake.Managers;

public class GameRuleManager : BaseManager, IGameRuleManager
{
    public override void ResetForNextRound(bool completeReset = true)
    {
    }
}