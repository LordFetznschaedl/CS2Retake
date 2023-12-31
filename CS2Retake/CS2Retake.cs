using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Configs;
using CS2Retake.Entities;
using CS2Retake.Managers;
using CS2Retake.Utils;
using Microsoft.Extensions.Logging;

namespace CS2Retake;

[MinimumApiVersion(129)]
public class CS2Retake : BasePlugin, IPluginConfig<CS2RetakeConfig>
{
    public override string ModuleName => "CS2Retake";
    public override string ModuleVersion => "1.0.5-alpha";
    public override string ModuleAuthor => "LordFetznschaedl";
    public override string ModuleDescription => "Retake Plugin implementation for CS2";

    public CS2RetakeConfig Config { get; set; } = new();

    public void OnConfigParsed(CS2RetakeConfig config)
    {
        if (config.Version < Config.Version)
            Logger?.LogWarning(
                $"The plugin configuration is out of date. Consider updating the config. [Current Version: {config.Version} - Plugin Version: {Config.Version}]");
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        Logger?.LogInformation(PluginInfo());
        Logger?.LogInformation(ModuleDescription);

        MessageUtils.ModuleName = ModuleName;
        MessageUtils.Logger = Logger;
        WeaponManager.Instance.ModuleDirectory = ModuleDirectory;
        RetakeManager.Instance.SecondsUntilBombPlantedCheck = Config.SecondsUntilBombPlantedCheck;

        if (MapManager.Instance.CurrentMap == null) OnMapStart(Server.MapName);

        // this.AddTimer(7 * 60, MessageUtils.ThankYouMessage, TimerFlags.REPEAT);

        RegisterListener<Listeners.OnMapStart>(mapName => OnMapStart(mapName));

        RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);
        RegisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
        RegisterEventHandler<EventCsPreRestart>(OnCsPreRestart);
        RegisterEventHandler<EventBombBeginplant>(OnBombBeginPlant);
        RegisterEventHandler<EventPlayerTeam>(OnPlayerTeam, HookMode.Pre);
        RegisterEventHandler<EventBeginNewMatch>(OnBeginNewMatch, HookMode.Pre);
        RegisterEventHandler<EventCsIntermission>(OnCsIntermission);
        RegisterEventHandler<EventRoundStart>(OnRoundStart);


