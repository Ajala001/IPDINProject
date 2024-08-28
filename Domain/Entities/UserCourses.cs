using App.Core.Interfaces;

namespace App.Core.Entities
{
    public class UserCourses
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public required User User { get; set; }
        public required Course Course { get; set; }
    }
}