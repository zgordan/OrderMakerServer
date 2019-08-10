using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Web.Services
{
    public interface IEmailSenderBlank
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task<bool> SendEmailBlankAsync(BlankEmail blankEmail);
    }
}
