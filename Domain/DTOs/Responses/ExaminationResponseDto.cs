using App.Core.Entities;

namespace App.Core.DTOs.Responses
{
    public class ExaminationResponseDto
    {
        public Guid Id { get; set; }
        public string ExamTitle { get; set; } = null!;
        public string ExamDate { get; set; } = null!;
        public string ExamTime { get; set; } =null!;
        public short ExamYear { get; set; } 
        public decimal Fee { get; set; }
        public ICollection<CourseResponseDto> Courses { get; set; } = new List<CourseResponseDto>();
    }
}
