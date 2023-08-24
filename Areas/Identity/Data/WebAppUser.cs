using Microsoft.AspNetCore.Identity;

namespace Mtd.OrderMaker.Server.Areas.Identity.Data
{
    public class WebAppUser : IdentityUser
    {
        [PersonalData]
        public string Title { get; set; }
        public string TitleGroup { get; set; }
    }
}
