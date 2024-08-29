using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Infrastructure.Repositories
{
    public class ExaminationRepository(IPDINDbContext dbContext) : IExaminationRepository    
    {
        public async Task<Examination> CreateAsync(Examination examination)
        {
            await dbContext.Examinations.AddAsync(examination);
            return examination;
        }

        public void Delete(Examination examination)
        {
            dbContext.Examinations.Remove(examination);
        }

        public async Task<Examination> GetExaminationAsync(Expression<Func<Examination, bool>> predicate)
        {
            var result = await dbContext.Examinations.Include(a => a.Course)
                                                .Include(a => a.Examinations)
                                                .FirstOrDefaultAsync(predicate);
            return result;
        }

        public async Task<IEnumerable<Examination>> GetExaminationsAsync()
        {
            return await dbContext.Examinations.Include(a => a.Course)
                                                .Include(a => a.Examinations)
                                                .ToListAsync();
        }

        public async Task<IEnumerable<Examination>> SearchExaminationAsync(string courseTitle, string courseCode)
        {
            return await dbContext.Examinations.Where(e => e
                    .Course.CourseTitle.Contains(courseTitle, StringComparison.OrdinalIgnoreCase) || e.Course.CourseCode.Contains(courseCode))
                    .ToListAsync();
        }

        public Examination Update(Examination examination)
        {
            dbContext.Examinations.Update(examination);
            return examination; 
        }
    }
}
