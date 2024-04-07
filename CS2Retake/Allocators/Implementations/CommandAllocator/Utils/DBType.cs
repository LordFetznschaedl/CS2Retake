using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CS2Retake.Allocators.Implementations.CommandAllocator.Utils
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DBType
    {
        Cache = 0,
        SQLite = 1,
    }
}
