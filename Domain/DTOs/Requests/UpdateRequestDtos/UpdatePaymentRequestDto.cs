using App.Core.Enums;

namespace App.Core.DTOs.Requests.UpdateRequestDtos
{
    public class UpdatePaymentRequestDto
    {
        public string? ReasonForPayment { get; set; }
        public PaymentStatus? Status { get; set; }
    }
}
