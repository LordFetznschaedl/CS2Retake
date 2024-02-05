﻿using CSZoneNet.Plugin.CS2BaseAllocator.Configs.Base;
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

        public string primary { get; set; } = "weapon_ak47";
        public string secondary { get; set; } = "weapon_deagle";

        public CommandAllocatorConfig()
        {
            this.Version = 1;
        }
    }
}