using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators.Implementations.CommandAllocator.Utils;
using CSZoneNet.Plugin.CS2BaseAllocator.Configs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Configs
{
    public class CommandAllocatorConfig : BaseAllocatorConfig
    {
        public bool EnableRoundTypePistolMenu { get; set; } = true;
        public bool EnableRoundTypeMidMenu { get; set; } = true;
        public bool EnableRoundTypeFullBuyMenu { get; set; } = true;

        public int DefuseKitChance { get; set; } = 100;

        public bool EnableZeus { get; set; } = false;
        public int ZeusChance { get; set; } = 20;

        public DBType DatabaseType { get; set; } = DBType.SQLite;

        public float HowToMessageDelayInMinutes { get; set; } = 3.5f;
        public string HowToMessage { get; set; } = $"Customize your weapons by using !guns";

        public CommandAllocatorConfig()
        {
            this.Version = 3;
        }
    }
}
