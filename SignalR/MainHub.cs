using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SpaceInvadersUI.Services;

namespace SpaceInvadersUI.SignalR
{
    public class MainHub : Hub
    {
        private readonly ILogger<MainHub> _log;
        private readonly InputService _inputService;

        public MainHub(ILogger<MainHub> log, InputService inputService)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _inputService = inputService ?? throw new ArgumentNullException(nameof(inputService));
        }

        public void KeyUp(string key)
        {
            _log.LogInformation("Key up: {0}", key);
            _inputService.KeyUp(key);
        }

        public void KeyDown(string key)
        {
            _log.LogInformation("Key down: {0}", key);
            _inputService.KeyDown(key);
        }
    }
}
