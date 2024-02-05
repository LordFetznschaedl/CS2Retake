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
        public bool EnableRoundTypeMenuPistol { get; set; } = true;
        public bool EnableRoundTypeMenuMid { get; set; } = true;
        public bool EnableRoundTypeMenuFullBuy { get; set; } = true;

        public string primary { get; set; } = "weapon_ak47";
        public string secondary { get; set; } = "weapon_deagle";

        public CommandAllocatorConfig()
        {
            this.Version = 1;
        }
    }
}
