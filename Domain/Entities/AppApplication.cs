using App.Core.Enums;

namespace App.Core.Entities
{
    public class AppApplication : Auditables
    {
        public Guid ApplicationId { get; set; }
        public Guid UserId { get; set; }
        public string? ApplicationPurpose { get; set; }
        public DateTime Date { get; set; }
        public ApplicationStatus Status { get; set; }
        public required User User { get; set; }  
    }
}
