using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Mtd.OrderMaker.Server.Areas.Identity.IdentityHostingStartup))]
namespace Mtd.OrderMaker.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}