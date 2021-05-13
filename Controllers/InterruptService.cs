using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpaceInvadersUI.Models;
using SpaceInvadersUI.Services;

namespace SpaceInvadersUI.Controllers
{
    [Route("api/v1")]
    public class InterruptService : Controller
    {
        private readonly ILogger<InterruptService> _logger;
        private readonly InterruptCheckService _interruptCheckService;

        public InterruptService(ILogger<InterruptService> logger, InterruptCheckService interruptCheckService)
        {
            _logger = logger;
            _interruptCheckService = interruptCheckService;
        }

        [HttpPost, Route("checkInterrupts")]
        public async Task<IActionResult> CheckInterrupts([FromBody] Cpu cpu)
        {
            _logger.LogDebug("Checking for interrupts on cpu {Id}", cpu.Id);
            var rstOperand = await _interruptCheckService.CheckForInterrupts(cpu);

            return Ok(rstOperand);
        }
    }
}
