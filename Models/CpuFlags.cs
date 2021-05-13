using System.Text.Json.Serialization;

namespace SpaceInvadersUI.Models
{
    public struct CpuFlags
    {
        [JsonInclude]
        public bool Sign;

        [JsonInclude]
        public bool Zero;

        [JsonInclude]
        public bool AuxCarry;

        [JsonInclude]
        public bool Parity;

        [JsonInclude]
        public bool Carry;
    }
}
