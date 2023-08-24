using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Service
{
    public class HostedApprovalExecutor : BackgroundService
    {
        private readonly ILogger<HostedApprovalExecutor> _logger;

        public HostedApprovalExecutor(IServiceProvider services, ILogger<HostedApprovalExecutor> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("The Hosted Approval Service has started.");
            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {
                var scopedService =
                    scope.ServiceProvider
                        .GetRequiredService<HostedApprovalService>();

                await scopedService.RunServiceAsync(stoppingToken);
            }

            _logger.LogInformation("The Hosted Approval Service is working.");
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("The Hosted Approval Service is stopping.");

            await Task.CompletedTask;
        }



    }
}
