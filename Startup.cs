/*
    MTD OrderMaker - http://mtdkey.com
    Copyright (c) 2019 Oleg Bruev <job4bruev@gmail.com>. All rights reserved.
*/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.AppConfig;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Services;
using Mtd.OrderMaker.Service;
using System;
using System.Globalization;


namespace Mtd.OrderMaker.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
                options.Secure = CookieSecurePolicy.Always;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Domain = Configuration.GetConnectionString("Domain");
                options.Cookie.Name = $".OrderMaker.{Configuration.GetConnectionString("ClientName")}";
            });

            services.AddDataProtection()
                .SetApplicationName($"{Configuration.GetConnectionString("ClientName")}")
                .PersistKeysToDbContext<IdentityDbContext>();

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("IdentityConnection"), new MySqlServerVersion(new Version(8, 0))));


            services.AddDefaultIdentity<WebAppUser>(config =>
            {
                config.SignIn.RequireConfirmedEmail = false;
                config.User.RequireUniqueEmail = true;

            }).AddRoles<WebAppRole>()
             .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddDbContext<OrderMakerContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DataConnection"), new MySqlServerVersion(new Version(8, 0))));

            services.AddMemoryCache();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RoleAdmin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RoleUser", policy => policy.RequireRole("User", "Admin"));
                options.AddPolicy("RoleGuest", policy => policy.RequireRole("Guest", "User", "Admin"));
            });

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResource));
                    })
                .AddRazorPagesOptions(options =>
                    {
                        options.Conventions.AuthorizeFolder("/");
                        options.Conventions.AuthorizeAreaFolder("Workplace", "/", "RoleUser");
                        options.Conventions.AuthorizeAreaFolder("Identity", "/Users", "RoleAdmin");
                        options.Conventions.AuthorizeAreaFolder("Config", "/", "RoleAdmin");
                    });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddSingleton<PolicyCache>();
            services.AddScoped<UserHandler>();
            services.AddTransient<ConfigHandler>();
            services.AddTransient<IEmailSenderBlank, EmailSender>();
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<ConfigSettings>(Configuration.GetSection("ConfigSettings"));
            services.Configure<LimitSettings>(Configuration.GetSection("LimitSettings"));


            services.AddMvc(options => options.EnableEndpointRouting = false);

            if (!CurrentEnvironment.IsDevelopment())
            {
                services.AddHostedService<HostedAssigmentExecutor>();
                services.AddHostedService<HostedApprovalExecutor>();
                services.AddScoped<HostedApprovalService>();
                services.AddScoped<HostedAssigmentService>();
            }

#if DEBUG
            IMvcBuilder builder = services.AddRazorPages();
            if (CurrentEnvironment.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
#endif

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                                new CultureInfo("en-US"),
                                new CultureInfo("ru-RU"),
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new[] { new CookieRequestCultureProvider() };
            });

            services.AddHostedService<MigrationService>();

        }


        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {

            var config = serviceProvider.GetRequiredService<IOptions<ConfigSettings>>();

            if (CurrentEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }


            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            var cultureInfo = new CultureInfo(config.Value.CultureInfo);
            locOptions.Value.DefaultRequestCulture = new RequestCulture(cultureInfo);
            app.UseRequestLocalization(locOptions.Value);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            app.UseMvc();

        }
    }
}
