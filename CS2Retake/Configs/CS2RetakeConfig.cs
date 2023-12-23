using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CS2Retake.Configs
{
    public class CS2RetakeConfig : BasePluginConfig
    {
        public float SecondsUntilBombPlantedCheck { get; set; } = 5.0f;

        public bool SpotAnnouncerEnabled { get; set; } = true;

        public bool EnableQueue { get; set; } = true;
        public bool EnableScramble { get; set; } = true;
        public bool EnableSwitchOnRoundWin { get; set; } = true;

        public int ScrambleAfterSubsequentTerroristRoundWins { get; set; } = 5;

        public int MaxPlayers { get; set; } = 10;
        public float TeamBalanceRatio { get; set; } = 0.499f;

        public CS2RetakeConfig() {
            this.Version = 2;
        }
    }
}
