
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpaceInvadersUI.Models;

namespace SpaceInvadersUI.Controllers
{
    [Route("api/v1/cpu")]
    public class ExecuteController : ControllerBase
    {
        private readonly ILogger<ExecuteController> _log;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ApiEndpoints _apiEndpoints;

        public ExecuteController(ILogger<ExecuteController> log, IHttpClientFactory clientFactory, IOptions<ApiEndpoints> apiEndpoints)
        {
            _log = log;
            _clientFactory = clientFactory;
            _apiEndpoints = apiEndpoints.Value;
        }

        [Route("initialise"), HttpPost]
        [ProducesResponseType(typeof(Cpu), StatusCodes.Status200OK)]
        public IActionResult InitializeCpu([FromBody] Cpu cpu)
        {
            // Space invaders doesn't need to do any pre-init, even SP is set from code
            return Ok(cpu);
        }

        [Route("start"), HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public async Task<IActionResult> StartProgram(IFormFile romForm)
        {
            if (romForm == null) return BadRequest("Missing rom file on start request");
            _log.LogInformation("Requesting CPU start with new ROM");

            var romBytes = new byte[0x10000];
            using var ms = new MemoryStream();
            await romForm.CopyToAsync(ms);
            var sentBytes = ms.ToArray();
            Array.Copy(sentBytes, romBytes, Math.Min(0x10000, sentBytes.LongLength));
            var computerId = Guid.NewGuid();

            using (var client = _clientFactory.CreateClient())
            {
                var rom = new
                {
                    Id = computerId.ToString(),
                    ProgramState = Convert.ToBase64String(romBytes),
                };
                var initResponse = await client.PostAsync(
                    $"{_apiEndpoints.InitialiseMemoryApi}",
                    new StringContent(JsonSerializer.Serialize(rom, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            IncludeFields = true,
                        }))
                    {
                        Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
                    });

                initResponse.EnsureSuccessStatusCode();

                var startResponse = await client.PostAsync($"{_apiEndpoints.StartProgramApi}?id={computerId}", new StringContent(""));
                startResponse.EnsureSuccessStatusCode();
            }

            _log.LogInformation("ROM loaded into shared CPU queue");
            return Ok(computerId);
        }

        [Route("status"), HttpGet]
        public IActionResult CheckProgramStatus(string id)
        {
            if (id == null) return BadRequest("Missing id for computer");
            _log.LogInformation("Requesting CPU status");

            throw new NotImplementedException("TODO - Implement status function");
        }

        [Route("reset"), HttpPost]
        public IActionResult StopProgram(string id)
        {
            _log.LogInformation("Requesting CPU stop and reset");

            throw new NotImplementedException("TODO - Implement stop function");
        }
    }
}
