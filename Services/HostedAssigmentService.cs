using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.AppConfig;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Services
{
    internal class HostedAssigmentService : IScopedService
    {
   
        private readonly ILogger _logger;
        private readonly OrderMakerContext context;
        private readonly UserHandler userHandler;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly EmailSettings emailSettings;
        private readonly IEmailSenderBlank emailSender;

        public HostedAssigmentService(ILogger<HostedAssigmentService> logger, OrderMakerContext context, UserHandler userHandler,
                IStringLocalizer<SharedResource> localizer, IOptions<EmailSettings> emailSettings, IEmailSenderBlank emailSender)
        {
            _logger = logger;
            this.context = context;
            this.userHandler = userHandler;
            this.localizer = localizer;
            this.emailSettings = emailSettings.Value;
            this.emailSender = emailSender;
        }

        public async Task RunServiceAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
  
                DateTime now = DateTime.Now;
                DateTime start = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
                DateTime end = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);


                if (now > start && now < end && now.DayOfWeek != DayOfWeek.Saturday && now.DayOfWeek != DayOfWeek.Sunday)
                {

                    List<MtdStoreTask> storeTasks = await context.MtdStoreTasks
                        .Where(x => x.Deadline < now && x.LastEventTime < now && x.Complete == 0).OrderBy(x => x.Deadline).ToListAsync() ?? new List<MtdStoreTask>();
                    List<string> userIds = storeTasks.GroupBy(x=>x.Executor).Select(x => x.Key).ToList() ?? new List<string>();


                    foreach (var userId in userIds)
                    {
                        WebAppUser user = await userHandler.FindByIdAsync(userId);
                        string email = user.Email;
                        List<string> links = new List<string>();
                        List<MtdStoreTask> sts = storeTasks.Where(x => x.Executor == userId).OrderBy(x=>x.MtdStoreId).ToList();

                        MtdStore doc = new MtdStore();
                        foreach (var st in sts)
                        {
                            MtdStore mtdStore = await context.MtdStore.Include(x => x.MtdFormNavigation).FirstOrDefaultAsync(x => x.Id == st.MtdStoreId);
                            if (doc.Id != mtdStore.Id)
                            {
                                doc = mtdStore;
                                string host = emailSettings.Host;
                                string href = $"{host}/workplace/store/details?id={st.MtdStoreId}";
                                string link = $"<a href='{HtmlEncoder.Default.Encode(href)}'>{mtdStore.MtdFormNavigation.Name} {localizer["Id"]} {mtdStore.Sequence:D9} {localizer["Date"]} {mtdStore.Timecr.ToShortDateString()}</a>";
                                links.Add(link);
                            }

                            links.Add($"- {st.Deadline:g} {st.Name}");
                        }

                        BlankEmail blankEmail = new BlankEmail
                        {
                            Email = email,
                            Subject = localizer["Task management event"],
                            Header = localizer["Deadline for tasks"],
                            Content = new List<string>() {
                                localizer["Please follow the links and close the tasks."]
                            }
                        };

                        links.ForEach((link) =>
                        {
                            blankEmail.Content.Add(link);
                        });


                        bool isOk = await emailSender.SendEmailBlankAsync(blankEmail);
                        if (isOk)
                        {
                            sts.ForEach((t) =>
                            {
                                t.LastEventTime = DateTime.Now.AddHours(10);
                            });

                            context.MtdStoreTasks.UpdateRange(sts);
                            await context.SaveChangesAsync();
                            _logger.LogInformation("Reminder sent to user {user}", user.GetFullName());
                        }

                        await Task.Delay(10000, stoppingToken);

                    }
                }

                await Task.Delay(5400000, stoppingToken);
            }
            
        }
    }
}
