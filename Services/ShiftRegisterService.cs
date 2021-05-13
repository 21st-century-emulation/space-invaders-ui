using SpaceInvadersUI.Models;

namespace SpaceInvadersUI.Services
{
    public class ShiftRegisterService
    {
        private ShiftRegister _shiftRegister;

        internal byte GetValue() => _shiftRegister.Value();

        internal void SetOffset(byte value)
        {
            _shiftRegister.Offset = (byte)(value & 0b111); ;
        }

        internal void SetRegister(byte value)
        {
            _shiftRegister.Register = (ushort)((_shiftRegister.Register >> 8) | (value << 8));
        }
    }
}
