using App.Core.Enums;

namespace App.Core.Entities
{
    public class Course : Auditables
    {
        public Guid ExaminationId { get; set; }
        public required string CourseCode { get; set; }
        public required string CourseTitle { get; set; }
        public required string CourseUnit { get; set; }
        public CourseStatus Status { get; set; }
        public Examination? Examination { get; set; }
        public ICollection<UserCourses> Courses { get; set; } = new List<UserCourses>();
    }
}
