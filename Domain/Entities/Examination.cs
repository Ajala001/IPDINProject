namespace App.Core.Entities
{
    public class Examination : Auditables
    {
        public required string ExamTitle { get; set; }
        public DateTime ExamDateAndTime { get; set; }
        public DateOnly ExamYear { get; set; }
        public required Guid CourseId { get; set; }
        public required Course Course { get; set; }
        public required decimal Fee { get; set; }
        public ICollection<Result> Results { get; set; } = new List<Result>();
        public ICollection<UserExaminations> Examinations { get; set; } = new List<UserExaminations>();
    }
}
