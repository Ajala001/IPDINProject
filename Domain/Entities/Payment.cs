using App.Core.Entities;
using App.Core.Enums;

namespace App.Core.Entities
{
    public class Payment : Auditables
    {
        public required Guid UserId { get; set; }
        public Guid ServiceId { get; set; }
        public required decimal Amount { get; set; }
        public required string PaymentRef { get; set; }
        public required PaymentType PaymentType { get; set; }
        public required string PaymentFor { get; set; }
        public PaymentStatus Status { get; set; }
        public required User User { get; set; }
    }
}
