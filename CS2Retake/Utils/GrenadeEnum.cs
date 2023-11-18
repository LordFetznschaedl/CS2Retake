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
    public enum GrenadeEnum
    {
        [EnumMember(Value = "weapon_smokegrenade")]
        Smoke = 0,

        [EnumMember(Value = "weapon_flashbang")]
        Flashbang = 1,

        [EnumMember(Value = "weapon_hegrenade")]
        HighExplosive = 2,

        [EnumMember(Value = "weapon_molotov")]
        Molotov = 3,

        [EnumMember(Value = "weapon_incgrenade")]
        Incendiary = 4,

        [EnumMember(Value = "weapon_decoy")]
        Decoy = 5,

        //Frag Grenade (Danger Zone)
        [EnumMember(Value = "weapon_frag")]
        Frag = 6,

        //Tactical Awareness Grenade (Operation Wildfire)
        [EnumMember(Value = "weapon_tagrenade")]
        TacticalAwareness = 7,
    }
}
