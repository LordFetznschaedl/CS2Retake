using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Configs
{
    public static class RuntimeConfig
    {
        public static string ModuleName { get; set; } = string.Empty;
        public static string ModuleDirectory { get; set; } = string.Empty;

        public static float SecondsUntilBombPlantedCheck { get; set; } = 5;

        public static int ScrambleAfterSubsequentTerroristRoundWins { get; set; } = 5;

        public static int MaxPlayers { get; set; } = 10;
        public static float TeamBalanceRatio { get; set; } = 0.499f;

        public static PlantTypeEnum PlantType { get; set; } = PlantTypeEnum.AutoPlant;

        public static void SetModuleInfo(string moduleName, string moduleDirectory)
        {
            ModuleName = moduleName;
            ModuleDirectory = moduleDirectory;
        }

        public static void SetBaseConfig(CS2RetakeConfig baseConfig)
        {
            SecondsUntilBombPlantedCheck = baseConfig.SecondsUntilBombPlantedCheck;

            ScrambleAfterSubsequentTerroristRoundWins = baseConfig.ScrambleAfterSubsequentTerroristRoundWins;

            MaxPlayers = baseConfig.MaxPlayers;
            TeamBalanceRatio = baseConfig.TeamBalanceRatio;

            PlantType = baseConfig.PlantType;
        }
    }
}
