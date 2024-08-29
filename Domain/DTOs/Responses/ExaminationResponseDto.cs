namespace App.Core.DTOs.Responses
{
    public class ExaminationResponseDto
    {
        public Guid Id { get; set; }
        public string ExamTitle { get; set; } = null!;
        public string ExamDate { get; set; } = null!;
        public string ExamTime { get; set; } =null!;
        public string ExamYear { get; set; } =null!;
        public string CourseTitle { get; set; } = null!;
        public string CourseCode { get; set; } = null!;
        public decimal Fee { get; set; }
    }
}
