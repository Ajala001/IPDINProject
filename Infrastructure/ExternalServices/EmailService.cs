using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;
using System.Diagnostics;

namespace App.Infrastructure.ExternalServices
{
    public class EmailService: IEmailService
    {
        private readonly IAppEnvironment _appEnvironment;
        private readonly TransactionalEmailsApi _apiInstance;
        private readonly IConfiguration _configuration;

        public EmailService(IAppEnvironment appEnvironment, TransactionalEmailsApi transactionalEmailsApi, 
            IConfiguration configuration)
        {
            _appEnvironment = appEnvironment;
            _apiInstance = transactionalEmailsApi;
            _configuration = configuration;
        }

        private void SendEmail(SendEmailDto emailDto)
        {
            SendSmtpEmailSender sender = new SendSmtpEmailSender(emailDto.SenderName, emailDto.SenderEmail);
            var recipients = new List<SendSmtpEmailTo>
            {
                new SendSmtpEmailTo(emailDto.ReceiverEmail, emailDto.ReceiverName)
            };

            try
            {
                var sendSmtpEmail = new SendSmtpEmail(sender, recipients, null, null, emailDto.HtmlContent, null, emailDto.Subject);
                CreateSmtpEmail result = _apiInstance.SendTransacEmail(sendSmtpEmail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending email: {ex.Message}");
                throw; // Rethrow to let global exception handling take over
            }
        }

        public void SendEmail(string templateName, Dictionary<string, string> replacements, string receiverEmail, string receiverName, string subject)
        {
            var sendEmailDto = new SendEmailDto
            {
                SenderEmail = _configuration["BrevoEmailApi:SenderEmail"]!,
                SenderName = _configuration["BrevoEmailApi:SenderName"]!,
                ReceiverEmail = receiverEmail,
                ReceiverName = receiverName,
                Subject = subject,
                HtmlContent = CreateBody(templateName, replacements)
            };

            SendEmail(sendEmailDto);
        }

        public string CreateBody(string templateFileName, Dictionary<string, string> replacements)
        {
            var filePath = Path.Combine(_appEnvironment.WebRootPath, "html", templateFileName);
            if (!File.Exists(filePath))
            {
                return "Path not found";
            }

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(filePath))
            {
                body = reader.ReadToEnd();
            }

            foreach (var replacement in replacements)
            {
                body = body.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
            }

            return body;
        }

    }
}
