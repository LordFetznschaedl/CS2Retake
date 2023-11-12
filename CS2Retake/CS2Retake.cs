using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Entity;
using CS2Retake.Manager;
using CS2Retake.Utils;
using System.Security.Cryptography.X509Certificates;

namespace CS2Retake
{
    public class CS2Retake : BasePlugin  
    {
        public override string ModuleName => "CS2Retake";
        public override string ModuleVersion => "1.0.0";
        public override string ModuleAuthor => "LordFetznschaedl";
        public override string ModuleDescription => "Retake Plugin implementation for CS2";

        public override void Load(bool hotReload)
        {
            this.Log(PluginInfo());
            this.Log(this.ModuleDescription);

            RetakeManager.GetInstance().ModuleName= this.ModuleName;

            MapManager.GetInstance().ModuleName = this.ModuleName;

            if (MapManager.GetInstance().CurrentMap == null)
            {
                this.OnMapStart(Server.MapName);
            }

            this.RegisterListener<Listeners.OnMapStart>(mapname => this.OnMapStart(mapname));

            this.RegisterEventHandler<EventRoundFreezeEnd>(OnRoundFreezeEnd);
            this.RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
            this.RegisterEventHandler<EventCsPreRestart>(OnCsPreRestart);
        }


        [ConsoleCommand("css_retakeinfo", "This command prints the plugin information")]
        public void OnCommandInfo(CCSPlayerController? player, CommandInfo command)
        {
            command.ReplyToCommand(PluginInfo());
        }

        [ConsoleCommand("css_retakespawn", "This command teleports the player to a spawn with the given index in the args")]
        [RequiresPermissions("@cs2retake/admin", "@cs2retake/editor")]
        public void OnCommandSpawn(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null)
            {
                this.Log("Command has been called by the server.");
                return;
            }
            if(!player.PlayerPawn.IsValid)
            {
                this.Log("PlayerPawn not valid");
                return;
            }

            if (command.ArgCount != 2)
            {
                this.Log($"ArgCount: {command.ArgCount} - ArgString: {command.ArgString}");
                command.ReplyToCommand($"One argument with a valid spawn index is needed! Example: !retakespawn 0");
                return;
            }

            if(!int.TryParse(command.ArgByIndex(1), out int spawnIndex))
            {
                this.Log("Argument index not a valid integer!");
                return;
            }

            MapManager.GetInstance().TeleportPlayerToSpawn(player, BombSiteEnum.Undefined ,spawnIndex);
        }

        [ConsoleCommand("css_retakewrite", "This command writes the spawns for the current map")]
        [RequiresPermissions("@cs2retake/admin", "@cs2retake/editor")]
        public void OnCommandWrite(CCSPlayerController? player, CommandInfo command)
        {
            MapManager.GetInstance().CurrentMap.WriteSpawns();
        }

        [ConsoleCommand("css_retakeread", "This command reads the spawns for the current map")]
        [RequiresPermissions("@cs2retake/admin", "@cs2retake/editor")]
        public void OnCommandRead(CCSPlayerController? player, CommandInfo command)
        {
            MapManager.GetInstance().CurrentMap.ReadSpawns();
            this.Log($"{MapManager.GetInstance().CurrentMap.SpawnPoints.Count} spawnpoints read");
        }

        [ConsoleCommand("css_retakescramble", "This command scrambles the teams")]
        [RequiresPermissions("@css/generic", "@cs2retake/admin")]
        public void OnCommandScramble(CCSPlayerController? player, CommandInfo command)
        {
            RetakeManager.GetInstance().ScrambleTeams();
        }

        [ConsoleCommand("css_retaketeleport", "This command teleports the player to the given coordinates")]
        [RequiresPermissions("@cs2retake/admin", "@cs2retake/editor")]
        public void OnCommandTeleport(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null)
            {
                this.Log("Command has been called by the server.");
                return;
            }
            if (!player.PlayerPawn.IsValid)
            {
                this.Log("PlayerPawn not valid");
                return;
            }

            if (command.ArgCount != 4)
            {
                this.Log($"ArgCount: {command.ArgCount} - ArgString: {command.ArgString}");
                command.ReplyToCommand($"Command format: !retaketeleport <position X float> <position Y float> <position Z float>");
                return;
            }

            if (!float.TryParse(command.ArgByIndex(1), out float positionX))
            {
                this.Log("Argument position X not a valid float!");
                return;
            }

