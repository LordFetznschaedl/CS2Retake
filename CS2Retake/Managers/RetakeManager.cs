using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Utils;
using CS2Retake.Managers.Base;
using Microsoft.Extensions.Logging;
using CS2Retake.Managers.Interfaces;
using CounterStrikeSharp.API.Modules.Entities;
using CS2Retake.Configs;

namespace CS2Retake.Managers
{
    public class RetakeManager : BaseManager, IRetakeManager
    {
        private static RetakeManager? _instance = null;
        public bool BombHasBeenPlanted { get; set; } = false;

        private CCSPlayerController? _planterPlayerController = null;

        public CCSPlayerController? PlanterPlayerController
        {
            get
            {

                return this._planterPlayerController;
            }
        }



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

        public void AssignRandomPlayerInBombZoneAsPlanter()
        {

            var random = new Random();
            var plantSpawn = MapManager.Instance.CurrentMap?.SpawnPoints.Where(spawn => spawn.SpawnUsedBy != null && spawn.IsInBombZone).OrderBy(x => random.Next()).FirstOrDefault();


            if(plantSpawn == null)
            {
                MessageUtils.Log(LogLevel.Warning,$"No valid plant spawn found! This might be because no player is on terrorist team.");
                return;
            }

            if(plantSpawn.SpawnUsedBy == null)
            {
                MessageUtils.Log(LogLevel.Error, $"Spawn is not used by any player");
                return;
            }

            this._planterPlayerController = plantSpawn.SpawnUsedBy;

            if(this._planterPlayerController == null)
            {
                MessageUtils.Log(LogLevel.Error, $"Player that uses the valid plant spawn is null");
                return;
            }

            if (RuntimeConfig.PlantType == PlantTypeEnum.AutoPlant)
            {
                return;
            }

            this._planterPlayerController.GiveNamedItem("weapon_c4");
            this._planterPlayerController.ExecuteClientCommand("slot5");

            

            if (RuntimeConfig.SecondsUntilBombPlantedCheck > 0)
            {
                this._planterPlayerController.PrintToCenter($"YOU HAVE {ChatColors.DarkRed}{RuntimeConfig.SecondsUntilBombPlantedCheck}{ChatColors.White} SECONDS TO PLANT THE BOMB!");
            }
        }


        public void PlaySpotAnnouncer()
        {
            var bombsite = MapManager.Instance.BombSite;

            foreach(var player in Utilities.GetPlayers().FindAll(x => x.TeamNum == (int)CsTeam.CounterTerrorist))
            {
                player.ExecuteClientCommand($"play sounds/vo/agents/seal_epic/loc_{bombsite.ToString().ToLower()}_01");
            }
        }

        public void ConfigureForRetake(bool complete = true)
        {   
            Server.ExecuteCommand($"execifexists cs2retake/retake.cfg");
        }

        public override void ResetForNextRound(bool completeReset = true)
        {
            if (completeReset)
            {
                
            }

            this.BombHasBeenPlanted = false;
        }

        public override void ResetForNextMap(bool completeReset = true)
        {

        }
    }
}
