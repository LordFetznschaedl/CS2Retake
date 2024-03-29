﻿using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Utils
{
    public static class PlayerUtils
    {
        public static List<CCSPlayerController> GetPlayerControllersOfTeam(CsTeam team)
        {
            var playerList = Utilities.GetPlayers();

            //Valid players
            playerList = playerList.FindAll(x => x != null && x.IsValid && x.PlayerPawn != null && x.PlayerPawn.IsValid && x.PlayerPawn.Value != null && x.PlayerPawn.Value.IsValid);

            //Team specific players
            playerList = playerList.FindAll(x => x.TeamNum == (int)team);

            return playerList ?? new List<CCSPlayerController>();
        }

        public static List<CCSPlayerController> GetCounterTerroristPlayers() => GetPlayerControllersOfTeam(CsTeam.CounterTerrorist);
        public static List<CCSPlayerController> GetTerroristPlayers() => GetPlayerControllersOfTeam(CsTeam.Terrorist);

        public static List<CCSPlayerController> GetValidPlayerControllers() => Utilities.GetPlayers().Where(player => player.PlayerPawn != null && player.PlayerPawn.IsValid && player.PlayerPawn.Value != null && player.PlayerPawn.Value.IsValid).ToList();

        public static bool AreMoreThenPlayersConnected(int playerCount) => GetValidPlayerControllers().Count() >= playerCount;

        public static bool AreMoreThenOrEqualPlayersConnected(int playerCount) => GetValidPlayerControllers().Count() >= playerCount;

        public static void SuicideAll() => GetValidPlayerControllers().ForEach(x => x.CommitSuicide(true, true));
    }
}
