using App.Core.Enums;

namespace App.Core.Entities
{
    public class AppApplication : Auditables
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid? ExaminationId { get; set; }
        public Guid? TrainingId { get; set; }
        public string? ApplicationPurpose { get; set; }
        public DateTime Date { get; set; }
        public ApplicationStatus Status { get; set; }
    }
}
