using App.Core.DTOs.Requests.CreateRequestDtos;
using System.Net.Mail;

namespace App.Infrastructure.ExternalServices.Email
{
    public interface IEmailService
    {
        MailMessage CreateMailMessage(MailRequestDto request);
        void SendEmail(MailMessage mailMessage);
    }
}
