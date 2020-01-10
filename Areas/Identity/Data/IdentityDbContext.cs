using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mtd.OrderMaker.Server.Areas.Identity.Data;

namespace Mtd.OrderMaker.Server.Data
{
    public class IdentityDbContext : IdentityDbContext<WebAppUser,WebAppRole,string>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {            
        }


    }
}
