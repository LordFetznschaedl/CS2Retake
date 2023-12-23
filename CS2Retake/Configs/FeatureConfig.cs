using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Configs
{
    public static class FeatureConfig
    {
        public static bool EnableSpotAnnouncer { get; set; } = true;
        public static bool EnableQueue { get; set; } = true;
        public static bool EnableScramble { get; set; } = true;
        public static bool EnableSwitchOnRoundWin { get; set; } = true;

        public static void SetBaseConfig(CS2RetakeConfig baseConfig)
        {
            EnableSpotAnnouncer = baseConfig.SpotAnnouncerEnabled;
            EnableQueue = baseConfig.EnableQueue;
            EnableScramble = baseConfig.EnableScramble;
            EnableSwitchOnRoundWin = baseConfig.EnableSwitchOnRoundWin;
        }
    }
}
