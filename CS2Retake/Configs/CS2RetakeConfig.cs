using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS2Retake.Configs
{
    public class CS2RetakeConfig : BasePluginConfig
    {
        public override int Version { get; set; } = 1;
        public bool SpotAnnouncerEnabled { get; set; } = true;
    }
}
