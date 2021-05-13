using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpaceInvadersUI.Models;
using SpaceInvadersUI.Services;

namespace SpaceInvadersUI.Controllers
{
    [Route("api/v1")]
    public class InController : Controller
    {
        private readonly ILogger<InController> _logger;
        private readonly ShiftRegisterService _shiftRegisterService;
        private readonly InputService _inputService;

        public InController(ILogger<InController> logger, ShiftRegisterService shiftRegisterService, InputService inputService)
        {
            _logger = logger;
            _shiftRegisterService = shiftRegisterService;
            _inputService = inputService;
        }

        [Route("in")]
        public Cpu In([FromQuery(Name = "operand1")] byte port, [FromBody] Cpu cpu)
        {
            _logger.LogInformation("Requesting IN for port {Port}", port);
            cpu.State.A = port switch
            {
                0 => 0x0, // TODO INP0
                1 => _inputService.GetPortStatus(0),
                2 => _inputService.GetPortStatus(1),
                3 => _shiftRegisterService.GetValue(),
                _ => 0x0 // TODO - Other ports
            };
            return cpu;
        }
    }
}
