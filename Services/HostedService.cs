using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Service
{
    public class HostedService : BackgroundService
    {
        private readonly ILogger<HostedService> _logger;

        public HostedService(IServiceProvider services, ILogger<HostedService> logger)
        {
            Services = services;
            _logger = logger;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation( "Hosted Service running.");

            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hosted Service is working.");

            using (var scope = Services.CreateScope())
            {
                var scopedService =
                    scope.ServiceProvider
                        .GetRequiredService<IScopedService>();
                
                await scopedService.RunService(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hosted Service is stopping.");

            await Task.CompletedTask;
        }
    }
}
