using App.Core.Enums;

namespace App.Core.Entities
{
    public class Course : Auditables
    {
        public required string CourseCode { get; set; }
        public required string CourseTitle { get; set; }
        public required string CourseUnit { get; set; }
        public CourseStatus Status { get; set; }
        public ICollection<Examination> Examinations { get; set; } = new List<Examination>();
        public ICollection<UserCourses> Courses { get; set; } = new List<UserCourses>();
    }
}
