using CounterStrikeSharp.API.Core;
using CS2Retake.Managers.Base;
using CS2Retake.Managers.Interfaces;
using CS2Retake.Utils;
using Microsoft.Extensions.Logging;
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

        private Dictionary<int, PlayerStateEnum> _playerStateDict = new Dictionary<int, PlayerStateEnum>();

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

        public void PlayerConnected(CCSPlayerController player)
        {
            if(player == null || !player.IsValid) 
            {
                return;
            }

            if(player.UserId == null || !player.UserId.HasValue)
            {
                return;
            }

            if(!this._playerStateDict.ContainsKey(player.UserId.Value)) 
            {
                this._playerStateDict.Add(player.UserId.Value, PlayerStateEnum.Connecting);
            }
            else 
            {
                this._playerStateDict[player.UserId.Value] = PlayerStateEnum.Connecting;
            }

            MessageUtils.Log(LogLevel.Information, $"Player {player.UserId.Value} is connecting...");
        }

        public void PlayerConnectedFull(CCSPlayerController player)
        {
            if (player == null || !player.IsValid)
            {
                return;
            }

            if (player.UserId == null || !player.UserId.HasValue)
            {
                return;
            }

            if (!this._playerStateDict.ContainsKey(player.UserId.Value))
            {
                this._playerStateDict.Add(player.UserId.Value, PlayerStateEnum.Connected);
            }
            else
            {
                this._playerStateDict[player.UserId.Value] = PlayerStateEnum.Connected;
            }

            MessageUtils.Log(LogLevel.Information, $"Player {player.UserId.Value} is now connected.");
        }

        public void PlayerDisconnected(CCSPlayerController player)
        {
            if (player == null || !player.IsValid)
            {
                return;
            }

            if (player.UserId == null || !player.UserId.HasValue)
            {
                return;
            }

            if (!this._playerStateDict.ContainsKey(player.UserId.Value))
            {
                this._playerStateDict.Remove(player.UserId.Value);
            }

            MessageUtils.Log(LogLevel.Information, $"Player {player.UserId.Value} is now disconnected.");
        }

        public override void ResetForNextRound(bool completeReset = true)
        {

        }
    }
}
