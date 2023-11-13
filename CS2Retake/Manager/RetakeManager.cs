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
using CS2Retake.Entity;
using CounterStrikeSharp.API.Modules.Entities;

namespace CS2Retake.Manager
{
    public class RetakeManager
    {
        private static RetakeManager? _instance = null;
        public string ModuleName { get; set; }
        public bool BombHasBeenAssigned { get; set; } = false;

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


        public void PlantBomb()
        {

            var random = new Random();
            var plantSpawn = MapManager.Instance.CurrentMap.SpawnPoints.Where(spawn => spawn.SpawnUsedBy != null && spawn.IsInBombZone).OrderBy(x => random.Next()).FirstOrDefault();


            if(plantSpawn == null)
            {
                this.Log($"No valid plant spawn found!");
                return;
            }

            var player = plantSpawn.SpawnUsedBy;

            if(player == null)
            {
                this.Log($"Player that uses the valid plant spawn is null");
                return;
            }

            //player.GiveNamedItem("weapon_c4");

            //var c4list = Utilities.FindAllEntitiesByDesignerName<CC4>("weapon_c4");


            //if (!c4list.Any())
            //{
            //    return;
            //}

            //var c4 = c4list.FirstOrDefault();
            //if (c4 == null)
            //{ return; }

            //c4.StartedArming = true;
            //c4.IsPlantingViaUse = true;
            //c4.ArmedTime = 0;
            //c4.BombPlanted = true;

            




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

        public void ConfigureForRetake()
        {   
            Server.ExecuteCommand($"execifexists cs2retake/retake.cfg");
        }

        public void ResetForNextRound(bool completeReset = true)
        {
            if (completeReset)
            {
                
            }

            this.ModifyGameRulesBombPlanted(false);
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

        private CBombTarget? GetBombTarget()
        {
            var bombTargetList = Utilities.FindAllEntitiesByDesignerName<CBombTarget>("func_bomb_target");

            if (!bombTargetList.Any())
            {
                return null;
            }

            var isBombSiteB = MapManager.Instance.BombSite == Utils.BombSiteEnum.B;

            return bombTargetList.FirstOrDefault(x => x.IsBombSiteB == isBombSiteB);
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

        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{this.ModuleName}:{this.GetType().Name}] {message}");
            Console.ResetColor();
        }

    }
}