        AddCommandListener("jointeam", OnCommandJoinTeam);
    }

    [ConsoleCommand("css_retakeinfo", "This command prints the plugin information")]
    public void OnCommandInfo(CCSPlayerController? player, CommandInfo command)
    {
        command.ReplyToCommand($"{MessageUtils.PluginPrefix} {PluginInfo()}");
    }

    [ConsoleCommand("css_retakespawn", "This command teleports the player to a spawn with the given index in the args")]
    [RequiresPermissions("@cs2retake/admin")]
    public void OnCommandSpawn(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
        {
            Logger?.LogError("Command has been called by the server.");
            return;
        }

        if (!player.PlayerPawn.IsValid)
        {
            Logger?.LogError("PlayerPawn not valid");
            return;
        }

        if (command.ArgCount != 2)
        {
            Logger?.LogError($"ArgCount: {command.ArgCount} - ArgString: {command.ArgString}");
            command.ReplyToCommand(
                $"{MessageUtils.PluginPrefix} One argument with a valid spawn index is needed! Example: !retakespawn <index (int)>");
            return;
        }

        if (!int.TryParse(command.ArgByIndex(1), out var spawnIndex))
        {
            Logger?.LogError("Argument index not a valid integer!");
            return;
        }

        MapManager.Instance.TeleportPlayerToSpawn(player, spawnIndex);
    }

    [ConsoleCommand("css_retakewrite", "This command writes the spawns for the current map")]
    [RequiresPermissions("@cs2retake/admin")]
    public void OnCommandWrite(CCSPlayerController? player, CommandInfo command)
    {
        MapManager.Instance.CurrentMap.SaveSpawns();
        Logger?.LogInformation($"{MapManager.Instance.CurrentMap.SpawnPoints.Count} spawnpoints saved");
    }

    [ConsoleCommand("css_retakeread", "This command reads the spawns for the current map")]
    [RequiresPermissions("@cs2retake/admin")]
    public void OnCommandRead(CCSPlayerController? player, CommandInfo command)
    {
        MapManager.Instance.CurrentMap.LoadSpawns();
        Logger?.LogInformation($"{MapManager.Instance.CurrentMap.SpawnPoints.Count} spawnpoints read");
    }

    [ConsoleCommand("css_retakescramble", "This command scrambles the teams")]
    [RequiresPermissions("@cs2retake/admin")]
    public void OnCommandScramble(CCSPlayerController? player, CommandInfo command)
    {
        RetakeManager.Instance.ScrambleTeams();
    }

    [ConsoleCommand("css_retaketeleport", "This command teleports the player to the given coordinates")]
    [RequiresPermissions("@cs2retake/admin")]
    public void OnCommandTeleport(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
        {
            Logger?.LogError("Command has been called by the server.");
            return;
        }

        if (!player.PlayerPawn.IsValid)
        {
            Logger?.LogError("PlayerPawn not valid");
            return;
        }

        if (command.ArgCount != 4)
        {
            Logger?.LogError($"ArgCount: {command.ArgCount} - ArgString: {command.ArgString}");
            command.ReplyToCommand(
                $"{MessageUtils.PluginPrefix} Command format: !retaketeleport <position X float> <position Y float> <position Z float>");
            return;
        }

        if (!float.TryParse(command.ArgByIndex(1), out var positionX))
        {
            Logger?.LogError("Argument position X not a valid float!");
            return;
        }

        if (!float.TryParse(command.ArgByIndex(2), out var positionY))
        {
            Logger?.LogError("Argument position Y not a valid float!");
            return;
        }

        if (!float.TryParse(command.ArgByIndex(3), out var positionZ))
        {
            Logger?.LogError("Argument position Z not a valid float!");
            return;
        }

        player?.PlayerPawn?.Value?.Teleport(new Vector(positionX, positionY, positionZ), new QAngle(0f, 0f, 0f),
            new Vector(0f, 0f, 0f));
    }

    [ConsoleCommand("css_retakeaddspawn", "This command adds a new spawn to the current map")]
    [RequiresPermissions("@cs2retake/admin")]
    public void OnCommandAdd(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null)
        {
            Logger?.LogError("Command has been called by the server.");
            return;
        }

        if (!player.PlayerPawn.IsValid)
        {
            Logger?.LogError("PlayerPawn not valid");
            return;
        }

        if (command.ArgCount != 3)
        {
            Logger?.LogError($"ArgCount: {command.ArgCount} - ArgString: {command.ArgString}");
            command.ReplyToCommand(
                $"{MessageUtils.PluginPrefix} Command format: !retakeaddspawn <2/3 - 2 = T; 3 = CT> <0/1 - 0 = A; 1 = B>");
            return;
        }

        if (!int.TryParse(command.ArgByIndex(1), out var team))
        {
            Logger?.LogError("Team could not be parsed!");
            return;
        }

        if (team != 2 && team != 3)
        {
            Logger?.LogError("Team index is not in 2 or 3");
            return;
        }

        if (!int.TryParse(command.ArgByIndex(2), out var bombSite))
        {
            Logger?.LogError("Team could not be parsed!");
            return;
        }

        if (bombSite != 0 && bombSite != 1)
        {
            Logger?.LogError("BombSite index is not in 0 or 1");
            return;
        }

        MapManager.Instance.AddSpawn(player, (CsTeam)team, (BombSiteEnum)bombSite);
    }


    private HookResult OnCommandJoinTeam(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (RetakeManager.Instance.IsWarmup) return HookResult.Continue;

        if (player == null || !player.IsValid)
        {
            Logger?.LogError("Player is null or not valid");
            return HookResult.Handled;
        }

        var oldTeam = (CsTeam)player.TeamNum;

        if (commandInfo.ArgCount < 2)
        {
            Logger?.LogError("Wrong amount of arguments for JoinTeam");
            return HookResult.Handled;
        }

        if (!Enum.TryParse(commandInfo.GetArg(1), out CsTeam newTeam))
        {
            Logger?.LogError("Parsing new team failed");
            return HookResult.Handled;
        }

        Logger?.LogDebug($"From {oldTeam} To {newTeam}");

        if (oldTeam == newTeam && oldTeam != CsTeam.None)
        {
            Logger?.LogDebug("Old Team is new team");
            return HookResult.Continue;
        }

        if ((oldTeam == CsTeam.CounterTerrorist && newTeam == CsTeam.Terrorist) ||
            (oldTeam == CsTeam.Terrorist && newTeam == CsTeam.CounterTerrorist))
        {
            Logger?.LogDebug("team switch");
            return HookResult.Continue;
        }

        if (newTeam == CsTeam.Spectator)
        {
            MessageUtils.PrintToPlayerOrServer("You have been removed from the queue.");
            RetakeManager.Instance.PlayerJoinQueue.RemoveAll(x => x.SteamID == player.SteamID);

            return HookResult.Continue;
        }

        if (!RetakeManager.Instance.PlayerJoinQueue.Any(x => x.SteamID == player.SteamID))
        {
            MessageUtils.PrintToPlayerOrServer(
                "You have been placed into the queue. Please wait for the next round to start.");
            RetakeManager.Instance.PlayerJoinQueue.Add(player);
        }

        return HookResult.Handled;
    }

    public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
        if (@event.Userid == null || !@event.Userid.IsValid) return HookResult.Continue;
        if (@event.Userid.PlayerPawn == null || !@event.Userid.PlayerPawn.IsValid) return HookResult.Continue;
        if (@event.Userid.PlayerPawn.Value == null || !@event.Userid.PlayerPawn.Value.IsValid)
            return HookResult.Continue;

        MapManager.Instance.TeleportPlayerToSpawn(@event.Userid);

        return HookResult.Continue;
    }

    private HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
    {
        RetakeManager.Instance.HasBombBeenPlanted();

        return HookResult.Continue;
    }

    private HookResult OnBombBeginPlant(EventBombBeginplant @event, GameEventInfo info)
    {
        RetakeManager.Instance.FastPlantBomb();

        return HookResult.Continue;
    }

    private HookResult OnBombPickup(EventBombPickup @event, GameEventInfo info)
    {
        var pBombCarrierController = GetBombCarrier();

        if (pBombCarrierController == null) return HookResult.Continue;

        if (!pBombCarrierController.PlayerPawn.Value!.InBombZone) return HookResult.Continue;

        pBombCarrierController.ExecuteClientCommand("slot5");

        return HookResult.Continue;
    }

    public CCSPlayerController? GetBombCarrier()
    {
        CCSPlayerController? foundPlayer = null;

        foreach (var player in Utilities.GetPlayers())
        {
            if (player.PlayerPawn.Value!.WeaponServices == null) continue;

            foreach (var weapon in player.PlayerPawn.Value.WeaponServices.MyWeapons)
            {
                if (weapon.Value == null) continue;
                if (weapon.Value.DesignerName != "weapon_c4") continue;

                foundPlayer = player;
                break;
            }
        }

        return foundPlayer;
    }


    private HookResult OnCsPreRestart(EventCsPreRestart @event, GameEventInfo info)
    {
        MapManager.Instance.ResetForNextRound(false);
        RetakeManager.Instance.ResetForNextRound();

        MessageUtils.PrintToChatAll(
            $"Bombsite: {ChatColors.Darkred}{MapManager.Instance.BombSite}{ChatColors.White} - Roundtype: {ChatColors.Darkred}{WeaponManager.Instance.RoundType}{ChatColors.White}");


        return HookResult.Continue;
    }

    private HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        WeaponManager.Instance.AssignWeapons();
        RetakeManager.Instance.GiveBombToPlayerRandomPlayerInBombZone();

        var pBombCarrierController = GetBombCarrier();

        if (pBombCarrierController == null) return HookResult.Continue;

        pBombCarrierController.ExecuteClientCommand("slot5");

        if (Config.SpotAnnouncerEnabled) RetakeManager.Instance.PlaySpotAnnouncer();

        return HookResult.Continue;
    }

    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (@event.Winner == (int)CsTeam.Terrorist)
        {
            MapManager.Instance.TerroristRoundWinStreak++;
            MessageUtils.PrintToChatAll(
                $"The Terrorists have won {ChatColors.Darkred}{MapManager.Instance.TerroristRoundWinStreak}{ChatColors.White} rounds subsequently.");
            RetakeManager.Instance.AddQueuedPlayersAndRebalance();
        }
        else
        {
            MessageUtils.PrintToChatAll("The Counter-Terrorists have won!");
            MapManager.Instance.TerroristRoundWinStreak = 0;
            RetakeManager.Instance.SwitchTeams();
        }

        if (MapManager.Instance.TerroristRoundWinStreak == 5)
        {
            MessageUtils.PrintToChatAll("Teams will be scrambled now!");
            MapManager.Instance.TerroristRoundWinStreak = 0;
            RetakeManager.Instance.ScrambleTeams();
        }

        MapManager.Instance.ResetForNextRound();
        WeaponManager.Instance.ResetForNextRound();
        RetakeManager.Instance.ResetForNextRound();

        return HookResult.Continue;
    }

    private HookResult OnPlayerTeam(EventPlayerTeam @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        @event.Silent = true;

        return HookResult.Continue;
    }

    private HookResult OnBeginNewMatch(EventBeginNewMatch @event, GameEventInfo info)
    {
        RetakeManager.Instance.IsWarmup = false;

        //SCRAMBLE AT START OF MATCH

        return HookResult.Continue;
    }

    private HookResult OnCsIntermission(EventCsIntermission @event, GameEventInfo info)
    {
        Logger?.LogDebug("OnCsIntermission");

        RetakeManager.Instance.IsWarmup = true;

        return HookResult.Continue;
    }

    public void OnMapStart(string mapName)
    {
        Logger?.LogInformation($"Map changed to {mapName}");
        MapManager.Instance.CurrentMap = new MapEntity(Server.MapName, ModuleDirectory);
        RetakeManager.Instance.ConfigureForRetake();
    }

    private string PluginInfo()
    {
        return $"Plugin: {ModuleName} - Version: {ModuleVersion} by {ModuleAuthor}";
    }
}