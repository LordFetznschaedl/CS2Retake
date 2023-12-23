using CounterStrikeSharp.API;
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
    public class GameRuleManager : BaseManager, IGameRuleManager
    {
        private static GameRuleManager? _instance = null;

        public CCSGameRules? GameRules { get; set; } = null;

        public static GameRuleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameRuleManager();
                }
                return _instance;
            }
        }

        private GameRuleManager() { }

        public bool IsWarmup
        {
            get
            {
                if (this.GameRules == null)
                {
                    this.GetGameRules();
                }

                return this.GameRules?.WarmupPeriod ?? false;
            }
        }

        private void GetGameRules()
        {
            MessageUtils.Log(LogLevel.Information, $"GameRules is null. Fetching gamerule...");

            var gameRuleProxyList = this.GetGameRulesProxies();

            if (gameRuleProxyList.Count > 1)
            {
                MessageUtils.Log(LogLevel.Error, $"Multiple GameRuleProxies found. Using firstOrDefault");
            }

            var gameRuleProxy = gameRuleProxyList.FirstOrDefault();

            if (gameRuleProxy == null)
            {
                MessageUtils.Log(LogLevel.Error, $"GameRuleProxy is null");
                return;
            }

            if (gameRuleProxy.GameRules == null)
            {
                MessageUtils.Log(LogLevel.Error, $"GameRules is null");
                return;
            }

            this.GameRules = gameRuleProxy.GameRules;
        }

        private List<CCSGameRulesProxy> GetGameRulesProxies()
        {
            var gameRulesProxyList = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").ToList();

            if (!gameRulesProxyList.Any())
            {
                MessageUtils.Log(LogLevel.Error, $"No gameRuleProxy found!");
            }

            return gameRulesProxyList;
        }

        public override void ResetForNextRound(bool completeReset = true)
        {
            
        }
    }
}
