using App.Core.Enums;

namespace App.Core.Entities
{
    public class Examination : Auditables
    {
        public required string ExamTitle { get; set; }
        public DateTime ExamDateAndTime { get; set; }
        public short ExamYear { get; set; }
        public required int Fee { get; set; }
        public BatchResult BatchResult { get; set; } = null!;
        public ExaminationStatus Status { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Result> Results { get; set; } = new List<Result>();
        public ICollection<UserExaminations> Examinations { get; set; } = new List<UserExaminations>();
    }
}
