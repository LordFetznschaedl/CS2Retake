using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
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

        public void PlayerSwitchTeam(CCSPlayerController player, CsTeam previousTeam, CsTeam newTeam)
        {
            if (player == null || !player.IsValid)
            {
                return;
            }

            if (player.UserId == null || !player.UserId.HasValue)
            {
                return;
            }

            var userId = player.UserId.Value;
            var currentState = this.GetCurrentPlayerState(userId);

            //Allow switch to spectator
            if((currentState == PlayerStateEnum.Connected || currentState == PlayerStateEnum.Playing || currentState == PlayerStateEnum.Queue) && newTeam == CsTeam.Spectator)
            {
                this.UpdatePlayerStateDict(userId, PlayerStateEnum.Spectating);
                player.SwitchTeam(CsTeam.Spectator);
            }
            //Place player into queue
            else if(currentState == PlayerStateEnum.Connected && (newTeam == CsTeam.Terrorist || newTeam == CsTeam.CounterTerrorist))
            {
                this.UpdatePlayerStateDict(userId, PlayerStateEnum.Queue);
                player.SwitchTeam(CsTeam.Spectator);
            }
        }

        public override void ResetForNextRound(bool completeReset = true)
        {

        }

        private PlayerStateEnum GetCurrentPlayerState(CCSPlayerController player)
        {
            if (player == null || !player.IsValid)
            {
                return PlayerStateEnum.None;
            }

            if (player.UserId == null || !player.UserId.HasValue)
            {
                return PlayerStateEnum.None;
            }

            return this.GetCurrentPlayerState(player.UserId.Value);
        }

        private PlayerStateEnum GetCurrentPlayerState(int userId)
        {
            if(!this._playerStateDict.TryGetValue(userId, out PlayerStateEnum state))
            {
                return PlayerStateEnum.None;
            }

            return state;
        }
        
        private void UpdatePlayerStateDict(int userId, PlayerStateEnum state)
        {
            if (this._playerStateDict.ContainsKey(userId))
            {
                this._playerStateDict[userId] = state;
            }
        }

        private List<CCSPlayerController> GetQueuedPlayers()
        {
            var playerControllerList = new List<CCSPlayerController>();

            var userIdList = this._playerStateDict.Where(x => x.Value == PlayerStateEnum.Queue).Select(x => x.Key).ToList();
           
            foreach ( var userId in userIdList) 
            {
                playerControllerList.Add(Utilities.GetPlayerFromUserid(userId));
            }

            return playerControllerList;
        }
    }
}
