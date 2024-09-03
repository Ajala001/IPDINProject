using App.Core.Enums;

namespace App.Core.DTOs.Responses
{
    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public string PayerFullName { get; set; } = string.Empty;
        public required decimal Amount { get; set; }
        public required string PaymentRef { get; set; }
        public required string PaymentFor { get; set; }
        public DateTime CreatedAt { get; set; }
        public PaymentStatus Status { get; set; }
    }
}

