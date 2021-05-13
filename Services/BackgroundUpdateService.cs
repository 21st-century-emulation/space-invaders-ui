using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpaceInvadersUI.Models;
using SpaceInvadersUI.SignalR;

namespace SpaceInvadersUI.Services
{
    internal class BackgroundUpdateService : IHostedService, IDisposable
    {
        private readonly ILogger<BackgroundUpdateService> _logger;
        private Timer _timer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ApiEndpoints _apiEndpoints;

        public BackgroundUpdateService(ILogger<BackgroundUpdateService> logger, IServiceScopeFactory serviceScopeFactory, IOptions<ApiEndpoints> apiEndpoints)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _apiEndpoints = apiEndpoints.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Update Service is starting.");

            _timer = new Timer(UpdateCpuOnClients, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        private async void UpdateCpuOnClients(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<MainHub>>();
                using (var client = clientFactory.CreateClient())
                {
                    var response = await client.GetAsync($"{_apiEndpoints.GetStatusApi}");
                    response.EnsureSuccessStatusCode();
                    var data = await response.Content.ReadAsStringAsync();
                    var cpu = JsonSerializer.Deserialize<Cpu>(data, new JsonSerializerOptions
                    {
                        IncludeFields = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    if (cpu.Id != null)
                    {
                        // Get all 0x100 bytes used for RAM to display in the UI
                        var memoryResponse = await client.GetAsync($"{_apiEndpoints.ReadRangeApi}?id={cpu.Id}&address={0x2000}&length={0x100}");
                        memoryResponse.EnsureSuccessStatusCode();
                        var memoryBase64String = await memoryResponse.Content.ReadAsStringAsync();
                        
                        await hubContext.Clients.All.SendAsync("cpuUpdated", cpu, memoryBase64String);
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Update Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
