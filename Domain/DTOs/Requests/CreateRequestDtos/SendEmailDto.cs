namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class SendEmailDto
    {
        public string SenderEmail { get; set; } = String.Empty;
        public string SenderName { get; set; } = String.Empty;
        public string ReceiverEmail { get; set; } = String.Empty;
        public string ReceiverName { get; set; } = String.Empty;
        public string Subject { get; set; } = String.Empty;
        public string HtmlContent { get; set; } = String.Empty;
    }
}
