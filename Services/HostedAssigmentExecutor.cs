using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Service
{
    public class HostedAssigmentExecutor : BackgroundService
    {
        private readonly ILogger<HostedAssigmentExecutor> _logger;

        public HostedAssigmentExecutor(IServiceProvider services, ILogger<HostedAssigmentExecutor> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("The Hosted Assigment Service has started.");
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {
                var scopedService = scope.ServiceProvider.GetRequiredService<HostedAssigmentService>();
                await scopedService.RunServiceAsync(stoppingToken);
            }

            _logger.LogInformation("The Hosted Assigment Service is working.");
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("The Hosted Assigment Service is stopping.");

            await Task.CompletedTask;
        }
    }
}
