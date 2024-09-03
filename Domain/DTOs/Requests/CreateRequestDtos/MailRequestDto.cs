namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class MailRequestDto
    {
        public string ToEmail { get; set; } = string.Empty; 
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
