using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using SpaceInvaders.Models;

namespace SpaceInvadersUI.Services
{
    public class InputService
    {
        private readonly ILogger<InputService> _logger;
        private readonly byte[] _portStatus = { 0b0000_1000, 0b0000_0000 };

        private readonly Dictionary<string, SpaceInvadersKey> _keyMap = new()
        {
            { "ArrowRight", SpaceInvadersKey.P1Right },
            { "ArrowLeft", SpaceInvadersKey.P1Left },
            { "ArrowUp", SpaceInvadersKey.P1Fire },
            { "a", SpaceInvadersKey.P2Left },
            { "d", SpaceInvadersKey.P2Right },
            { "w", SpaceInvadersKey.P2Fire },
            { "1", SpaceInvadersKey.P1Start },
            { "2", SpaceInvadersKey.P2Start },
            { "Enter", SpaceInvadersKey.Credit }
        };

        public InputService(ILogger<InputService> logger)
        {
            _logger = logger;
        }

        internal byte GetPortStatus(int port)
        {
            if (port < 0 || port > 1) throw new ArgumentException(nameof(port));

            return _portStatus[port];
        }

        internal void KeyUp(string key)
        {
            if (_keyMap.ContainsKey(key))
            {
                _logger.LogInformation("Registered key up event for key {Key}", key);
                _portStatus[_keyMap[key].PortIndex()] &= _keyMap[key].KeyUpMask();
            }
        }

        internal void KeyDown(string key)
        {
            if (_keyMap.ContainsKey(key))
            {
                _logger.LogInformation("Registered key down event for key {Key}", key);
                _portStatus[_keyMap[key].PortIndex()] |= _keyMap[key].KeyDownMask();
            }
        }
    }
}
