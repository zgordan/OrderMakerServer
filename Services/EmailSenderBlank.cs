using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Mtd.OrderMaker.Server.AppConfig;
using Mtd.OrderMaker.Server.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Mtd.OrderMaker.Server.Services
{
    public class BlankEmail
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Header { get; set; }
        public List<string> Content { get; set; }
    }

    public class EmailSenderBlank : IEmailSenderBlank
    {
        private EmailSettings _emailSettings { get; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly OrderMakerContext _context;
        private readonly ConfigHandler configHandler;

        public EmailSenderBlank(IOptions<EmailSettings> emailSettings, IWebHostEnvironment hostingEnvironment, ConfigHandler configHandler)
        {
            _emailSettings = emailSettings.Value;
            _hostingEnvironment = hostingEnvironment;
            this.configHandler = configHandler;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            Execute(email, subject, message).Wait();
            return Task.FromResult(0);
        }

        public async Task<bool> SendEmailBlankAsync(BlankEmail blankEmail)
        {
            string pathImgMenu = $"{_emailSettings.Host}/lib/mtd-ordermaker/images/logo-mtd.png";
            var imgMenu = await configHandler.GetImageFromConfig(configHandler.CodeImgMenu);
            if (imgMenu != string.Empty) { pathImgMenu = $"{_emailSettings.Host}/images/logo.png"; }

            try
            {

                string message = string.Empty;
                foreach (string p in blankEmail.Content)
                {
                    message += $"<p>{p}</p>";
                }

                string webRootPath = _hostingEnvironment.WebRootPath;
                string contentRootPath = _hostingEnvironment.ContentRootPath;
                var file = Path.Combine(contentRootPath, "wwwroot", "lib", "mtd-ordermaker", "emailform", "blank.html");
                var htmlArray = File.ReadAllText(file);
                string htmlText = htmlArray.ToString();

                htmlText = htmlText.Replace("{logo}", pathImgMenu);
                htmlText = htmlText.Replace("{title}", _emailSettings.Title);
                htmlText = htmlText.Replace("{header}", blankEmail.Header);
                htmlText = htmlText.Replace("{content}", message);
                htmlText = htmlText.Replace("{copyright}", _emailSettings.CopyRight);
                htmlText = htmlText.Replace("{footer}", _emailSettings.Footer);

                await SendEmailAsync(blankEmail.Email, blankEmail.Subject, htmlText);
            }
            catch
            {
                return false;
            }


            return true;
        }

        private async Task Execute(string email, string subject, string message)
        {
            try
            {
                MailAddress toAddress = new MailAddress(email);
                MailAddress fromAddress = new MailAddress(_emailSettings.FromAddress, _emailSettings.FromName);
                // создаем письмо: message.Destination - адрес получателя
                MailMessage mail = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true,
                };

                using SmtpClient smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_emailSettings.FromAddress, _emailSettings.Password);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Email sender service \n {ex.Message}");
            }
        }
    }
}
