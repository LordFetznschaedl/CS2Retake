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
using CS2Retake.Allocators.Implementations.CommandAllocator.Manager;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Allocators.Implementations.CommandAllocator.Entities;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Timers;

namespace CS2Retake.Allocators.Implementations.CommandAllocator
{
    public class CommandAllocator : BaseGrenadeAllocator, IAllocatorConfig<CommandAllocatorConfig>, IDisposable
    {
        public CommandAllocatorConfig Config { get; set; } = new CommandAllocatorConfig();

        private ChanceEntity _awpChanceCT { get; set; }
        private ChanceEntity _awpChanceT { get; set; }

        private int _awpInUseCountCT { get; set; } = 0;
        private int _awpInUseCountT { get; set; } = 0;

        private CounterStrikeSharp.API.Modules.Timers.Timer? _howToTimer { get; set; } = null;

        public CommandAllocator()
        {

        }

        public override (string primaryWeapon, string secondaryWeapon, KevlarEnum kevlar, bool kit, bool zeus, List<GrenadeEnum> grenades) Allocate(CCSPlayerController player, RoundTypeEnum roundType = RoundTypeEnum.Undefined)
        {
            (string primaryWeapon, string secondaryWeapon, KevlarEnum kevlar, bool kit, bool zeus, List<GrenadeEnum> grenades) returnValue = ("", "weapon_deagle", KevlarEnum.KevlarHelmet, true, false, new List<GrenadeEnum>());

            if (player == null || !player.IsValid || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value!.IsValid)
            {
                return returnValue;
            }

            var random = new Random();

            //GRENADES

            var grenades = this.AllocateGrenades(player, roundType);

            if (grenades == null)
            {
                return returnValue;
            }

            returnValue.grenades = grenades;

            //RIFLES AND PISTOLS

            (string? primary, string? secondary, int? awpChance) weapons = ("", "", null);
            (string primary, string secondary, int? awpChance) defaultWeapons = ("", "weapon_deagle", 0);
            switch (roundType)
            {
                case RoundTypeEnum.FullBuy:
                    weapons = CacheManager.Instance.GetFullBuyWeapons(player);
                    defaultWeapons.primary = player.Team == CsTeam.CounterTerrorist ? "weapon_m4a1" : "weapon_ak47";

                    var chanceEntity = player.Team == CsTeam.CounterTerrorist ? this._awpChanceCT : this._awpChanceT;

                    if((player.Team == CsTeam.CounterTerrorist && chanceEntity.Limit <= this._awpInUseCountCT) || (player.Team == CsTeam.Terrorist && chanceEntity.Limit <= this._awpInUseCountT))
                    {
                        break;
                    }

                    var randomValue = random.Next(1, 100);

                    if(randomValue <= weapons.awpChance)
                    {
                        weapons.primary = "weapon_awp";

                        if(player.Team == CsTeam.CounterTerrorist)
                        {
                            this._awpInUseCountCT++;
                        }
                        else
                        {
                            this._awpInUseCountT++;
                        }
                    }


                    break;
                case RoundTypeEnum.Mid:
                    weapons = CacheManager.Instance.GetMidWeapons(player);
                    defaultWeapons.primary = player.Team == CsTeam.CounterTerrorist ? "weapon_mp9" : "weapon_mac10";
                    break;
                case RoundTypeEnum.Pistol:
                    weapons = CacheManager.Instance.GetPistolWeapons(player);
                    defaultWeapons.secondary = player.Team == CsTeam.CounterTerrorist ? "weapon_usp_silencer" : "weapon_glock";
                    break;

            }

            MessageUtils.LogDebug($"SteamID: {player.SteamID} - Default: {defaultWeapons.primary}, {defaultWeapons.secondary}, {defaultWeapons.awpChance} - Selected: {weapons.primary}, {weapons.secondary}, {weapons.awpChance}");

            if (string.IsNullOrWhiteSpace(weapons.primary))
            {
                returnValue.primaryWeapon = defaultWeapons.primary;
            }
            else
            {
                returnValue.primaryWeapon = weapons.primary;
            }
            if (string.IsNullOrWhiteSpace(weapons.secondary))
            {
                returnValue.secondaryWeapon = defaultWeapons.secondary;
            }
            else
            {
                returnValue.secondaryWeapon = weapons.secondary;
            }


            //ZEUS
            if (this.Config.EnableZeus)
            {
                returnValue.zeus = random.Next(1,100) <= this.Config.ZeusChance;
            }


            //DEFUSEKIT
            returnValue.kit = this.Config.DefuseKitChance == 100 || random.Next(1, 100) <= this.Config.DefuseKitChance;



            return returnValue;
        }

