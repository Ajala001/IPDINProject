using App.Core.Entities;
using App.Core.Enums;

namespace App.Core.DTOs.Responses
{
    public class ExaminationResponseDto
    {
        public Guid Id { get; set; }
        public string ExamTitle { get; set; } = null!;
        public string ExamDate { get; set; } = null!;
        public string ExamTime { get; set; } =null!;
        public short ExamYear { get; set; }
        public ExaminationStatus Status { get; set; }
        public string Fee { get; set; } = null!;
        public string ApplicationFee { get; set; } = null!;
        public ICollection<CourseResponseDto> Courses { get; set; } = new List<CourseResponseDto>();
    }
}
