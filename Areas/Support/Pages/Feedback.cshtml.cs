using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.Areas.Identity.Data;
using Mtd.OrderMaker.Server.AppConfig;
using Mtd.OrderMaker.Server.Services;

namespace Mtd.OrderMaker.Server.Areas.Support.Pages
{
    public class FeedbackModel : PageModel
    {
        private readonly UserManager<WebAppUser> userManager;
        private readonly ConfigSettings configSettings;
        private readonly EmailSettings emailSettings;
        private readonly IEmailSenderBlank emailSender;
        private readonly IStringLocalizer<SharedResource> localizer;

        public FeedbackModel(IOptions<ConfigSettings> configSetting, 
            IOptions<EmailSettings> emailSettings, UserManager<WebAppUser> userManager,
            IEmailSenderBlank emailSender, IStringLocalizer<SharedResource> localizer)
        {
            this.configSettings = configSetting.Value;
            this.emailSettings = emailSettings.Value;
            this.emailSender = emailSender;
            this.localizer = localizer;
            this.userManager = userManager;
        }

        public void OnGet()
        {
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            string message = Request.Form["text-message"];
            WebAppUser user = await userManager.GetUserAsync(Request.HttpContext.User);
            BlankEmail blankEmail = new BlankEmail
            {
                Email = configSettings.EmailSupport,
                Subject = $"{emailSettings.Title} (feedback)",
                Header = localizer["Feedback"],
                Content = new List<string>()
                       {
                           $"{localizer["User"]}: <strong>{user.Title}</strong>",
                           $"{localizer["login"]} <strong>{user.UserName}</strong>",
                           $"{localizer["email"]} <strong>{user.Email}</strong>",
                           $"<hr>",
                           $"{message}",
                       }
            };

            await emailSender.SendEmailBlankAsync(blankEmail);

            return new OkResult();
        }
    }
}
