using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Areas.Identity.Data
{
    public class WebAppUser : IdentityUser
    {
        [PersonalData]
        public string Title { get; set; }
    }
}
