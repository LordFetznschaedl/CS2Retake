using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
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

        public void HotReload();


        public void PlayerConnected(CCSPlayerController player);
        public void PlayerConnectedFull(CCSPlayerController player);
        public void PlayerDisconnected(CCSPlayerController player);

        public void PlayerSwitchTeam(CCSPlayerController player, CsTeam previousTeam, CsTeam newTeam);
    }
}
