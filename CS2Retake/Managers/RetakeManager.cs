using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Managers.Base;
using CS2Retake.Managers.Interfaces;
using CS2Retake.Utils;
using Microsoft.Extensions.Logging;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace CS2Retake.Managers;

public class RetakeManager : BaseManager, IRetakeManager
{
    private static RetakeManager? _instance;

    private CCSPlayerController _planterPlayerController;

    public Timer? HasBombBeenPlantedTimer;

    public List<CCSPlayerController> PlayerJoinQueue = new();

    private RetakeManager()
    {
    }

    public bool BombHasBeenPlanted { get; set; }
    public CCSGameRules? GameRules { get; set; }
    public bool IsWarmup { get; set; }
    public float SecondsUntilBombPlantedCheck { get; set; } = 5.0f;

    public static RetakeManager Instance
    {
        get
        {
            if (_instance == null) _instance = new RetakeManager();
            return _instance;
        }
    }


    public void HasBombBeenPlanted()
    {
        if (SecondsUntilBombPlantedCheck <= 0 && Instance.IsWarmup) return;

        //Finding planted_c4 or weapon_c4
        var bombList = Utilities.FindAllEntitiesByDesignerName<CCSWeaponBase>("c4");

        if (!bombList.Any() && !IsWarmup && GetPlayerControllersOfTeam(CsTeam.Terrorist).Any())
        {
            MessageUtils.PrintToChatAll("No bomb was found in any players inventory resetting.");
            ScrambleTeams();
            GetPlayerControllers().ForEach(x => x?.PlayerPawn?.Value?.CommitSuicide(false, true));
            return;
        }

        HasBombBeenPlantedTimer = new Timer(SecondsUntilBombPlantedCheck, HasBombBeenPlantedCallback);
    }

    public void HasBombBeenPlantedCallback()
    {
        var plantedBomb = FindPlantedBomb();

        if (plantedBomb == null)
        {
            Server.PrintToChatAll(
                $"{MessageUtils.PluginPrefix} Player {ChatColors.Darkred}{_planterPlayerController?.PlayerName ?? "NOBODY"}{ChatColors.White} failed to plant the bomb in time. Counter-Terrorists win this round.");

            var terroristPlayerList = GetPlayerControllers().Where(x =>
                x != null && x is { IsValid: true, PlayerPawn.IsValid: true } && x.PlayerPawn.Value != null &&
                x.PlayerPawn.Value.IsValid && x.TeamNum == (int)CsTeam.Terrorist).ToList();
            terroristPlayerList.ForEach(x => x?.PlayerPawn?.Value?.CommitSuicide(true, true));
        }
    }

    /*
     * TODO:
     *
     * ScrambleTeams, SwitchTeams & AddQueuePlayers has an issue with reconnecting players because the server still thinks they are on the server
     * Thus leading to issues with the algorithm. (Scrambling and team switching algorithm seems to be working if no player disconnects and reconnects).
     *
     * Possible solutions:
     *      - solve the issue by implementing a check if the user is a active player
     *      - implement a list with a player status (connecting, queue, playing, spectator, disconnected, ...)
     *
     */

    public void ScrambleTeams()
    {
        MessageUtils.Log(LogLevel.Debug, "ScrambleTeams");

        var nonSpectatingValidPlayers = GetPlayerControllers().Where(x =>
            x.IsValid && x.PlayerPawn.IsValid && x.PlayerPawn.Value.IsValid &&
            x.TeamNum is (int)CsTeam.Terrorist or (int)CsTeam.CounterTerrorist).ToList();

        if (!nonSpectatingValidPlayers.Any())
        {
            MessageUtils.Log(LogLevel.Error, "No valid non spectating players have been found!");
            return;
        }

        var random = new Random();
        nonSpectatingValidPlayers = nonSpectatingValidPlayers.OrderBy(x => random.Next()).ToList();

        for (var i = 0; i < nonSpectatingValidPlayers.Count; i++)
            nonSpectatingValidPlayers[i].SwitchTeam(i % 2 == 0 ? CsTeam.CounterTerrorist : CsTeam.Terrorist);
    }

