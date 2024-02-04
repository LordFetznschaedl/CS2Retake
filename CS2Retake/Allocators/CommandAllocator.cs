using CounterStrikeSharp.API.Core;
using CS2Retake.Allocators.Configs;
using CSZoneNet.Plugin.CS2BaseAllocator;
using CSZoneNet.Plugin.CS2BaseAllocator.Configs.Interfaces;
using CSZoneNet.Plugin.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Allocators
{
    public class CommandAllocator : BaseAllocator, IAllocatorConfig<CommandAllocatorConfig>
    {
        public CommandAllocatorConfig AllocatorConfig { get; set; } = new CommandAllocatorConfig();

        public override (string primaryWeapon, string secondaryWeapon, KevlarEnum kevlar, bool kit, List<GrenadeEnum> grenades) Allocate(CCSPlayerController player, RoundTypeEnum roundType = RoundTypeEnum.Undefined)
        {
            return ("weapon_ak47", "weapon_deagle", KevlarEnum.KevlarHelmet, true, new List<GrenadeEnum>());
        }
    }
}
