using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Interfaces.Repositories
{
    public interface ICourseRepository
    {
        Task<Course> CreateAsync(Course course);
        Course Update(Course course);
        void Delete(Course course);
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task<IEnumerable<Course>> SearchCourseAsync(string courseTitle, string courseCode);
        Task<Course> GetCourseAsync(Expression<Func<Course, bool>> predicate);

    }
}
