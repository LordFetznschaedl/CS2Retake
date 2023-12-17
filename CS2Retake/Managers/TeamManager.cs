using CS2Retake.Managers.Base;
using CS2Retake.Managers.Interfaces;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Managers
{
    public class TeamManager : BaseManager, ITeamManager
    {
        private static TeamManager? _instance = null;

        private Dictionary<ulong, PlayerStateEnum> _playerStateDict = new Dictionary<ulong, PlayerStateEnum>();

        public static TeamManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TeamManager();
                }
                return _instance;
            }
        }

        private TeamManager() { }

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
}
