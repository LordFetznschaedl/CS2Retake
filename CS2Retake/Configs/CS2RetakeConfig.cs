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
        

        public CS2RetakeConfig() {
            this.Version = 1;
        }
    }
}
