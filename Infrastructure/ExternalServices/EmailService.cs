using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace App.Infrastructure.ExternalServices
{
    public class EmailService(IOptions<EmailSettings> emailSettings) : IEmailService
    {
        private readonly EmailSettings _emailSettings = emailSettings.Value;

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
    }
}
