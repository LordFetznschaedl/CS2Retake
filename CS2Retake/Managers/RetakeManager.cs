using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Modules.Utils;
using System.Security.Cryptography;
using CS2Retake.Entities;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Timers;
using System.Timers;
using CS2Retake.Utils;
using CS2Retake.Managers.Base;

namespace CS2Retake.Managers
{
    public class RetakeManager : BaseManager
    {
        private static RetakeManager? _instance = null;
        public bool BombHasBeenPlanted { get; set; } = false;

        private CCSPlayerController _planterPlayerController;

        public CCSGameRules? GameRules { get; set; } = null;


        public static RetakeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RetakeManager();
                }
                return _instance;
            }
        }

        private RetakeManager() { }

        public void ScrambleTeams()
        {
            var nonSpectatingValidPlayers = this.GetPlayerControllers().Where(x => x.IsValid && (x.TeamNum == (int)CsTeam.Terrorist || x.TeamNum == (int)CsTeam.CounterTerrorist)).ToList();

            if(!nonSpectatingValidPlayers.Any()) 
            {
                this.Log($"No valid non spectating players have been found!");
                return;
            }

            var random = new Random();
            nonSpectatingValidPlayers = nonSpectatingValidPlayers.OrderBy(x => random.Next()).ToList();

            for(int i = 0; i < nonSpectatingValidPlayers.Count; i++) 
            {
                nonSpectatingValidPlayers[i].SwitchTeam(i % 2 == 0 ? CsTeam.CounterTerrorist : CsTeam.Terrorist);
            }
        }

        public void SwitchTeams()
        {

        }

        public void GiveBombToPlayerRandomPlayerInBombZone()
        {

            var random = new Random();
            var plantSpawn = MapManager.Instance.CurrentMap.SpawnPoints.Where(spawn => spawn.SpawnUsedBy != null && spawn.IsInBombZone).OrderBy(x => random.Next()).FirstOrDefault();


            if(plantSpawn == null)
            {
                this.Log($"No valid plant spawn found!");
                return;
            }

            if(plantSpawn.SpawnUsedBy == null)
            {
                this.Log($"Spawn is not used by any player");
                return;
            }

            this._planterPlayerController = plantSpawn.SpawnUsedBy;

            if(this._planterPlayerController == null)
            {
                this.Log($"Player that uses the valid plant spawn is null");
                return;
            }

            var seconds = 5;

            this._planterPlayerController.GiveNamedItem("weapon_c4");

            MessageUtils.PrintToPlayerOrServer($"YOU HAVE {ChatColors.Darkred}{seconds}{ChatColors.White} SECONDS TO PLANT THE BOMB!", this._planterPlayerController);


            _  = new CounterStrikeSharp.API.Modules.Timers.Timer(seconds, this.HasBombBeenPlantedCallback);
            //c4.BombPlacedAnimation = false;



            //var plantedBomb = this.FindPlantedBomb();

            //if(plantedBomb == null)
            //{
            //    this.Log($"No planted bomb was found!");
            //    return;
            //}

            //var playerPawn = player.PlayerPawn.Value;

            //if(playerPawn == null)
            //{
            //    this.Log($"Player pawn is null");
            //    return;
            //}
            //if(playerPawn.AbsRotation == null)
            //{
            //    this.Log($"Player pawn rotation is null");
            //    return;
            //}
            //if(playerPawn.AbsOrigin == null)
            //{
            //    this.Log($"Player pawn position is null");
            //    return;
            //}


            //plantedBomb.Teleport(playerPawn.AbsOrigin, playerPawn.AbsRotation, new Vector(0f, 0f, 0f));

            //this.ModifyGameRulesBombPlanted(true);

            //plantedBomb.BombTicking = true;
            ////plantedBomb.BombSite = 168 + (int)MapManager.Instance.BombSite;


            //var bombTarget = this.GetBombTarget();

            //if (bombTarget == null)
            //{
            //    return;
            //}

            //bombTarget.BombPlantedHere = true;



            //var bombPlantedEventPtr = NativeAPI.CreateEvent("bomb_planted", false);
            //NativeAPI.SetEventPlayerController(bombPlantedEventPtr, "userid", player.Handle);
            //NativeAPI.SetEventInt(bombPlantedEventPtr, "site", 0);
            ////NativeAPI.SetEventEntity(bombPlantedEventPtr, "userid_pawn", player.PlayerPawn.Value.Handle);
            //NativeAPI.FireEvent(bombPlantedEventPtr, false);


        }

        public void FastPlantBomb()
        {
            var c4list = Utilities.FindAllEntitiesByDesignerName<CC4>("weapon_c4");


            if (!c4list.Any())
            {
                return;
            }

            var c4 = c4list.FirstOrDefault();
            if (c4 == null)
            {
                return;
            }

            c4.BombPlacedAnimation = false;
            c4.ArmedTime = 0f;

            //c4.IsPlantingViaUse = true;
            //c4.StartedArming = true;
            //c4.BombPlanted = true;

        }

        private void HasBombBeenPlantedCallback()
        {
            if(!this.BombHasBeenPlanted)
            {
                Server.PrintToChatAll($"{MessageUtils.PluginPrefix} Player {ChatColors.Darkred}{this._planterPlayerController.PlayerName}{ChatColors.White} failed to plant the bomb in time. Counter-Terrorists win this round.");

                var terroristPlayerList = this.GetPlayerControllers().Where(x => x.IsValid && x.TeamNum == (int)CsTeam.Terrorist).ToList();
                terroristPlayerList.ForEach(x => x.PlayerPawn.Value.CommitSuicide(true, true));
            }
        }
        

        public void ConfigureForRetake()
        {   
            Server.ExecuteCommand($"execifexists cs2retake/retake.cfg");
        }

        

        private void ModifyGameRulesBombPlanted(bool bombPlanted)
        {
            if (this.GameRules == null)
            {
                this.Log($"GameRules is null. Fetching gamerule...");

                var gameRuleProxyList = this.GetGameRulesProxies();

                if (gameRuleProxyList.Count > 1)
                {
                    this.Log($"Multiple GameRuleProxies found. Using firstOrDefault");
                }

                var gameRuleProxy = gameRuleProxyList.FirstOrDefault();

                if (gameRuleProxy == null)
                {
                    this.Log($"GameRuleProxy is null");
                    return;
                }

                if (gameRuleProxy.GameRules == null)
                {
                    this.Log($"GameRules is null");
                    return;
                }

                this.GameRules = gameRuleProxy.GameRules;
            }

            this.GameRules.BombPlanted = bombPlanted;
        }

        private List<CCSGameRulesProxy> GetGameRulesProxies()
        {
            var gameRulesProxyList = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").ToList();

            if(!gameRulesProxyList.Any())
            {
                this.Log($"No gameRuleProxy found!");
            }

            return gameRulesProxyList;
        }

        private List<CCSPlayerController> GetPlayerControllers() 
        {
            var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").ToList();

            if (!playerList.Any())
            {
                this.Log($"No Players have been found!");
            }

            return playerList;
        }

        private CPlantedC4? FindPlantedBomb()
        {
            var plantedBombList = Utilities.FindAllEntitiesByDesignerName<CPlantedC4>("planted_c4");

            if (!plantedBombList.Any())
            {
                this.Log("No planted bomb entities have been found!");
                return null;
            }

            return plantedBombList.FirstOrDefault();
        }

        public override void ResetForNextRound(bool completeReset = true)
        {
            if (completeReset)
            {

            }

            this.BombHasBeenPlanted = false;
        }
    }
}
