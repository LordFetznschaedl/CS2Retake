using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Configs;
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
        private Queue<int> _playerQueue = new Queue<int>();

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
            var playingPlayers = this.GetPlayingPlayers();
            var queuedPlayers = this.GetQueuedPlayers();

            this.DequeuePlayers();
            var ratio = this.GetPlayerRatio();
            var totalRatio = ratio.ctRatio + ratio.tRatio;

            var playingCounterTerroristPlayers = playingPlayers.Where(x => x.TeamNum == (int)CsTeam.CounterTerrorist).ToList();
            var playingTerroristPlayers = playingPlayers.Where(x => x.TeamNum == (int)CsTeam.Terrorist).ToList();
            
            if(ratio.tRatio < playingTerroristPlayers.Count) 
            {
                
            }

        }

        public void ScrambleTeams()
        {
            this.DequeuePlayers();
            var ratio = this.GetPlayerRatio();

            var random = new Random();
            var playingPlayers = this.GetPlayingPlayers().OrderBy(x => random.Next()).ToList();

            if(playingPlayers.Count != (ratio.ctRatio + ratio.tRatio)) 
            {
                MessageUtils.Log(LogLevel.Error, $"Playing players count [{playingPlayers.Count}] doesnt match the total ratio [CT: {ratio.ctRatio}, T: {ratio.tRatio}]!");
            }

            playingPlayers.Take(ratio.ctRatio).ToList().ForEach(x => x.SwitchTeam(CsTeam.CounterTerrorist));
            playingPlayers.TakeLast(ratio.tRatio).ToList().ForEach(x => x.SwitchTeam(CsTeam.Terrorist));
        }

        public void SwitchTeams()
        {
            this.DequeuePlayers();
            var ratio = this.GetPlayerRatio();


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

            var userId = player.UserId.Value;

            if (this._playerStateDict.ContainsKey(userId))
            {
                this._playerStateDict.Remove(userId);
            }

            //Removing specific queued player from queue
            var queueList = this._playerQueue.ToList();
            this._playerQueue.Clear();

            foreach(var queueUserId in queueList) 
            {
                if(queueUserId != userId)
                {
                    this._playerQueue.Enqueue(queueUserId);
                }
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
            if((currentState == PlayerStateEnum.Connected || currentState == PlayerStateEnum.Playing) && newTeam == CsTeam.Spectator)
            {
                this.UpdatePlayerStateDict(userId, PlayerStateEnum.Spectating);
                player.SwitchTeam(CsTeam.Spectator);
            }
            //Place player into queue
            else if(currentState == PlayerStateEnum.Connected && (newTeam == CsTeam.Terrorist || newTeam == CsTeam.CounterTerrorist))
            {
                this.UpdatePlayerStateDict(userId, PlayerStateEnum.Queue);
                this.UpdateQueue(userId);
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
            else
            {
                this._playerStateDict.Add(userId, state);
            }
        }

        private void UpdateQueue(int userId)
        {
            if(!this._playerQueue.Contains(userId))
            {
                this._playerQueue.Enqueue(userId);
            }
        }

        private List<CCSPlayerController> GetQueuedPlayers() => this.GetPlayersByState(PlayerStateEnum.Queue);
        private int GetQueuedPlayersCount() => this.GetPlayersCountByState(PlayerStateEnum.Queue);

        private List<CCSPlayerController> GetPlayingPlayers() => this.GetPlayersByState(PlayerStateEnum.Playing);
        private int GetPlayingPlayersCount() => this.GetPlayersCountByState(PlayerStateEnum.Playing);

        private List<CCSPlayerController> GetPlayersByState(PlayerStateEnum playerState)
        {
            var playerControllerList = new List<CCSPlayerController>();

            var userIdList = this._playerStateDict.Where(x => x.Value == playerState).Select(x => x.Key).ToList();

            foreach (var userId in userIdList)
            {
                playerControllerList.Add(Utilities.GetPlayerFromUserid(userId));
            }

            return playerControllerList;
        }

        private int GetPlayersCountByState(PlayerStateEnum playerState)
        {
            return this._playerStateDict.Where(x => x.Value == playerState).Count();
        }

        private (int ctRatio, int tRatio) GetPlayerRatio()
        {
            (int ctRatio, int tRatio) playerRatio = (0,0);

            var playingPlayers = this.GetQueuedPlayersCount();
            var queuedPlayers = this.GetPlayingPlayersCount();

            var totalPlayers = playingPlayers + queuedPlayers;

            if(totalPlayers > RuntimeConfig.MaxPlayers)
            {
                totalPlayers = RuntimeConfig.MaxPlayers;
            }

            var tRatio = (int)Math.Round(totalPlayers * RuntimeConfig.TeamBalanceRatio);

            if(totalPlayers == 1 && tRatio == 0)
            {
                playerRatio.ctRatio = 0;
                playerRatio.tRatio = 1;
            }
            else
            {
                playerRatio.ctRatio = totalPlayers - tRatio;
                playerRatio.tRatio = tRatio;
            }

            return playerRatio;
        }

        private void DequeuePlayers()
        {
            var playersToDequeueCount = RuntimeConfig.MaxPlayers - this.GetPlayingPlayersCount();

            for(int i = 0; i < playersToDequeueCount; i++) 
            {
                if(!this._playerQueue.TryDequeue(out int userId))
                {
                    continue;
                }

                this.UpdatePlayerStateDict(userId, PlayerStateEnum.Playing);
            }
        }
    }
}
