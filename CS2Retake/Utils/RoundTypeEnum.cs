using System.Text.Json.Serialization;

namespace CS2Retake.Utils;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoundTypeEnum
{
    Undefined = -1,
    Pistol = 0,
    Mid = 1,
    FullBuy = 2
}