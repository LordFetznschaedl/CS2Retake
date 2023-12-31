using CS2Retake.Managers.Base;
using CS2Retake.Managers.Interfaces;
using CS2Retake.Utils;

namespace CS2Retake.Managers;

public class TeamManager : BaseManager, ITeamManager
{
    private static TeamManager? _instance;

    private Dictionary<nint, PlayerStateEnum> _playerStateDict = new();

    private TeamManager()
    {
    }

    public static TeamManager Instance
    {
        get
        {
            if (_instance == null) _instance = new TeamManager();
            return _instance;
        }
    }

    public void AddQueuePlayers()
    {
        throw new NotImplementedException();
    }

    public void ScrambleTeams()
    {
        throw new NotImplementedException();
    }

    public void SwitchTeams()
    {
        throw new NotImplementedException();
    }

    public override void ResetForNextRound(bool completeReset = true)
    {
    }
}