using CounterStrikeSharp.API.Core;
using CS2Retake.Allocators.Implementations.CommandAllocator.Configs;
using CSZoneNet.Plugin.CS2BaseAllocator.Configs.Interfaces;
using CSZoneNet.Plugin.CS2BaseAllocator;
using CSZoneNet.Plugin.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS2Retake.Utils;
using Microsoft.Extensions.Logging;

namespace CS2Retake.Allocators.Implementations.CommandAllocator
{
    //public class CommandAllocator : BaseAllocator, IAllocatorConfig<CommandAllocatorConfig>
    public class CommandAllocator : BaseAllocator, IAllocatorConfig<CommandAllocatorConfig>
    {
        public CommandAllocatorConfig Config { get; set; } = new CommandAllocatorConfig();

        public override (string primaryWeapon, string secondaryWeapon, KevlarEnum kevlar, bool kit, List<GrenadeEnum> grenades) Allocate(CCSPlayerController player, RoundTypeEnum roundType = RoundTypeEnum.Undefined)
        {
            return (Config.primary, Config.secondary, KevlarEnum.KevlarHelmet, true, new List<GrenadeEnum>());
        }

        public void OnAllocatorConfigParsed(CommandAllocatorConfig config)
        {
            this.Config = config;
        }

        public override void OnGunsCommand(CCSPlayerController? player)
        {
            MessageUtils.Log(LogLevel.Error, $"---!guns");
        }
    }
}
