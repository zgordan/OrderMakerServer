using Microsoft.AspNetCore.Identity;

namespace Mtd.OrderMaker.Server.Areas.Identity.Data
{
    public class WebAppRole : IdentityRole
    {
        public string Title { get; set; }
        public int Seq { get; set; }

    }
}
