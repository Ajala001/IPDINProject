using App.Core.Enums;

namespace App.Core.DTOs.Requests.CreateRequestDtos
{
    public class CreateCourseRequestDto
    {
        public required string CourseCode { get; set; }
        public required string CourseTitle { get; set; }
        public required string CourseUnit { get; set; }
        public required CourseStatus Status { get; set; }
    }
}
