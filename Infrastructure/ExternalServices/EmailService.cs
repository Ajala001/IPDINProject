using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace App.Infrastructure.ExternalServices
{
    public class EmailService: IEmailService
    {
        private readonly IAppEnvironment _appEnvironment;
        private readonly EmailSettings _emailSettings;

        public EmailService(IAppEnvironment appEnvironment, IOptions<EmailSettings> emailSettings)
        {
            _appEnvironment = appEnvironment;
            _emailSettings = emailSettings.Value;
        }
        public MailMessage CreateMailMessage(MailRequestDto request)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_emailSettings.Email, _emailSettings.DisplayName),
                Subject = request.Subject,
                Body = request.Body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(request.ToEmail);  // Add recipient(s) here

            return mailMessage;
        }

        public void SendEmail(MailMessage mailMessage)
        {
            using (SmtpClient smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
                smtpClient.EnableSsl = true;
                smtpClient.Send(mailMessage);
            }
        }

        public string CreateBody(string userName, string appName, string confimationLink)
        {
            var filePath = Path.Combine(_appEnvironment.WebRootPath, "html", "ConfirmationEmailTemplate.html");
            if (!File.Exists(filePath))
            {
                return "Path not found";
            }


            string body = string.Empty;
            using (StreamReader reader = new StreamReader(filePath))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{{UserName}}", userName);
            body = body.Replace("{{AppName}}", appName);
            body = body.Replace("{{ConfirmationLink}}", confimationLink);

            return body;
        }
    }
}
