using App.Core.DTOs.Requests.CreateRequestDtos;
using System.Net.Mail;

namespace App.Application.IExternalServices
{
    public interface IEmailService
    {
        MailMessage CreateMailMessage(MailRequestDto request);
        void SendEmail(MailMessage mailMessage);
        string CreateBody(string userName, string appName, string confimationLink, string membershipNo);
    }
}
