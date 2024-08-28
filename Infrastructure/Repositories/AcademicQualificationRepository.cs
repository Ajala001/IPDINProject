using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace App.Infrastructure.Repositories
{
    public class AcademicQualificationRepository(IPDINDbContext dbContext) : IAcademicQualificationRepository
    {
        private readonly IPDINDbContext _dbContext = dbContext;

        public async Task<AcademicQualification> CreateAsync(AcademicQualification qualification)
        {
            await _dbContext.AcademicQualifications.AddAsync(qualification);
            return qualification;
        }

        public void Delete(AcademicQualification qualification)
        {
            _dbContext.AcademicQualifications.Remove(qualification);
        }

        public async Task<AcademicQualification> GetQualificationAsync(Expression<Func<AcademicQualification, bool>> predicate)
        {
            return await _dbContext.AcademicQualifications.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<AcademicQualification>> GetQualificationsAsync()
        {
            return await _dbContext.AcademicQualifications.ToListAsync();
        }

        public AcademicQualification Update(AcademicQualification qualification)
        {
            _dbContext.AcademicQualifications.Update(qualification);
            return qualification;
        }
    }
}