        public void OnAllocatorConfigParsed(CommandAllocatorConfig config)
        {
            if (this.Config.Version > config.Version)
            {
                MessageUtils.Log(Microsoft.Extensions.Logging.LogLevel.Warning, $"The command allocator configuration is out of date. Consider updating the config. [Current Version: {config.Version} - Allocator Version: {this.Config.Version}]");
            }

            this.Config = config;

            var fullBuyConfig = FullBuyMenu.Instance.Config;
            _ = MidMenu.Instance;
            _ = PistolMenu.Instance;

            this._awpChanceCT = fullBuyConfig.AWPChanceCT;
            this._awpChanceT = fullBuyConfig.AWPChanceT;

            MessageUtils.Log(LogLevel.Warning, $"For some reason sometimes an exception happens with the sqlite stuff. It still works. So no worries needed.");

            DBManager.Instance.DBType = Config.DatabaseType;
            DBManager.Instance.AllocatorConfigDirectoryPath = Config.AllocatorConfigDirectoryPath;
            DBManager.Instance.ConnectionString = Config.ConnectionString;
            DBManager.Instance.Init();

            PlayerUtils.GetValidPlayerControllers().ForEach(x => this.OnPlayerConnected(x));


            if(this.Config.HowToMessageDelayInMinutes > 0)
            {
                this._howToTimer = new CounterStrikeSharp.API.Modules.Timers.Timer(this.Config.HowToMessageDelayInMinutes * 60, PrintHowToMessage, CounterStrikeSharp.API.Modules.Timers.TimerFlags.REPEAT);
            }
            
        }

        public override void OnGunsCommand(CCSPlayerController? player)
        {
            if(this.BasePluginInstance == null)
            {
                return;
            }

            ChooserMenu.OpenMenu(player, this.BasePluginInstance, this.Config.EnableRoundTypePistolMenu, this.Config.EnableRoundTypeMidMenu, this.Config.EnableRoundTypeFullBuyMenu);
        }

        public override void OnPlayerConnected(CCSPlayerController? player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value!.IsValid)
            {
                return;
            }

            //----------------------------FULLBUY--------------------------------------

            var fullBuyConfig = FullBuyMenu.Instance.Config;

            (string? primaryWeapon, string? secondaryWeapon, int? awpChance) fullBuyCT = DBManager.Instance.GetFullBuyWeapons(player.SteamID, (int)CsTeam.CounterTerrorist);

            if(!string.IsNullOrWhiteSpace(fullBuyCT.primaryWeapon))
            {
                CacheManager.Instance.AddOrUpdateFullBuyPrimaryCache(player, fullBuyCT.primaryWeapon, CsTeam.CounterTerrorist);
            }
            if (!string.IsNullOrWhiteSpace(fullBuyCT.secondaryWeapon))
            {
                CacheManager.Instance.AddOrUpdateFullBuySecondaryCache(player, fullBuyCT.secondaryWeapon, CsTeam.CounterTerrorist);
            }
            if(fullBuyCT.awpChance != null && fullBuyCT.awpChance.HasValue) 
            {
                var chance = fullBuyCT.awpChance.Value;
                var highestChance = fullBuyConfig.AWPChanceCT.Chances.OrderDescending().FirstOrDefault();

                if (!fullBuyConfig.EnableAWPChance)
                {
                    chance = 0;
                }

                CacheManager.Instance.AddOrUpdateFullBuyAWPChanceCache(player, chance <= highestChance ? chance : highestChance, CsTeam.CounterTerrorist);
            }

            var fullBuyT = DBManager.Instance.GetFullBuyWeapons(player.SteamID, (int)CsTeam.Terrorist);

