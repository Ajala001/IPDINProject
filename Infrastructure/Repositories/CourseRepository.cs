using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Infrastructure.Repositories
{
    public class CourseRepository(IPDINDbContext dbContext) : ICourseRepository
    {
        public async Task<Course> CreateAsync(Course course)
        {
            await dbContext.Courses.AddAsync(course);
            return course;
        }

        public void Delete(Course course)
        {
            dbContext.Courses.Remove(course);
        }

        public async Task<Course> GetCourseAsync(Expression<Func<Course, bool>> predicate)
        {
            return await dbContext.Courses.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            return await dbContext.Courses.ToListAsync();
        }

        public async Task<IEnumerable<Course>> SearchCourseAsync(string courseTitle, string courseCode)
        {
            return await dbContext.Courses.Where(c => c
                .CourseTitle.Contains(courseTitle, StringComparison.OrdinalIgnoreCase) || c.CourseCode.Contains(courseCode))
                .ToListAsync();
        }

        public Course Update(Course course)
        {
            dbContext.Courses.Update(course);
            return course;
        }

        public async Task<ICollection<Course>> GetSelectedAsync(Expression<Func<Course, bool>> predicate)
        {
            var response = await dbContext.Courses
                            .Where(predicate).ToListAsync();
            return response;
        }
    }
}
