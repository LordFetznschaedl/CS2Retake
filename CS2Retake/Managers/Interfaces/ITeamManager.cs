using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Managers.Interfaces
{
    public interface ITeamManager
    {
        public void ScrambleTeams();
        public void AddQueuePlayers();
        public void SwitchTeams();
    }
}
