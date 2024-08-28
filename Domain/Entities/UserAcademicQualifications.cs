using App.Core.Interfaces;

namespace App.Core.Entities
{
    public class UserAcademicQualifications
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid QualificationId { get; set; }
        public required User User { get; set; }
        public required AcademicQualification Qualification { get; set; }
    }
}
