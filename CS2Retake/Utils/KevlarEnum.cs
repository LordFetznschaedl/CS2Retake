using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CS2Retake.Utils
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum KevlarEnum
    {
        None = 0,
        Kevlar = 1,
        KevlarHelmet = 2,
    }
}
