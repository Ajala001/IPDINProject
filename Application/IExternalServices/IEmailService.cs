using App.Core.DTOs.Requests.CreateRequestDtos;

namespace App.Application.IExternalServices
{
    public interface IEmailService
    {
        string CreateBody(string templateFileName, Dictionary<string, string> replacements);
        void SendEmail(string templateName, Dictionary<string, string> replacements, 
                        string receiverEmail, string receiverName, string subject);
    }
}
