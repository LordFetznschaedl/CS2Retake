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

        public List<CCSPlayerController> PlayerJoinQueue = new List<CCSPlayerController>();

        public CounterStrikeSharp.API.Modules.Timers.Timer? HasBombBeenPlantedTimer = null;

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

        public void GiveBombToPlayerRandomPlayerInBombZone()
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

            this._planterPlayerController.GiveNamedItem("weapon_c4");

            if(RuntimeConfig.SecondsUntilBombPlantedCheck > 0)
            {
                this._planterPlayerController.PrintToCenter($"YOU HAVE {ChatColors.Darkred}{RuntimeConfig.SecondsUntilBombPlantedCheck}{ChatColors.White} SECONDS TO PLANT THE BOMB!");
            }
            

            //_ = new CounterStrikeSharp.API.Modules.Timers.Timer(seconds, this.HasBombBeenPlantedCallback);

            //c4.BombPlacedAnimation = false;



            //var plantedBomb = this.FindPlantedBomb();

            //if(plantedBomb == null)
            //{
            //    MessageUtils.Log(LogLevel.Warning,$"No planted bomb was found!");
            //    return;
            //}

            //var playerPawn = player.PlayerPawn.Value;

            //if(playerPawn == null)
            //{
            //    MessageUtils.Log(LogLevel.Warning,$"Player pawn is null");
            //    return;
            //}
            //if(playerPawn.AbsRotation == null)
            //{
            //    MessageUtils.Log(LogLevel.Warning,$"Player pawn rotation is null");
            //    return;
            //}
            //if(playerPawn.AbsOrigin == null)
            //{
            //    MessageUtils.Log(LogLevel.Warning,$"Player pawn position is null");
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


        public void HasBombBeenPlanted()
        {
            if (RuntimeConfig.SecondsUntilBombPlantedCheck <= 0 && GameRuleManager.Instance.IsWarmup)
            {
                return;
            }

            //Finding planted_c4 or weapon_c4
            var bombList = Utilities.FindAllEntitiesByDesignerName<CCSWeaponBase>("c4");

            if (!bombList.Any() && !GameRuleManager.Instance.IsWarmup && PlayerUtils.GetPlayerControllersOfTeam(CsTeam.Terrorist).Any())
            {
                MessageUtils.PrintToChatAll($"No bomb was found in any players inventory resetting.");
                TeamManager.Instance.ScrambleTeams();
                this.GetPlayerControllers().ForEach(x => x?.PlayerPawn?.Value?.CommitSuicide(false, true));
                return;
            }

            this.HasBombBeenPlantedTimer = new CounterStrikeSharp.API.Modules.Timers.Timer(RuntimeConfig.SecondsUntilBombPlantedCheck, this.HasBombBeenPlantedCallback);
            

        }

        public void HasBombBeenPlantedCallback()
        {
            var plantedBomb = this.FindPlantedBomb();

            if (plantedBomb != null)
            {
                return;
            }

            if(this._planterPlayerController != null)
            {
                Server.PrintToChatAll($"{MessageUtils.PluginPrefix} Player {ChatColors.Darkred}{this._planterPlayerController.PlayerName}{ChatColors.White} failed to plant the bomb in time. Counter-Terrorists win this round.");
            }

            var terroristPlayerList = this.GetPlayerControllers().Where(x => x != null && x.IsValid && x.PlayerPawn != null && x.PlayerPawn.IsValid && x.PlayerPawn.Value != null && x.PlayerPawn.Value.IsValid && x.TeamNum == (int)CsTeam.Terrorist).ToList();
            terroristPlayerList.ForEach(x => x?.PlayerPawn?.Value?.CommitSuicide(true, true));
            
        }
        
        public void PlaySpotAnnouncer()
        {
            var bombsite = MapManager.Instance.BombSite;

            foreach(var player in this.GetPlayerControllers().FindAll(x => x.TeamNum == (int)CsTeam.CounterTerrorist))
            {
                player.ExecuteClientCommand($"play sounds/vo/agents/seal_epic/loc_{bombsite.ToString().ToLower()}_01");
            }
        }

        public void ConfigureForRetake()
        {   
            Server.ExecuteCommand($"execifexists cs2retake/retake.cfg");
        }

        private List<CCSPlayerController> GetPlayerControllers() 
        {
            var playerList = Utilities.GetPlayers();

            if (!playerList.Any())
            {
                MessageUtils.Log(LogLevel.Error, $"No Players have been found!");
            }

            return playerList;
        }

        private CPlantedC4? FindPlantedBomb()
        {
            var plantedBombList = Utilities.FindAllEntitiesByDesignerName<CPlantedC4>("planted_c4");

            if (!plantedBombList.Any())
            {
                MessageUtils.Log(LogLevel.Warning, "No planted bomb entities have been found! This might be because no bomb was planted.");
                return null;
            }

            return plantedBombList.FirstOrDefault();
        }

        public override void ResetForNextRound(bool completeReset = true)
        {
            if (completeReset)
            {
                if(this.HasBombBeenPlantedTimer != null)
                {
                    this.HasBombBeenPlantedTimer?.Kill();
                }
            }

            this.BombHasBeenPlanted = false;
        }
    }
}
