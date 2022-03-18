using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.AppConfig;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server
{
    public class MigrationService : IHostedService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger logger;

        public MigrationService(IServiceScopeFactory serviceScopeFactory,
            ILogger<MigrationService> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceScopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<OrderMakerContext>();

            var idContext = scope.ServiceProvider
                .GetRequiredService<IdentityDbContext>();

            bool dbMigration = false;
            bool idMigration = false;

            try
            {
                IEnumerable<string> pm = await dbContext.Database
                    .GetPendingMigrationsAsync(cancellationToken);
                dbMigration = pm.Any();

                IEnumerable<string> idPending = await idContext.Database.GetPendingMigrationsAsync(cancellationToken);
                idMigration = idPending.Any();

                if (idMigration)
                {

                    await idContext.Database.MigrateAsync(cancellationToken);
                                        
                    using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<WebAppUser>>();
                    bool usersExists = await userManager.Users.AnyAsync(cancellationToken: cancellationToken);
                    if (usersExists == false)
                    {
                        using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<WebAppRole>>();
                        await InitIdentity(scope.ServiceProvider, roleManager, userManager);
                    }
                }

                if (dbMigration)
                {
                    await dbContext.Database.MigrateAsync(cancellationToken);

                    bool existstData = await dbContext.MtdSysType.AnyAsync();
                    if (existstData == false)
                    {
                        await InitDatabase(dbContext);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task InitIdentity(IServiceProvider serviceProvider, RoleManager<WebAppRole> roleManager, UserManager<WebAppUser> userManager)
        {

            var roleAdmin = new WebAppRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                Title = "Administrator",
                Seq = 30
            };

            var roleUser = new WebAppRole
            {
                Name = "User",
                NormalizedName = "USER",
                Title = "User",
                Seq = 20
            };

            var roleGuest = new WebAppRole
            {
                Name = "Guest",
                NormalizedName = "GUEST",
                Title = "Guest",
                Seq = 10
            };

            await roleManager.CreateAsync(roleAdmin);
            await roleManager.CreateAsync(roleUser);
            await roleManager.CreateAsync(roleGuest);

            var config = serviceProvider.GetRequiredService<IOptions<ConfigSettings>>();

            var defaultAdmin = new WebAppUser
            {
                Email = config.Value.EmailSupport,
                EmailConfirmed = true,
                Title = "Administrator",
                UserName = config.Value.DefaultUSR,

            };

            await userManager.CreateAsync(defaultAdmin, config.Value.DefaultPWD);
            await userManager.AddToRoleAsync(defaultAdmin, "Admin");

        }

        private async Task InitDatabase(OrderMakerContext context)
        {

            var mtdGroupForm = new MtdCategoryForm
            {
                Id = "17101180-9250-4498-BE4E-4A941AD6713C",
                Name = "Default",
                Description = "Default Group",
                Parent = "17101180-9250-4498-BE4E-4A941AD6713C"
            };

            await context.MtdCategoryForm.AddAsync(mtdGroupForm);
            await context.SaveChangesAsync();

            sbyte active = 1;
            List<MtdSysType> mtdSysTypes = new List<MtdSysType> {
                    new MtdSysType{ Id = 1, Name="Text", Description="Text", Active=active },
                    new MtdSysType{ Id = 2, Name="Integer", Description="Integer", Active=active},
                    new MtdSysType{ Id = 3, Name="Decimal",Description="Decimal", Active=active},
                    new MtdSysType{ Id = 4, Name = "Memo",Description="Memo",Active=active},
                    new MtdSysType{ Id = 5, Name="Date",Description="Date",Active=active},
                    new MtdSysType{ Id = 6, Name="DateTime",Description="DateTime",Active=active},
                    new MtdSysType{ Id = 7, Name="File",Description="File",Active=active},
                    new MtdSysType{ Id = 8, Name="Image",Description="Image",Active=active},
                    new MtdSysType{ Id = 11, Name="List",Description="List",Active=active},
                    new MtdSysType{ Id = 12, Name="Checkbox",Description="Checkbox",Active=active},
                    new MtdSysType{ Id = 13, Name="Link",Description="Link",Active=active},
                };

            await context.MtdSysType.AddRangeAsync(mtdSysTypes);
            await context.SaveChangesAsync();

            var mtdSysTerms = new List<MtdSysTerm>
                {
                    new MtdSysTerm {Id=1,Name="equal", Sign="=" },
                    new MtdSysTerm {Id=2,Name="less", Sign="<" },
                    new MtdSysTerm {Id=3,Name="more", Sign=">" },
                    new MtdSysTerm {Id=4,Name="contains", Sign="~" },
                    new MtdSysTerm {Id=5,Name="no equal", Sign="<>" },
                };

            await context.MtdSysTerm.AddRangeAsync(mtdSysTerms);
            await context.SaveChangesAsync();

            var mtdSysStyles = new List<MtdSysStyle>
                {
                    new MtdSysStyle{Id=4,Name="Lines", Description="Lines", Active=(sbyte)1},
                    new MtdSysStyle{Id=5,Name="Columns", Description="Columns", Active=(sbyte)1}
                };

            await context.MtdSysStyle.AddRangeAsync(mtdSysStyles);
            await context.SaveChangesAsync();

        }

    }
}
