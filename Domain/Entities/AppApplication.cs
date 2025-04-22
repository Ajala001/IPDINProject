using App.Core.Enums;

namespace App.Core.Entities
{
    public class AppApplication : Auditables
    {
        public Guid UserId { get; set; }
        public Guid TrainingId { get; set; }
        public Guid ExaminationId { get; set; }
        public decimal ApplicationFee { get; set; } 
        public bool HasPaid { get; set; } = false;
        public string AppliedFor { get; set; } = string.Empty;
        public DateTime DateApplied { get; set; }
        public ApplicationStatus Status { get; set; }
        public required User User { get; set; }  
    }
}
