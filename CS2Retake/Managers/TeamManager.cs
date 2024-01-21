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

        public (int ctRatio, int tRatio) LatestRatio { get; private set; } = (0, 0);

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

        //TODO FIX: AddQueuePlayers does not rebalance properly
        public void AddQueuePlayers()
        {
            MessageUtils.LogDebug($"Methode: AddQueuePlayers");

            if (!FeatureConfig.EnableQueue)
            {
                MessageUtils.LogDebug($"Feature Queue is disabled!");

                return;
            }
               
            var playingPlayers = this.GetPlayingPlayers();
            var queuedPlayers = this.GetQueuedPlayers();

            this.DequeuePlayers();
            var ratio = this.GetPlayerRatio();
            var totalRatio = ratio.ctRatio + ratio.tRatio;
            var totalPlayers = this.GetPlayingPlayersCount();

            MessageUtils.LogDebug($"Ratio CT {ratio.ctRatio}, Ratio T {ratio.tRatio}, Total Ratio {totalRatio}, Total Players {totalPlayers}");

            if (totalPlayers != totalRatio)
            {
                MessageUtils.Log(LogLevel.Error, $"AddQueuePlayers - Playing players count [{totalPlayers}] doesnt match the total ratio [CT: {ratio.ctRatio}, T: {ratio.tRatio}]!");
                return;
            }

            var random = new Random();
            var playingCounterTerroristPlayers = playingPlayers.Where(x => x.TeamNum == (int)CsTeam.CounterTerrorist).OrderBy(x => random.Next()).ToList();
            var playingTerroristPlayers = playingPlayers.Where(x => x.TeamNum == (int)CsTeam.Terrorist).OrderBy(x => random.Next()).ToList();
            
            var ctsNeededInT = ratio.tRatio - playingTerroristPlayers.Count;

            queuedPlayers.ForEach(x => x.SwitchTeam(CsTeam.CounterTerrorist));

            if(ctsNeededInT > 0)
            {
                playingCounterTerroristPlayers.Take(ctsNeededInT).ToList().ForEach(x => x.SwitchTeam(CsTeam.Terrorist));
            }
            else if(ctsNeededInT < 0) 
            {
                playingTerroristPlayers.Take(ctsNeededInT * -1).ToList().ForEach(x => x.SwitchTeam(CsTeam.CounterTerrorist));
            }

        }

        public void ScrambleTeams()
        {
            MessageUtils.LogDebug($"Methode: ScrambleTeams");

            if (!FeatureConfig.EnableScramble)
            {
                MessageUtils.LogDebug($"Feature Scramble is disabled!");

                if (FeatureConfig.EnableQueue)
                {
                    this.AddQueuePlayers();
                }
                return;
            }

            this.DequeuePlayers();
            var ratio = this.GetPlayerRatio();
            var totalRatio = ratio.ctRatio + ratio.tRatio;
            var totalPlayers = this.GetPlayingPlayersCount();

            MessageUtils.LogDebug($"Ratio CT {ratio.ctRatio}, Ratio T {ratio.tRatio}, Total Ratio {totalRatio}, Total Players {totalPlayers}");

            if (totalPlayers != totalRatio) 
            {
                MessageUtils.Log(LogLevel.Error, $"ScrambleTeams - Playing players count [{totalPlayers}] doesnt match the total ratio [CT: {ratio.ctRatio}, T: {ratio.tRatio}]!");
                return;
            }

            var random = new Random();
            var playingPlayers = this.GetPlayingPlayers().OrderBy(x => random.Next()).ToList();

            playingPlayers.Take(ratio.ctRatio).ToList().ForEach(x => x.SwitchTeam(CsTeam.CounterTerrorist));
            playingPlayers.TakeLast(ratio.tRatio).ToList().ForEach(x => x.SwitchTeam(CsTeam.Terrorist));
        }

        public void SwitchTeams()
        {
            MessageUtils.LogDebug($"Methode: SwitchTeams");

            if (!FeatureConfig.EnableSwitchOnRoundWin)
            {
                MessageUtils.LogDebug($"Feature SwitchOnRoundWin is disabled!");

                if (FeatureConfig.EnableQueue)
                {
                    this.AddQueuePlayers();
                }
                return;
            }

            var playingPlayers = this.GetPlayingPlayers();
            var queuedPlayers = this.GetQueuedPlayers();

            this.DequeuePlayers();
            var ratio = this.GetPlayerRatio();
            var totalRatio = ratio.ctRatio + ratio.tRatio;
            var totalPlayers = this.GetPlayingPlayersCount();

            MessageUtils.LogDebug($"Ratio CT {ratio.ctRatio}, Ratio T {ratio.tRatio}, Total Ratio {totalRatio}, Total Players {totalPlayers}");

            if (totalPlayers != totalRatio)
            {
                MessageUtils.Log(LogLevel.Error, $"SwitchTeams - Playing players count [{totalPlayers}] doesnt match the total ratio [CT: {ratio.ctRatio}, T: {ratio.tRatio}]!");
                return;
            }

            var random = new Random();
            var playingCounterTerroristPlayers = playingPlayers.Where(x => x.TeamNum == (int)CsTeam.CounterTerrorist).OrderBy(x => random.Next()).ToList();
            var playingTerroristPlayers = playingPlayers.Where(x => x.TeamNum == (int)CsTeam.Terrorist).OrderBy(x => random.Next()).ToList();

            var ctsToSwitchToT =  playingCounterTerroristPlayers.Count >= ratio.tRatio ? ratio.tRatio : playingCounterTerroristPlayers.Count;
            var tsToSwitchToCT = playingTerroristPlayers.Count - (ratio.tRatio - ctsToSwitchToT);

            MessageUtils.LogDebug($"CT->T {ctsToSwitchToT}, T->CT {tsToSwitchToCT}");

            playingCounterTerroristPlayers.Take(ctsToSwitchToT).ToList().ForEach(x => x.SwitchTeam(CsTeam.Terrorist));
            playingTerroristPlayers.Take(tsToSwitchToCT).ToList().ForEach(x => x.SwitchTeam(CsTeam.CounterTerrorist));
            queuedPlayers.ForEach(x => x.SwitchTeam(CsTeam.CounterTerrorist));
        }

        public void OnTick()
        {
            var playerIds = this._playerStateDict.Where(x => x.Value == PlayerStateEnum.Connected).Select(x => x.Key).ToList();
            
            foreach(var playerId in playerIds)
            {
                var player = Utilities.GetPlayerFromUserid(playerId);

                if (player == null || !player.IsValid || player.UserId == null || !player.UserId.HasValue)
                {
                    continue;
                }

                if(player.TeamNum == (int)CsTeam.Terrorist || player.TeamNum == (int)CsTeam.CounterTerrorist)

                this.PlayerSwitchTeam(player, CsTeam.None, CsTeam.Spectator);
            }
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

            this.RemoveFromQueue(userId);

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
                MessageUtils.LogDebug($"Switch to spectator from playing or connected {userId}");
                this.UpdatePlayerStateDict(userId, PlayerStateEnum.Spectating);
            }
            //Place player into queue
            else if((currentState == PlayerStateEnum.Connected || currentState == PlayerStateEnum.Spectating) && (newTeam == CsTeam.Terrorist || newTeam == CsTeam.CounterTerrorist))
            {
                MessageUtils.LogDebug($"Switch to queue {userId}");
                this.UpdatePlayerStateDict(userId, PlayerStateEnum.Queue);
                this.UpdateQueue(userId);
            }
            //Remove player from queue when the player wants to switch to spectator
            else if(currentState == PlayerStateEnum.Queue && newTeam == CsTeam.Spectator)
            {
                MessageUtils.LogDebug($"Switch to spectator from queue {userId}");
                this.UpdatePlayerStateDict(userId, PlayerStateEnum.Spectating);
                this.RemoveFromQueue(userId);
            }

            
            player.ChangeTeam(CsTeam.Spectator);
            
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

        private void RemoveFromQueue(int userId) 
        {
            //Removing specific queued player from queue
            var queueList = this._playerQueue.ToList();
            this._playerQueue.Clear();

            foreach (var queueUserId in queueList)
            {
                if (queueUserId != userId)
                {
                    this._playerQueue.Enqueue(queueUserId);
                }
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

            this.LatestRatio = playerRatio;

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
