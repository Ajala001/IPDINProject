using App.Core.Enums;

namespace App.Core.DTOs.Responses
{
    public class CourseResponseDto
    {
        public Guid Id { get; set; }
        public string CourseCode { get; set; } = null!;
        public string CourseTitle { get; set; } = null!;
        public string CourseUnit { get; set; } = null!;
        public CourseStatus Status { get; set; }
    }
}
