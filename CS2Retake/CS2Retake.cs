using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Configs;
using CS2Retake.Entities;
using CS2Retake.Managers;
using CS2Retake.Utils;
using Microsoft.Extensions.Logging;
using CSZoneNet.Plugin.Utils.Enums;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace CS2Retake
{
    [MinimumApiVersion(159)]
    public class CS2Retake : BasePlugin, IPluginConfig<CS2RetakeConfig>
    {
        public override string ModuleName => "CS2Retake";
        public override string ModuleVersion => "1.3.0";
        public override string ModuleAuthor => "LordFetznschaedl";
        public override string ModuleDescription => "Highly configurable and modular implementation Retake for CS2";

        public CS2RetakeConfig Config { get; set; } = new CS2RetakeConfig();
        private bool _scrambleAfterWarmupDone = false;

        public void OnConfigParsed(CS2RetakeConfig config)
        {
            if(config.Version < this.Config.Version) 
            {
                this.Logger?.LogWarning($"The plugin configuration is out of date. Consider updating the config. [Current Version: {config.Version} - Plugin Version: {this.Config.Version}]");
            }
            this.Config = config;
        }

        public override void Load(bool hotReload)
        {
            this.Logger?.LogInformation(this.PluginInfo());
            this.Logger?.LogInformation(this.ModuleDescription);

            MessageUtils.Logger = this.Logger;

            RuntimeConfig.SetModuleInfo(this.ModuleName, this.ModuleDirectory);
            RuntimeConfig.SetBaseConfig(this.Config);
            FeatureConfig.SetBaseConfig(this.Config);

            if (MapManager.Instance.CurrentMap == null)
            {
                this.OnMapStart(Server.MapName);
            }

            if(FeatureConfig.EnableThankYouMessage)
            {
                this.AddTimer(7 * 60, MessageUtils.ThankYouMessage, TimerFlags.REPEAT);
            }

            if(hotReload)
            {
                Server.ExecuteCommand($"map {Server.MapName}");
            }

            this.RegisterListener<Listeners.OnMapStart>(mapName => this.OnMapStart(mapName));
            this.RegisterListener<Listeners.OnTick>(this.OnTick);

            this.RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
            this.RegisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
            this.RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
            this.RegisterEventHandler<EventCsPreRestart>(OnCsPreRestart);
            this.RegisterEventHandler<EventBombBeginplant>(OnBombBeginPlant);
            this.RegisterEventHandler<EventPlayerTeam>(OnPlayerTeam, HookMode.Pre);
            this.RegisterEventHandler<EventBeginNewMatch>(OnBeginNewMatch, HookMode.Pre);
            this.RegisterEventHandler<EventCsIntermission>(OnCsIntermission);
            this.RegisterEventHandler<EventRoundStart>(OnRoundStart);

            this.RegisterEventHandler<EventPlayerConnect>(OnPlayerConnect);
            this.RegisterEventHandler<EventPlayerConnectFull>(OnPlayerConnectFull);
            this.RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);

            this.AddCommandListener("jointeam", OnCommandJoinTeam);
        }

        [ConsoleCommand("css_retakeinfo", "This command prints the plugin information")]
        public void OnCommandInfo(CCSPlayerController? player, CommandInfo command)
        {
            command.ReplyToCommand($"{ MessageUtils.PluginPrefix} {PluginInfo()}");
        }

        [ConsoleCommand("css_retakespawn", "This command teleports the player to a spawn with the given index in the args")]
        [RequiresPermissions("@cs2retake/admin")]
        public void OnCommandSpawn(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null)
            {
                this.Logger?.LogError("Command has been called by the server.");
                return;
            }
            if(!player.PlayerPawn.IsValid)
            {
                this.Logger?.LogError("PlayerPawn not valid");
                return;
            }

            if (command.ArgCount != 2)
            {
                this.Logger?.LogError($"ArgCount: {command.ArgCount} - ArgString: {command.ArgString}");
                command.ReplyToCommand($"{MessageUtils.PluginPrefix} One argument with a valid spawn index is needed! Example: !retakespawn <index (int)>");
                return;
            }

            if(!int.TryParse(command.ArgByIndex(1), out int spawnIndex))
            {
                this.Logger?.LogError("Argument index not a valid integer!");
                return;
            }

            MapManager.Instance.TeleportPlayerToSpawn(player, spawnIndex);
        }

        [ConsoleCommand("css_retakewrite", "This command writes the spawns for the current map")]
        [RequiresPermissions("@cs2retake/admin")]
        public void OnCommandWrite(CCSPlayerController? player, CommandInfo command)
        {
            MapManager.Instance.CurrentMap?.SaveSpawns();
            this.Logger?.LogInformation($"{MapManager.Instance.CurrentMap?.SpawnPoints.Count} spawnpoints saved");
        }

        [ConsoleCommand("css_retakeread", "This command reads the spawns for the current map")]
        [RequiresPermissions("@cs2retake/admin")]
        public void OnCommandRead(CCSPlayerController? player, CommandInfo command)
        {
            MapManager.Instance.CurrentMap?.LoadSpawns();
            this.Logger?.LogInformation($"{MapManager.Instance.CurrentMap?.SpawnPoints.Count} spawnpoints read");
        }

        [ConsoleCommand("css_retakescramble", "This command scrambles the teams")]
        [RequiresPermissions("@cs2retake/admin")]
        public void OnCommandScramble(CCSPlayerController? player, CommandInfo command)
        {
            TeamManager.Instance.ScrambleTeams();
        }

        [ConsoleCommand("css_retaketeleport", "This command teleports the player to the given coordinates")]
        [RequiresPermissions("@cs2retake/admin")]
        public void OnCommandTeleport(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null)
            {
                this.Logger?.LogError("Command has been called by the server.");
                return;
            }
            if (!player.PlayerPawn.IsValid)
            {
                this.Logger?.LogError("PlayerPawn not valid");
                return;
            }

            if (command.ArgCount != 4)
            {
                this.Logger?.LogError($"ArgCount: {command.ArgCount} - ArgString: {command.ArgString}");
                command.ReplyToCommand($"{MessageUtils.PluginPrefix} Command format: !retaketeleport <position X float> <position Y float> <position Z float>");
                return;
            }

            if (!float.TryParse(command.ArgByIndex(1), out float positionX))
            {
                this.Logger?.LogError("Argument position X not a valid float!");
                return;
            }

            if (!float.TryParse(command.ArgByIndex(2), out float positionY))
            {
                this.Logger?.LogError("Argument position Y not a valid float!");
                return;
            }

            if (!float.TryParse(command.ArgByIndex(3), out float positionZ))
            {
                this.Logger?.LogError("Argument position Z not a valid float!");
                return;
            }

            player?.PlayerPawn?.Value?.Teleport(new Vector(positionX, positionY, positionZ), new QAngle(0f,0f,0f), new Vector(0f, 0f, 0f));
        }

        [ConsoleCommand("css_retakeaddspawn", "This command adds a new spawn to the current map")]
        [RequiresPermissions("@cs2retake/admin")]
        public void OnCommandAdd(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null)
            {
                this.Logger?.LogError("Command has been called by the server.");
                return;
            }
            if (!player.PlayerPawn.IsValid)
            {
                this.Logger?.LogError("PlayerPawn not valid");
                return;
            }

            if (command.ArgCount != 3)
            {
                this.Logger?.LogError($"ArgCount: {command.ArgCount} - ArgString: {command.ArgString}");
                command.ReplyToCommand($"{MessageUtils.PluginPrefix} Command format: !retakeaddspawn <2/3 - 2 = T; 3 = CT> <0/1 - 0 = A; 1 = B>");
                return;
            }

            if (!int.TryParse(command.ArgByIndex(1), out int team))
            {
                this.Logger?.LogError("Team could not be parsed!");
                return;
            }

            if(team != 2 && team != 3) 
            {
                this.Logger?.LogError("Team index is not in 2 or 3");
                return;
            }

            if (!int.TryParse(command.ArgByIndex(2), out int bombSite))
            {
                this.Logger?.LogError("Team could not be parsed!");
                return;
            }

            if (bombSite != 0 && bombSite != 1)
            {
                this.Logger?.LogError("BombSite index is not in 0 or 1");
                return;
            }

            MapManager.Instance.AddSpawn(player, (CsTeam)team, (BombSiteEnum)bombSite);
        }

        [ConsoleCommand("css_guns", "Base guns command for weapon allocation settings.")]
        public void OnGuns(CCSPlayerController? player, CommandInfo command)
        {
            
        }

        private HookResult OnCommandJoinTeam(CCSPlayerController? player, CommandInfo commandInfo)
        {
            if (player == null || !player.IsValid)
            {
                this.Logger?.LogError("Player is null or not valid");
                return HookResult.Handled;
            }

            MessageUtils.LogDebug($"CommandInfo: ArgString: {commandInfo.ArgString}, CommandString: {commandInfo.GetCommandString}");

            var oldTeam = (CsTeam)player.TeamNum;

            if (commandInfo.ArgCount < 2)
            {
                this.Logger?.LogError("Wrong amount of arguments for JoinTeam");
                return HookResult.Handled;
            }

            if (!Enum.TryParse(commandInfo.GetArg(1), out CsTeam newTeam))
            {
                this.Logger?.LogError("Parsing new team failed");
                return HookResult.Handled;
            }

            TeamManager.Instance.PlayerSwitchTeam(player, oldTeam, newTeam);

            if((oldTeam == CsTeam.CounterTerrorist && newTeam == CsTeam.Terrorist) || (oldTeam == CsTeam.Terrorist && newTeam == CsTeam.CounterTerrorist))
            {
                MessageUtils.PrintToPlayerOrServer($"You cant switch your team manually. This will be done automatically.", player);
                return HookResult.Handled;
            }

            return HookResult.Continue;
            
        }

        public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            if (@event == null)
            {
                return HookResult.Continue;
            }
            if(@event.Userid == null || !@event.Userid.IsValid)
            {
                return HookResult.Continue;
            }
            if(@event.Userid.PlayerPawn == null || !@event.Userid.PlayerPawn.IsValid)
            {
                return HookResult.Continue;
            }
            if (@event.Userid.PlayerPawn.Value == null || !@event.Userid.PlayerPawn.Value.IsValid)
            {
                return HookResult.Continue;
            }

            MapManager.Instance.TeleportPlayerToSpawn(@event.Userid);

            return HookResult.Continue;
        }

        private HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
        {
            var ratios = TeamManager.Instance.LatestRatio;

            if(!GameRuleManager.Instance.IsWarmup && (ratios.ctRatio != PlayerUtils.GetCounterTerroristPlayers().Count || ratios.tRatio != PlayerUtils.GetTerroristPlayers().Count))
            {
                MessageUtils.PrintToChatAll($"Player ratios not matching how they should be. Resetting...");
                PlayerUtils.SuicideAll();
                return HookResult.Continue;
            }

            if(RuntimeConfig.PlantType == PlantTypeEnum.FastPlant)
            {
                PlantManager.Instance.HasBombBeenPlanted();
            }
            else if(RuntimeConfig.PlantType == PlantTypeEnum.AutoPlant)
            {
                PlantManager.Instance.HandlePlant();
            }

            

            return HookResult.Continue;
        }

        private HookResult OnBombBeginPlant(EventBombBeginplant @event, GameEventInfo info)
        {
            if(RuntimeConfig.PlantType == PlantTypeEnum.FastPlant)
            {
                PlantManager.Instance.HandlePlant();
            }

            if(RuntimeConfig.PlantType == PlantTypeEnum.AutoPlant)
            {
                return HookResult.Handled;
            }

            return HookResult.Continue;
        }


        private HookResult OnCsPreRestart(EventCsPreRestart @event, GameEventInfo info)
        {
            MapManager.Instance.ResetForNextRound();
            RetakeManager.Instance.ResetForNextRound();
            PlantManager.Instance.ResetForNextRound();

            return HookResult.Continue;
        }

        private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
        {
            WeaponManager.Instance.AssignWeapons();

            RetakeManager.Instance.AssignRandomPlayerInBombZoneAsPlanter();
            
            if(GameRuleManager.Instance.IsWarmup) 
            {
                return HookResult.Continue;
            }
        
            if (FeatureConfig.EnableSpotAnnouncer)
            {
                RetakeManager.Instance.PlaySpotAnnouncer();
            }

            var ratio = TeamManager.Instance.LatestRatio;

            MessageUtils.PrintToChatAll($"Bombsite: {ChatColors.DarkRed}{MapManager.Instance.BombSite}{ChatColors.White} - Roundtype: {ChatColors.DarkRed}{RoundTypeManager.Instance.RoundType}{ChatColors.White} - {ChatColors.Blue}{ratio.ctRatio}CTs{ChatColors.White} VS {ChatColors.Red}{ratio.tRatio}Ts{ChatColors.White}");

            return HookResult.Continue;
        }

        private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {   
            MapManager.Instance.ResetForNextRound();
            WeaponManager.Instance.ResetForNextRound();
            RetakeManager.Instance.ResetForNextRound();
            PlantManager.Instance.ResetForNextRound();

            if(GameRuleManager.Instance.IsWarmup)
            {
                return HookResult.Continue;
            }

            if (@event.Winner == (int)CsTeam.Terrorist)
            {
                MapManager.Instance.TerroristRoundWinStreak++;
                MessageUtils.PrintToChatAll($"The Terrorists have won {ChatColors.Darkred}{MapManager.Instance.TerroristRoundWinStreak}{ChatColors.White} rounds subsequently.");
                TeamManager.Instance.AddQueuePlayers();
            }
            else
            {
                MessageUtils.PrintToChatAll($"The Counter-Terrorists have won!");
                MapManager.Instance.TerroristRoundWinStreak = 0;
                TeamManager.Instance.SwitchTeams();
            }
             
            if(MapManager.Instance.TerroristRoundWinStreak == RuntimeConfig.ScrambleAfterSubsequentTerroristRoundWins)
            {
                MessageUtils.PrintToChatAll($"Teams will be scrambled now!");
                MapManager.Instance.TerroristRoundWinStreak = 0;
                TeamManager.Instance.ScrambleTeams();
            }

            return HookResult.Continue;
        }

        private HookResult OnPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
        {
            if (@event == null)
            {
                return HookResult.Continue;
            }

            @event.Silent = true;

            return HookResult.Continue;
        }

        private HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
        {
            if(@event.Userid == null) 
            {
                return HookResult.Continue;
            }
            if(!@event.Userid.IsValid)
            {
                return HookResult.Continue;
            }

            TeamManager.Instance.PlayerConnected(@event.Userid);

            return HookResult.Continue;
        }
        private HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
        {
            if (@event.Userid == null)
            {
                return HookResult.Continue;
            }
            if (!@event.Userid.IsValid)
            {
                return HookResult.Continue;
            }

            TeamManager.Instance.PlayerConnectedFull(@event.Userid);

            return HookResult.Continue;
        }
        private HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
        {
            if (@event.Userid == null)
            {
                return HookResult.Continue;
            }
            if (!@event.Userid.IsValid)
            {
                return HookResult.Continue;
            }

            TeamManager.Instance.PlayerDisconnected(@event.Userid);

            return HookResult.Continue;
        }

        private HookResult OnBeginNewMatch(EventBeginNewMatch @event, GameEventInfo info)
        {
            //SCRAMBLE AT START OF MATCH


            return HookResult.Continue;
        }

        private HookResult OnCsIntermission(EventCsIntermission @event, GameEventInfo info)
        {
            RoundTypeManager.Instance.ResetForNextMap();

            return HookResult.Continue;
        }

        public void OnMapStart(string mapName)
        {
            this.Logger?.LogInformation($"Map changed to {mapName}");
            MapManager.Instance.CurrentMap = new MapEntity(Server.MapName, this.ModuleDirectory);
            RetakeManager.Instance.ConfigureForRetake();
            GameRuleManager.Instance.GameRules = null;
            this._scrambleAfterWarmupDone = false;
            RoundTypeManager.Instance.ResetForNextMap();
        }


        public void OnTick()
        {
            if(GameRuleManager.Instance.IsWarmup && Server.CurrentTime >= GameRuleManager.Instance.WarmupEnd && !this._scrambleAfterWarmupDone)
            {
                this._scrambleAfterWarmupDone = true;
                TeamManager.Instance.ScrambleTeams();
            }

            TeamManager.Instance.OnTick();
        }

        private string PluginInfo()
        {
            return $"Plugin: {this.ModuleName} - Version: {this.ModuleVersion} by {this.ModuleAuthor}";
        }
    }
}