            if (!float.TryParse(command.ArgByIndex(2), out float positionY))
            {
                this.Log("Argument position Y not a valid float!");
                return;
            }

            if (!float.TryParse(command.ArgByIndex(3), out float positionZ))
            {
                this.Log("Argument position Z not a valid float!");
                return;
            }

            player.PlayerPawn.Value.Teleport(new Vector(positionX, positionY, positionZ), new QAngle(0f,0f,0f), new Vector(0f, 0f, 0f));
        }

        [ConsoleCommand("css_retakeaddspawn", "This command adds a new spawn to the current map")]
        [RequiresPermissions("@cs2retake/admin", "@cs2retake/editor")]
        public void OnCommandAdd(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null)
            {
                this.Log("Command has been called by the server.");
                return;
            }
            if (!player.PlayerPawn.IsValid)
            {
                this.Log("PlayerPawn not valid");
                return;
            }

            if (command.ArgCount != 3)
            {
                this.Log($"ArgCount: {command.ArgCount} - ArgString: {command.ArgString}");
                command.ReplyToCommand($"Command format: !retakeaddspawn <2/3 - 2 = T; 3 = CT> <0/1 - 0 = A; 1 = B>");
                return;
            }

            if (!int.TryParse(command.ArgByIndex(1), out int team))
            {
                this.Log("Team could not be parsed!");
                return;
            }

            if(team != 2 && team != 3) 
            {
                this.Log("Team index is not in 2 or 3");
                return;
            }

            if (!int.TryParse(command.ArgByIndex(2), out int bombSite))
            {
                this.Log("Team could not be parsed!");
                return;
            }

            if (bombSite != 0 && bombSite != 1)
            {
                this.Log("BombSite index is not in 0 or 1");
                return;
            }

            MapManager.GetInstance().AddSpawn(player, (CsTeam)team, (BombSiteEnum)bombSite);
        }

        [GameEventHandler]
        public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            if (MapManager.GetInstance().BombSite == BombSiteEnum.Undefined) 
            { 
                MapManager.GetInstance().RandomBombSite();
            }

            if (@event == null)
            {
                return HookResult.Continue;
            }
            if(!@event.Userid.IsValid)
            {
                return HookResult.Continue;
            }

            MapManager.GetInstance().TeleportPlayerToSpawn(@event.Userid, MapManager.GetInstance().BombSite);

            return HookResult.Continue;
        }

        [GameEventHandler]
        private HookResult OnRoundFreezeEnd(EventRoundFreezeEnd @event, GameEventInfo info)
        {
            RetakeManager.GetInstance().PlantBomb();

            return HookResult.Continue;
        }

        [GameEventHandler]
        private HookResult OnCsPreRestart(EventCsPreRestart @event, GameEventInfo info)
        {
            MapManager.GetInstance().ResetForNextRound(false);

            return HookResult.Continue;
        }

        [GameEventHandler]
        private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
        {
            if (@event.Winner == (int)CsTeam.Terrorist)
            {
                MapManager.GetInstance().TerroristRoundWinStreak++;
                Server.PrintToChatAll($"[{ChatColors.Gold}CS2Retake{ChatColors.White}] The Terrorists have won {ChatColors.Red}{MapManager.GetInstance().TerroristRoundWinStreak}{ChatColors.White} rounds subsequently.");
            }
            else
            {
                MapManager.GetInstance().TerroristRoundWinStreak = 0;
            }

            if(MapManager.GetInstance().TerroristRoundWinStreak == 5)
            {
                Server.PrintToChatAll($"[{ChatColors.Gold}CS2Retake{ChatColors.White}] Teams will be scrambled now!");
                MapManager.GetInstance().TerroristRoundWinStreak = 0;
                RetakeManager.GetInstance().ScrambleTeams();
            }

            MapManager.GetInstance().ResetForNextRound();

            return HookResult.Continue;
        }

        public void OnMapStart(string mapName)
        {
            this.Log($"Map changed to {mapName}");
            MapManager.GetInstance().CurrentMap = new MapEntity(Server.MapName, this.ModuleDirectory, this.ModuleName);
            RetakeManager.GetInstance().ConfigureForRetake();
        }

        private string PluginInfo()
        {
            return $"Plugin: {this.ModuleName} - Version: {this.ModuleVersion} by {this.ModuleAuthor}";
        }

        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{this.ModuleName}] {message}");
            Console.ResetColor();
        }
    }
}