    public void SwitchTeams()
    {
        MessageUtils.Log(LogLevel.Debug, "SwitchTeams");

        var playersOnServer = GetPlayerControllers()
            .Where(x => x.IsValid && x.PlayerPawn.IsValid && x.PlayerPawn.Value.IsValid).ToList();

        var terroristPlayers = playersOnServer.Where(x => x.TeamNum == (int)CsTeam.Terrorist).ToList();
        var counterTerroristPlayers = playersOnServer.Where(x => x.TeamNum == (int)CsTeam.CounterTerrorist).ToList();

        var playersInQueue = PlayerJoinQueue.Count();

        var activePlayerCount = playersInQueue + terroristPlayers.Count + counterTerroristPlayers.Count;

        var playersNeededInCT = (int)Math.Ceiling((decimal)activePlayerCount / 2);

        var random = new Random();

        PlayerJoinQueue.ForEach(player => player.SwitchTeam(CsTeam.CounterTerrorist));

        playersNeededInCT = playersNeededInCT - PlayerJoinQueue.Count();

        var counterTerroristsToSwitch =
            counterTerroristPlayers.OrderBy(x => random.Next()).Take(terroristPlayers.Count).ToList();
        var terroristsToSwitch = terroristPlayers.OrderBy(x => random.Next()).Take(playersNeededInCT).ToList();

        terroristsToSwitch.ForEach(x => x.SwitchTeam(CsTeam.CounterTerrorist));
        counterTerroristsToSwitch.ForEach(x => x.SwitchTeam(CsTeam.Terrorist));

        PlayerJoinQueue.Clear();
    }

    public void AddQueuedPlayersAndRebalance()
    {
        MessageUtils.Log(LogLevel.Debug, "AddQueuedPlayers");

        if (!PlayerJoinQueue.Any()) return;

        var playersOnServer = GetPlayerControllers()
            .Where(x => x.IsValid && x.PlayerPawn.IsValid && x.PlayerPawn.Value.IsValid).ToList();

        var terroristPlayers = playersOnServer.Where(x => x.TeamNum == (int)CsTeam.Terrorist).ToList();
        var counterTerroristPlayers = playersOnServer.Where(x => x.TeamNum == (int)CsTeam.CounterTerrorist).ToList();

        var playersInQueue = PlayerJoinQueue.Count();

        var activePlayerCount = playersInQueue + terroristPlayers.Count + counterTerroristPlayers.Count;

        var playersNeededInCT = (int)Math.Ceiling((decimal)activePlayerCount / 2);

        var random = new Random();

        PlayerJoinQueue.ForEach(player => player.SwitchTeam(CsTeam.CounterTerrorist));

        var ctCount = counterTerroristPlayers.Count() + playersInQueue;

        var counterTerroristsToSwitch = counterTerroristPlayers.OrderBy(x => random.Next())
            .Take(ctCount - playersNeededInCT).ToList();

        counterTerroristsToSwitch.ForEach(x => x.SwitchTeam(CsTeam.Terrorist));

        if (terroristPlayers.Count > ctCount)
        {
            //REBALANCE IF TERRORISTS HAS MORE PLAYERS THEN COUNTER TERRORISTS
        }

        PlayerJoinQueue.Clear();
    }

    public void GiveBombToPlayerRandomPlayerInBombZone()
    {
        var random = new Random();
        var plantSpawn = MapManager.Instance.CurrentMap.SpawnPoints
            .Where(spawn => spawn.SpawnUsedBy != null && spawn.IsInBombZone).OrderBy(x => random.Next())
            .FirstOrDefault();


        if (plantSpawn == null)
        {
            MessageUtils.Log(LogLevel.Warning,
                "No valid plant spawn found! This might be because no player is on terrorist team.");
            return;
        }

        if (plantSpawn.SpawnUsedBy == null)
        {
            MessageUtils.Log(LogLevel.Error, "Spawn is not used by any player");
            return;
        }

        _planterPlayerController = plantSpawn.SpawnUsedBy;

        if (_planterPlayerController == null)
        {
            MessageUtils.Log(LogLevel.Error, "Player that uses the valid plant spawn is null");
            return;
        }

        _planterPlayerController.GiveNamedItem("weapon_c4");

        if (SecondsUntilBombPlantedCheck > 0)
            _planterPlayerController.PrintToCenter(
                $"YOU HAVE {ChatColors.Darkred}{SecondsUntilBombPlantedCheck}{ChatColors.White} SECONDS TO PLANT THE BOMB!");


        //_ = new CounterStrikeSharp.API.Modules.Timers.Timer(seconds, this.HasBombBeenPlantedCallback);

        //c4.BombPlacedAnimation = false;


        //var plantedBomb = this.FindPlantedBomb();

        //if(plantedBomb == null)
        //{
        //    MessageUtils.Log(LogLevel.Warning,$"No planted bomb was found!");
        //    return;
        //}

        //var playerPawn = player.PlayerPawn.Value;

        //if(playerPawn == null)
        //{
        //    MessageUtils.Log(LogLevel.Warning,$"Player pawn is null");
        //    return;
        //}
        //if(playerPawn.AbsRotation == null)
        //{
        //    MessageUtils.Log(LogLevel.Warning,$"Player pawn rotation is null");
        //    return;
        //}
        //if(playerPawn.AbsOrigin == null)
        //{
        //    MessageUtils.Log(LogLevel.Warning,$"Player pawn position is null");
        //    return;
        //}


        //plantedBomb.Teleport(playerPawn.AbsOrigin, playerPawn.AbsRotation, new Vector(0f, 0f, 0f));

        //this.ModifyGameRulesBombPlanted(true);

        //plantedBomb.BombTicking = true;
        ////plantedBomb.BombSite = 168 + (int)MapManager.Instance.BombSite;


        //var bombTarget = this.GetBombTarget();

        //if (bombTarget == null)
        //{
        //    return;
        //}

        //bombTarget.BombPlantedHere = true;


        //var bombPlantedEventPtr = NativeAPI.CreateEvent("bomb_planted", false);
        //NativeAPI.SetEventPlayerController(bombPlantedEventPtr, "userid", player.Handle);
        //NativeAPI.SetEventInt(bombPlantedEventPtr, "site", 0);
        ////NativeAPI.SetEventEntity(bombPlantedEventPtr, "userid_pawn", player.PlayerPawn.Value.Handle);
        //NativeAPI.FireEvent(bombPlantedEventPtr, false);
    }

