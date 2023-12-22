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

        public static float SecondsUntilBombPlantedCheck { get; set; }

        public static int MaxPlayers { get; set; }
        public static float TeamBalanceRatio { get; set; }

        

        public static void SetModuleInfo(string moduleName, string moduleDirectory)
        {
            ModuleName = moduleName;
            ModuleDirectory = moduleDirectory;
        }

        public static void SetBaseConfig(CS2RetakeConfig baseConfig)
        {
            SecondsUntilBombPlantedCheck = baseConfig.SecondsUntilBombPlantedCheck;

            MaxPlayers = baseConfig.MaxPlayers;
            TeamBalanceRatio = baseConfig.TeamBalanceRatio;
        }
    }
}
