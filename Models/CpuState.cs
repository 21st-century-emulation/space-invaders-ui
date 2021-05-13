using System.Text.Json.Serialization;

namespace SpaceInvadersUI.Models
{
    public struct CpuState
    {
        [JsonInclude]
        public CpuFlags Flags;

        [JsonInclude]
        public byte A;
        [JsonInclude]
        public byte B;
        [JsonInclude]
        public byte C;
        [JsonInclude]
        public byte D;
        [JsonInclude]
        public byte E;
        [JsonInclude]
        public byte H;
        [JsonInclude]
        public byte L;

        [JsonInclude]
        public ushort StackPointer;

        [JsonInclude]
        public ushort ProgramCounter;

        [JsonInclude]
        public ulong Cycles;

        [JsonInclude]
        public bool InterruptsEnabled;
    }
}
