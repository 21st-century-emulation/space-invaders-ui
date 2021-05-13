using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace SpaceInvadersUI
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var logConfig = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Serilog", LogEventLevel.Warning)
                .WriteTo.Console();

            var lokiUrl = Environment.GetEnvironmentVariable("LOKI_URL");
            if (lokiUrl != null)
            {
                Console.WriteLine("Found LOKI_URL {0}", lokiUrl);
                logConfig
                    .MinimumLevel.Warning()
                    .WriteTo.GrafanaLoki(lokiUrl, labels: new[] {
                        new LokiLabel {Key = "application", Value = "space-invaders-ui"},
                    });
            }

            Log.Logger = logConfig.CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
