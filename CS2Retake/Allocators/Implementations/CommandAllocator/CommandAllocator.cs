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
using CS2Retake.Allocators.Implementations.CommandAllocator.Menus;

namespace CS2Retake.Allocators.Implementations.CommandAllocator
{
    public class CommandAllocator : BaseGrenadeAllocator, IAllocatorConfig<CommandAllocatorConfig>
    {
        public CommandAllocatorConfig Config { get; set; } = new CommandAllocatorConfig();

        public CommandAllocator()
        {

        }

        public override (string primaryWeapon, string secondaryWeapon, KevlarEnum kevlar, bool kit, List<GrenadeEnum> grenades) Allocate(CCSPlayerController player, RoundTypeEnum roundType = RoundTypeEnum.Undefined)
        {
            (string primaryWeapon, string secondaryWeapon, KevlarEnum kevlar, bool kit, List<GrenadeEnum> grenades) returnValue = ("", "weapon_deagle", KevlarEnum.KevlarHelmet, true, new List<GrenadeEnum>());

            if (player == null || !player.IsValid || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value!.IsValid)
            {
                return returnValue;
            }

            var grenades = this.AllocateGrenades(player, roundType);

            if (grenades == null)
            {
                return returnValue;
            }

            returnValue.grenades = grenades;





            return returnValue;
        }

        public void OnAllocatorConfigParsed(CommandAllocatorConfig config)
        {
            this.Config = config;
        }

        public override void OnGunsCommand(CCSPlayerController? player)
        {
            if(this.BasePluginInstance == null)
            {
                return;
            }

            ChooserMenu.OpenMenu(player, this.BasePluginInstance, this.Config.EnableRoundTypePistolMenu, this.Config.EnableRoundTypeMidMenu, this.Config.EnableRoundTypeFullBuyMenu);
        }
    }
}
