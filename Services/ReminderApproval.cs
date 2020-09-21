using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.AppConfig;
using Mtd.OrderMaker.Server.Areas.Config.Pages.Approval;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.Entity;
using Mtd.OrderMaker.Server.EntityHandler.Approval;
using Mtd.OrderMaker.Server.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Mtd.OrderMaker.Server.Services
{
    internal class ReminderApproval : IScopedService
    {
        private int executionCount = 0;
        private readonly ILogger _logger;
        private readonly OrderMakerContext context;
        private readonly UserHandler userHandler;
        private readonly IStringLocalizer<SharedResource> localizer;
        private readonly EmailSettings emailSettings;
        private readonly IEmailSenderBlank emailSender;

        public ReminderApproval(ILogger<ReminderApproval> logger, OrderMakerContext context, UserHandler userHandler,
                IStringLocalizer<SharedResource> localizer, IOptions<EmailSettings> emailSettings, IEmailSenderBlank emailSender)
        {
            _logger = logger;
            this.context = context;
            this.userHandler = userHandler;
            this.localizer = localizer;
            this.emailSettings = emailSettings.Value;
            this.emailSender = emailSender;
        }

        public async Task RunService(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                executionCount++;
                DateTime now = DateTime.Now;
                DateTime start = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);
                DateTime end = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);
                

                if (now > start && now < end && now.DayOfWeek != DayOfWeek.Saturday && now.DayOfWeek != DayOfWeek.Sunday)
                {
                    Dictionary<string, List<MtdStore>> keyValues = await ApprovalHandler.GetHoveringApprovalAsync(context, userHandler);

                    foreach (KeyValuePair<string, List<MtdStore>> entry in keyValues)
                    {
                        WebAppUser user = await userHandler.FindByIdAsync(entry.Key);
                        string email = user.Email;
                        List<string> links = new List<string>();
                        foreach (MtdStore store in entry.Value)
                        {
                            string host = emailSettings.Host;
                            string href = $"{host}/workplace/store/details?id={store.Id}";
                            string link = $"<a href='{HtmlEncoder.Default.Encode(href)}'>{store.MtdFormNavigation.Name} {localizer["Id"]} {store.Sequence:D9} {localizer["Date"]} {store.Timecr.ToShortDateString()}</a>";
                            links.Add(link);
                        }

                        BlankEmail blankEmail = new BlankEmail
                        {
                            Email = email,
                            Subject = localizer["Approval process event"],
                            Header = localizer["Approval required"],
                            Content = new List<string>() {
                                localizer["Please follow the links and complete the approval process.<br/> Or use the filter 'Wait list' to get a list of documents awaiting approval."],
                                localizer["The following documents are awaiting your approval:"]
                            }
                        };

                        links.ForEach((link) =>
                        {
                            blankEmail.Content.Add(link);
                        });

                        

                        bool isOk = await emailSender.SendEmailBlankAsync(blankEmail);
                        if (isOk)
                        {
                            List<string> storeIds = entry.Value.Select(x => x.Id).ToList();
                            List<MtdStoreApproval> approvals = await context.MtdStoreApproval.Where(x => storeIds.Contains(x.Id))
                                .Select(x => new MtdStoreApproval
                                {
                                    Id = x.Id,
                                    MtdApproveStage = x.MtdApproveStage,
                                    PartsApproved = x.PartsApproved,
                                    Complete = x.Complete,
                                    Result = x.Result,
                                    SignChain = x.SignChain,
                                    LastEventTime = DateTime.Now

                                }).ToListAsync();

                            context.MtdStoreApproval.UpdateRange(approvals);
                            await context.SaveChangesAsync();
                            _logger.LogInformation("Reminder sent to user {user}", user.GetFullName());
                        }

                        await Task.Delay(10000, stoppingToken);
                    }
                }

                

                await Task.Delay(7200000, stoppingToken);
            }
        }
    }
}
