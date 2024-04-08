using CS2Retake.Entities;
using CS2Retake.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CSZoneNet.Plugin.Utils.Enums;

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
        public static RoundTypeModeEnum RoundTypeMode { get; set; } = RoundTypeModeEnum.Sequence;

        public static List<RoundTypeSequenceEntity> RoundTypeSequence { get; set; } = new List<RoundTypeSequenceEntity>()
        {
            new RoundTypeSequenceEntity(RoundTypeEnum.Pistol, 5),
            new RoundTypeSequenceEntity(RoundTypeEnum.Mid, 3),
            new RoundTypeSequenceEntity(RoundTypeEnum.FullBuy, -1),
        };

        public static RoundTypeEnum RoundTypeSpecific { get; set; } = RoundTypeEnum.FullBuy;

        public static AllocatorEnum Allocator { get; set; } = AllocatorEnum.Command;

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

            RoundTypeMode = baseConfig.RoundTypeMode;
            RoundTypeSequence = baseConfig.RoundTypeSequence;
            RoundTypeSpecific = baseConfig.RoundTypeSpecific;

            Allocator = baseConfig.Allocator;
        }
    }
}
