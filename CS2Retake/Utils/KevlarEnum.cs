using System.Text.Json.Serialization;

namespace CS2Retake.Utils;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum KevlarEnum
{
    None = 0,
    Kevlar = 1,
    KevlarHelmet = 2
}