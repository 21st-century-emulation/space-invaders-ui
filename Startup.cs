using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpaceInvadersUI.Services;
using SpaceInvadersUI.SignalR;

namespace SpaceInvadersUI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IHostEnvironment env)
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddControllers();
            services.AddSingleton<ShiftRegisterService>();
            services.AddSingleton<InputService>();
            services.AddSingleton<InterruptCheckService>();
            services.AddHostedService<BackgroundUpdateService>();
            services.AddHttpClient();
            services.AddSignalR().AddJsonProtocol((options) =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.PayloadSerializerOptions.IncludeFields = true;
            });
            services.Configure<ApiEndpoints>(_configuration.GetSection("ApiEndpoints"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHealthChecks("/status");
            //app.UseSerilogRequestLogging();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/status");
                endpoints.MapControllers();
                endpoints.MapHub<MainHub>("/signalr/hub");
            });
        }
    }
}
