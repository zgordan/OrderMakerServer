using System.Threading;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Services
{
    internal interface IScopedService
    {
        Task RunServiceAsync(CancellationToken stoppingToken);
    }
}
