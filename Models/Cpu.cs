using System.Text.Json.Serialization;

namespace SpaceInvadersUI.Models
{
    public struct Cpu
    {

        [JsonInclude]
        public byte Opcode;

        [JsonInclude]
        public string Id;

        [JsonInclude]
        public CpuState State;
    }
}
