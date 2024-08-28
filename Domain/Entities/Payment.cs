using App.Core.Enums;

namespace App.Core.Entities
{
    public class Payment : Auditables
    {
        public required Guid UserId { get; set; }
        public required decimal Amount { get; set; }
        public required Guid PaymentRef { get; set; }
        public required string PaymentFor { get; set; }
        public PaymentStatus Status { get; set; }
        public required User User { get; set; }
    }
}