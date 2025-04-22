using App.Core.Enums;

namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreatePaymentRequestDto
    {
        public Guid ServiceId { get; set; } 
        public PaymentType PaymentType { get; set; }
        public string? Token { get; set; } 


    }
}

