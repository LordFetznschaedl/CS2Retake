using CounterStrikeSharp.API.Core;

namespace CS2Retake.Configs;

public class CS2RetakeConfig : BasePluginConfig
{
    public CS2RetakeConfig()
    {
        Version = 1;
    }

    public float SecondsUntilBombPlantedCheck { get; set; } = 5.0f;
    public bool SpotAnnouncerEnabled { get; set; } = true;
}