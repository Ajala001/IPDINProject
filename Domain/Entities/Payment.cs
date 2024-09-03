using App.Core.Enums;

namespace App.Core.Entities
{
    public class Payment : Auditables
    {
        public required Guid UserId { get; set; }
        public required decimal Amount { get; set; }
        public required string PaymentRef { get; set; }
        public required string ReasonForPayment { get; set; }
        public PaymentStatus Status { get; set; }
        public required User User { get; set; }
    }
}