using App.Core.Enums;

namespace App.Core.DTOs.Requests.UpdateRequestDtos
{
    public class UpdateCourseRequestDto
    {
        public string? CourseCode { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseUnit { get; set; }
        public CourseStatus? Status { get; set; }
    }
}