    public void FastPlantBomb()
    {
        var c4list = Utilities.FindAllEntitiesByDesignerName<CC4>("weapon_c4");


        if (!c4list.Any()) return;

        var c4 = c4list.FirstOrDefault();
        if (c4 == null) return;

        c4.BombPlacedAnimation = false;
        c4.ArmedTime = 0f;

        //c4.IsPlantingViaUse = true;
        //c4.StartedArming = true;
        //c4.BombPlanted = true;
    }

    public void PlaySpotAnnouncer()
    {
        var bombsite = MapManager.Instance.BombSite;

        foreach (var player in GetPlayerControllers().FindAll(x => x.TeamNum == (int)CsTeam.CounterTerrorist))
            player.ExecuteClientCommand($"play sounds/vo/agents/seal_epic/loc_{bombsite.ToString().ToLower()}_01");
    }

    public void ConfigureForRetake()
    {
        Server.ExecuteCommand("execifexists cs2retake/retake.cfg");

        IsWarmup = true;
    }


    private void ModifyGameRulesBombPlanted(bool bombPlanted)
    {
        if (GameRules == null)
        {
            MessageUtils.Log(LogLevel.Information, "GameRules is null. Fetching gamerule...");

            var gameRuleProxyList = GetGameRulesProxies();

            if (gameRuleProxyList.Count > 1)
                MessageUtils.Log(LogLevel.Error, "Multiple GameRuleProxies found. Using firstOrDefault");

            var gameRuleProxy = gameRuleProxyList.FirstOrDefault();

            if (gameRuleProxy == null)
            {
                MessageUtils.Log(LogLevel.Error, "GameRuleProxy is null");
                return;
            }

            if (gameRuleProxy.GameRules == null)
            {
                MessageUtils.Log(LogLevel.Error, "GameRules is null");
                return;
            }

            GameRules = gameRuleProxy.GameRules;
        }

        GameRules.BombPlanted = bombPlanted;
    }

    private List<CCSGameRulesProxy> GetGameRulesProxies()
    {
        var gameRulesProxyList = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").ToList();

        if (!gameRulesProxyList.Any()) MessageUtils.Log(LogLevel.Error, "No gameRuleProxy found!");

        return gameRulesProxyList;
    }

    private List<CCSPlayerController> GetPlayerControllers()
    {
        var playerList = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller").ToList();

        if (!playerList.Any()) MessageUtils.Log(LogLevel.Error, "No Players have been found!");

        return playerList;
    }

    private List<CCSPlayerController> GetPlayerControllersOfTeam(CsTeam team)
    {
        var playerList = GetPlayerControllers();

        playerList = playerList.FindAll(x =>
            x != null && x.IsValid && x.PlayerPawn != null && x.PlayerPawn.IsValid && x.PlayerPawn.Value != null &&
            x.PlayerPawn.Value.IsValid);

        playerList = playerList.FindAll(x => x.TeamNum == (int)team);

        return playerList;
    }

    private CPlantedC4? FindPlantedBomb()
    {
        var plantedBombList = Utilities.FindAllEntitiesByDesignerName<CPlantedC4>("planted_c4");

        if (!plantedBombList.Any())
        {
            MessageUtils.Log(LogLevel.Warning,
                "No planted bomb entities have been found! This might be because no bomb was planted.");
            return null;
        }

        return plantedBombList.FirstOrDefault();
    }

    public override void ResetForNextRound(bool completeReset = true)
    {
        if (completeReset)
            if (HasBombBeenPlantedTimer != null)
                HasBombBeenPlantedTimer?.Kill();

        BombHasBeenPlanted = false;
    }
}