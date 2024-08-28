using App.Core.Interfaces;

namespace App.Core.Entities
{
    public class UserExaminations
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ExaminationId { get; set; }
        public required User User { get; set; }
        public required Examination Examination { get; set; }
    }
}