            if (!string.IsNullOrWhiteSpace(fullBuyT.primaryWeapon))
            {
                CacheManager.Instance.AddOrUpdateFullBuyPrimaryCache(player, fullBuyT.primaryWeapon, CsTeam.Terrorist);
            }
            if (!string.IsNullOrWhiteSpace(fullBuyT.secondaryWeapon))
            {
                CacheManager.Instance.AddOrUpdateFullBuySecondaryCache(player, fullBuyT.secondaryWeapon, CsTeam.Terrorist);
            }
            if (fullBuyT.awpChance != null && fullBuyT.awpChance.HasValue)
            {
                var chance = fullBuyT.awpChance.Value;
                var highestChance = fullBuyConfig.AWPChanceT.Chances.OrderDescending().FirstOrDefault();

                if(!fullBuyConfig.EnableAWPChance)
                {
                    chance = 0;
                }

                CacheManager.Instance.AddOrUpdateFullBuyAWPChanceCache(player, chance <= highestChance ? chance : highestChance, CsTeam.Terrorist);
            }

            MessageUtils.LogDebug($"SteamID: {player.SteamID} - CT: {fullBuyCT.primaryWeapon}, {fullBuyCT.secondaryWeapon}, {fullBuyCT.awpChance} - T: {fullBuyT.primaryWeapon}, {fullBuyT.secondaryWeapon}, {fullBuyT.awpChance}");

            //----------------------------MID--------------------------------------

            var midCT = DBManager.Instance.GetMidWeapons(player.SteamID, (int)CsTeam.CounterTerrorist);

            if (!string.IsNullOrWhiteSpace(midCT.primaryWeapon))
            {
                CacheManager.Instance.AddOrUpdateMidPrimaryCache(player, midCT.primaryWeapon, CsTeam.CounterTerrorist);
            }
            if (!string.IsNullOrWhiteSpace(midCT.secondaryWeapon))
            {
                CacheManager.Instance.AddOrUpdateMidSecondaryCache(player, midCT.secondaryWeapon, CsTeam.CounterTerrorist);
            }

            var midT = DBManager.Instance.GetMidWeapons(player.SteamID, (int)CsTeam.Terrorist);

            if (!string.IsNullOrWhiteSpace(midT.primaryWeapon))
            {
                CacheManager.Instance.AddOrUpdateMidPrimaryCache(player, midT.primaryWeapon, CsTeam.Terrorist);
            }
            if (!string.IsNullOrWhiteSpace(midT.secondaryWeapon))
            {
                CacheManager.Instance.AddOrUpdateMidSecondaryCache(player, midT.secondaryWeapon, CsTeam.Terrorist);
            }

            MessageUtils.LogDebug($"SteamID: {player.SteamID} - CT: {midCT.primaryWeapon}, {midCT.secondaryWeapon}, {midCT.awpChance} - T: {midT.primaryWeapon}, {midT.secondaryWeapon}, {midT.awpChance}");

            //----------------------------PISTOLS--------------------------------------

            var pistolCT = DBManager.Instance.GetPistolWeapons(player.SteamID, (int)CsTeam.CounterTerrorist);

            if (!string.IsNullOrWhiteSpace(pistolCT.secondaryWeapon))
            {
                CacheManager.Instance.AddOrUpdatePistolCache(player, pistolCT.secondaryWeapon, CsTeam.CounterTerrorist);
            }

            var pistolT = DBManager.Instance.GetPistolWeapons(player.SteamID, (int)CsTeam.Terrorist);

            if (!string.IsNullOrWhiteSpace(pistolT.secondaryWeapon))
            {
                CacheManager.Instance.AddOrUpdatePistolCache(player, pistolT.secondaryWeapon, CsTeam.Terrorist);
            }

            MessageUtils.LogDebug($"SteamID: {player.SteamID} - CT: {pistolCT.primaryWeapon}, {pistolCT.secondaryWeapon}, {pistolCT.awpChance} - T: {pistolT.primaryWeapon}, {pistolT.secondaryWeapon}, {pistolT.awpChance}");
        }

        public override void OnPlayerDisconnected(CCSPlayerController? player)
        {
            if (player == null || !player.IsValid || player.PlayerPawn == null || !player.PlayerPawn.IsValid || player.PlayerPawn.Value == null || !player.PlayerPawn.Value!.IsValid)
            {
                return;
            }

            CacheManager.Instance.RemoveUserFromCache(player);
        }

        public override void ResetForNextRound(bool completeReset = true)
        {
            this._awpInUseCountCT = 0;
            this._awpInUseCountT = 0;
        }

        private void PrintHowToMessage()
        {
            Server.PrintToChatAll($"[{ChatColors.Gold}CommandAllocator{ChatColors.White}] {this.Config.HowToMessage}");
        }

        public void Dispose()
        {
            this._howToTimer?.Kill();
        }
    }
}
