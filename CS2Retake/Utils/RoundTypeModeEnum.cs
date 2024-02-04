using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CS2Retake.Utils
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RoundTypeModeEnum
    {
        Random = 0,
        Sequence = 1,
        Specific = 2,
    }
}
