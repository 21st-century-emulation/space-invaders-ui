using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpaceInvadersUI.Models;
using SpaceInvadersUI.Services;

namespace SpaceInvadersUI.Controllers
{
    [Route("api/v1")]
    public class OutController : Controller
    {
        private readonly ILogger<InController> _logger;
        private readonly ShiftRegisterService _shiftRegisterService;

        public OutController(ILogger<InController> logger, ShiftRegisterService shiftRegisterService)
        {
            _logger = logger;
            _shiftRegisterService = shiftRegisterService;
        }

        [Route("out")]
        public Cpu Out([FromQuery(Name = "operand1")] byte port, [FromQuery(Name = "operand2")] byte value, [FromBody] Cpu cpu)
        {
            _logger.LogInformation("Requesting OUT for port {Port} = {Value}", port, value);
            switch (port)
            {
                case 2:
                    _shiftRegisterService.SetOffset(value);
                    break;
                case 3:
                    // TODO SOUND1 port
                    break;
                case 4:
                    _shiftRegisterService.SetRegister(value);
                    break;
                case 5:
                    // TODO SOUND2 port
                    break;
                case 6:
                    // TODO WATCHDOG PORT
                    break;
            }

            return cpu;
        }
    }
}
