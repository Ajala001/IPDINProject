using App.Core.Enums;

namespace App.Core.Entities
{
    public class AppApplication : Auditables
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public required string ApplicationPurpose { get; set; } // reason for making the application
        public DateTime Date { get; set; }
        public ApplicationStatus Status { get; set; }
    }
}
