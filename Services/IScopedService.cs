using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Services
{
    internal interface IScopedService
    {
        Task RunService(CancellationToken stoppingToken);
    }
}
