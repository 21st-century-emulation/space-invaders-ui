using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SpaceInvadersUI.Controllers
{
    [Route("api/v1/memory")]
    public class MemoryController : ControllerBase
    {
        private readonly ILogger<MemoryController> _log;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ApiEndpoints _memoryBusOptions;

        public MemoryController(ILogger<MemoryController> log, IHttpClientFactory clientFactory, IOptions<ApiEndpoints> memoryBusOptions)
        {
            _log = log;
            _clientFactory = clientFactory;
            _memoryBusOptions = memoryBusOptions.Value;
        }

        [Route("dump"), HttpGet]
        [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> DumpMemory(string id)
        {
            if (id == null) return BadRequest("Missing id for computer");

            _log.LogInformation("Dumping all memory");
            using (var client = _clientFactory.CreateClient())
            {
                var response = await client.GetAsync($"{_memoryBusOptions.ReadRangeApi}?id={id}&address={0}&length={0x10000}");
                response.EnsureSuccessStatusCode();
                var base64Range = await response.Content.ReadAsStringAsync();
                var dbDump = Convert.FromBase64String(base64Range);
                return Ok(dbDump);
            }
        }

        [Route("inspect"), HttpGet]
        [ProducesResponseType(typeof(byte), StatusCodes.Status200OK)]
        public async Task<IActionResult> InspectValue(string id, ushort address)
        {
            if (id == null) return BadRequest("Missing id for computer");
            if (id == null) return BadRequest("Missing address to inspect");

            _log.LogInformation("Inspecting value at address {0:X4}", address);
            using (var client = _clientFactory.CreateClient())
            {
                var response = await client.GetAsync($"{_memoryBusOptions.ReadByteApi}?id={id}&address={address}");
                response.EnsureSuccessStatusCode();
                return Ok(await response.Content.ReadAsStringAsync());
            }
        }

        [Route("reset"), HttpPost]
        public async Task<IActionResult> InjectValue(string id, ushort address, byte value)
        {
            if (id == null) return BadRequest("Missing id for computer");
            if (id == null) return BadRequest("Missing address to inspect");
            if (id == null) return BadRequest("Missing value to set");

            _log.LogInformation("Injecting value {0:X2} at address {1:X4} for computer {2}", value, address, id);
            using (var client = _clientFactory.CreateClient())
            {
                var response = await client.PostAsync($"{_memoryBusOptions.WriteByteApi}?id={id}&address={address}&value={value}", new StringContent(""));
                response.EnsureSuccessStatusCode();
                return Ok();
            }
        }
    }
}
