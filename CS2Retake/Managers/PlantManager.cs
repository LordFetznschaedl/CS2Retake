using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API;
using CS2Retake.Configs;
using CS2Retake.Managers.Base;
using CS2Retake.Managers.Interfaces;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;
using CSZoneNet.Plugin.Utils.Enums;

namespace CS2Retake.Managers
{
    public class PlantManager : BaseManager, IPlantManager
    {
        private static PlantManager? _instance = null;

        public CounterStrikeSharp.API.Modules.Timers.Timer? HasBombBeenPlantedTimer = null;

        public static PlantManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlantManager();
                }
                return _instance;
            }
        }

        private PlantManager() { }

        public void HandlePlant()
        {
            switch(RuntimeConfig.PlantType)
            {
                case PlantTypeEnum.AutoPlant:
                    this.AutoPlant();
                    break;
                case PlantTypeEnum.FastPlant:
                    this.FastPlant();
                    break;
                default:
                    this.AutoPlant();
                    break;
            }
        }

        private void AutoPlant()
        {
            if(GameRuleManager.Instance.IsWarmup || !PlayerUtils.AreMoreThenOrEqualPlayersConnected(2))
            {
                MessageUtils.LogDebug($"Skipping AutoPlant because its still Warmup or less then 2 players are connected.");
                return;
            }

            var planterPlayerController = RetakeManager.Instance.PlanterPlayerController;

            if (planterPlayerController == null || MapManager.Instance.BombSite == BombSiteEnum.Undefined || planterPlayerController.PlayerPawn == null || planterPlayerController.PlayerPawn.Value == null)
            {
                MessageUtils.Log(LogLevel.Error, $"planterPlayerController null or bombsite undefined [{MapManager.Instance.BombSite}]");
                return;
            }

            var bombPlanterPawn = planterPlayerController.PlayerPawn.Value;

            if (bombPlanterPawn == null || !bombPlanterPawn.IsValid)
            {
                MessageUtils.Log(LogLevel.Error, "bombPlanterPawn null or isn't valid");
                return;
            }
            if (bombPlanterPawn.AbsOrigin == null)
            {
                MessageUtils.Log(LogLevel.Error, "bombPlanterPawn AbsOrigin null");
                return;
            }
            if (bombPlanterPawn.AbsRotation == null)
            {
                MessageUtils.Log(LogLevel.Error, "bombPlanterPawn AbsRotation null");
                return;
            }
            if (bombPlanterPawn.TeamNum != (int)CsTeam.Terrorist)
            {
                MessageUtils.Log(LogLevel.Error, "bombPlanterPawn team is not terrorist");
                return;
            }

            var plantedc4 = Utilities.CreateEntityByName<CPlantedC4>("planted_c4");

            if (plantedc4 == null)
            {
                MessageUtils.Log(LogLevel.Error, "plantedc4 null");
                return;
            }
            if (plantedc4.AbsOrigin == null)
            {
                MessageUtils.Log(LogLevel.Error, "plantedc4 AbsOrigin is null");
                return;
            }
            plantedc4.AbsOrigin.X = bombPlanterPawn.AbsOrigin.X;
            plantedc4.AbsOrigin.Y = bombPlanterPawn.AbsOrigin.Y;
            plantedc4.AbsOrigin.Z = bombPlanterPawn.AbsOrigin.Z;
            plantedc4.HasExploded = false;

            plantedc4.BombSite = (int)MapManager.Instance.BombSite;
            plantedc4.BombTicking = true;
            plantedc4.CannotBeDefused = false;

            plantedc4.DispatchSpawn();

            GameRuleManager.Instance.BombPlanted = true;
            GameRuleManager.Instance.BombDefused = false;

            this.FireBombPlantedEvent(planterPlayerController, MapManager.Instance.BombSite);
        }

        private void FastPlant()
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
        }

        public void HasBombBeenPlanted()
        {
            if (RuntimeConfig.SecondsUntilBombPlantedCheck <= 0 || GameRuleManager.Instance.IsWarmup || !PlayerUtils.AreMoreThenOrEqualPlayersConnected(2))
            {
                return;
            }

            //Finding planted_c4 or weapon_c4
            var bombList = Utilities.FindAllEntitiesByDesignerName<CCSWeaponBase>("c4");

            if (!bombList.Any() && !GameRuleManager.Instance.IsWarmup && PlayerUtils.GetPlayerControllersOfTeam(CsTeam.Terrorist).Any())
            {
                MessageUtils.PrintToChatAll($"No bomb was found in any players inventory resetting.");
                TeamManager.Instance.ScrambleTeams();
                Utilities.GetPlayers().ForEach(x => x?.PlayerPawn?.Value?.CommitSuicide(false, true));
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

            var planterPlayerController = RetakeManager.Instance.PlanterPlayerController;

            if (planterPlayerController != null)
            {
                Server.PrintToChatAll($"{MessageUtils.PluginPrefix} Player {ChatColors.DarkRed}{planterPlayerController.PlayerName}{ChatColors.White} failed to plant the bomb in time. Counter-Terrorists win this round.");
            }

            //var terroristPlayerList = Utilities.GetPlayers().Where(x => x != null && x.IsValid && x.PlayerPawn != null && x.PlayerPawn.IsValid && x.PlayerPawn.Value != null && x.PlayerPawn.Value.IsValid && x.TeamNum == (int)CsTeam.Terrorist).ToList();
            //terroristPlayerList.ForEach(x => x?.PlayerPawn?.Value?.CommitSuicide(true, true));

            GameRuleManager.Instance.TerminateRound(CounterStrikeSharp.API.Modules.Entities.Constants.RoundEndReason.CTsWin);
        }

        private void FireBombPlantedEvent(CCSPlayerController planterController, BombSiteEnum bombsite)
        {
            if (!planterController.IsValid || planterController.PlayerPawn.Value == null)
            {
                return;
            }

            var eventPtr = NativeAPI.CreateEvent("bomb_planted", true);
            NativeAPI.SetEventPlayerController(eventPtr, "userid", planterController.Handle);
            NativeAPI.SetEventInt(eventPtr, "userid", (int)planterController.PlayerPawn.Value.Index);
            NativeAPI.SetEventInt(eventPtr, "site", (int)bombsite);

            NativeAPI.FireEvent(eventPtr, false);
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
            if (this.HasBombBeenPlantedTimer != null)
            {
                this.HasBombBeenPlantedTimer?.Kill();
            }
        }

        public override void ResetForNextMap(bool completeReset = true)
        {

        }
    }
}
