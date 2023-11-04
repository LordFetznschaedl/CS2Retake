using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using CS2Retake.Entity;

namespace CS2Retake
{
    public class CS2Retake : BasePlugin
    {
        public override string ModuleName => "CS2Retake";
        public override string ModuleVersion => "0.0.1";
        public override string ModuleAuthor => "LordFetznschaedl";
        public override string ModuleDescription => "Retake Plugin implementation for CS2";

        private MapEntity _currentMap { get; set; } = null;
       

        public override void Load(bool hotReload)
        {
            this.Log(PluginInfo());
            this.Log(this.ModuleDescription);

            this.RegisterEventHandler<EventPlayerSpawn>(OnPlayerSpawn);

            if(this._currentMap == null)
            {
                this._currentMap = new MapEntity(Server.MapName, this.ModuleDirectory);
            }

            this.RegisterListener<Listeners.OnMapStart>((mapName) =>
            {
                this._currentMap = new MapEntity(mapName, this.ModuleDirectory);
            });
        }

        [ConsoleCommand("css_retakeinfo", "This command prints the plugin information")]
        public void OnCommandInfo(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null)
            {
                Console.WriteLine("Command has been called by the server.");
                return;
            }

            player.PrintToChat(PluginInfo());
            player.PrintToConsole(PluginInfo());
        }

        [ConsoleCommand("css_retakewrite", "This command writes the spawns")]
        public void OnCommandWrite(CCSPlayerController? player, CommandInfo command)
        {
            this._currentMap.WriteSpawns();
        }

        [GameEventHandler]
        public HookResult OnPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
        {
            return HookResult.Continue;

            if (@event == null)
            {
                return HookResult.Continue;
            }
            if(@event.Userid.IsValid)
            {
                return HookResult.Continue;
            }


            var spawnPoint = this._currentMap.GetRandomSpawn((CsTeam)@event.Userid.TeamNum);

            var absOrigin = @event.Userid.CBodyComponent!.SceneNode!.AbsOrigin;
            absOrigin.X = 0;
            absOrigin.Y = 0;
            absOrigin.Z = 0;

            var absRotation = @event.Userid.CBodyComponent!.SceneNode!.AbsRotation;
            absRotation.X = 0;
            absRotation.Y = 0;
            absRotation.Z = 0;

            //@event.Userid.Teleport();

            this.Log(@event.Userid.TeamNum.ToString());

            return HookResult.Continue;
        }

        private string PluginInfo()
        {
            return $"Plugin: {ModuleName} - Version: {ModuleVersion}";
        }

        private void Log(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}