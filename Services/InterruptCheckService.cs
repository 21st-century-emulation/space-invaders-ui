using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpaceInvadersUI.Models;
using SpaceInvadersUI.SignalR;

namespace SpaceInvadersUI.Services
{
    public class InterruptCheckService
    {
        private const int CyclesPerFrame = 34132;
        private const int CyclesPerHalfFrame = 17066;
        private readonly ILogger<InterruptCheckService> _logger;
        private ulong _lastSeenCycles = 0;
        private int _countDownToVBlank = 0; // TODO - Triggers vblank immediately to see what's going on, set back to something longer
        private int _countDownToHalfScreen = CyclesPerHalfFrame;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ApiEndpoints _memoryBusOptions;


        public InterruptCheckService(ILogger<InterruptCheckService> logger, IServiceScopeFactory serviceScopeFactory, IOptions<ApiEndpoints> memoryBusOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _memoryBusOptions = memoryBusOptions?.Value ?? throw new ArgumentNullException(nameof(memoryBusOptions));
        }

        internal async Task<byte?> CheckForInterrupts(Cpu cpu)
        {
            if (cpu.State.Cycles < _lastSeenCycles) throw new ArgumentException("Invalid call to check interrupts, cycles is less than last time it was called", nameof(cpu));
            var cycleDiff = (int)(cpu.State.Cycles - _lastSeenCycles);
            _lastSeenCycles = cpu.State.Cycles;

            _countDownToVBlank -= cycleDiff;
            _countDownToHalfScreen -= cycleDiff;

            // Check for half screen interrupts first - this generates RST 2 which is a CALL 0x0008
            if (_countDownToHalfScreen < 0)
            {
                _countDownToHalfScreen += CyclesPerFrame;
                _logger.LogInformation("Firing half screen interrupt");
                return 2; // Don't need to check for both interrupts
            }

            // Then check for vblank interrupts - this generates RST 4 which is a CALL 0x0010
            if (_countDownToVBlank < 0)
            {
                _countDownToVBlank += CyclesPerFrame;
                _logger.LogInformation("Firing vblank interrupt and calling external service");
                
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<MainHub>>();
                    using (var client = httpClientFactory.CreateClient())
                    {
                        var vramRangeResponse = await client.GetAsync($"{_memoryBusOptions.ReadRangeApi}?id={cpu.Id}&address={0x2400}&length={0x1C00}");
                        vramRangeResponse.EnsureSuccessStatusCode();
                        var base64String = await vramRangeResponse.Content.ReadAsStringAsync();
                        await hubContext.Clients.All.SendAsync("vblank", Convert.FromBase64String(base64String));
                    }
                }
                return 1;
            }

            return null;
        }
    }

}
