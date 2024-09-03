namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreatePaymentRequestDto
    {
        public Guid EntityId { get; set; }
        public string ReasonForPayment { get; set; } = string.Empty;
    }
}

