using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Infrastructure.Repositories
{
    public class AppApplicationRepository(IPDINDbContext dbContext) : IAppApplicationRepository
    {
        public async Task<AppApplication> CreateAsync(AppApplication application)
        {
            await dbContext.Applications.AddAsync(application);
            return application; 
        }

        public void Delete(AppApplication application)
        {
            dbContext.Applications.Remove(application);
        }

        public void DeleteAll(List<AppApplication> applications)
        {
            dbContext.Applications.RemoveRange(applications);
        }

        public async Task<AppApplication> GetApplicationAsync(Expression<Func<AppApplication, bool>> predicate)
        {
            return await dbContext.Applications.FirstOrDefaultAsync(predicate);
        }

        public IQueryable<AppApplication> GetApplicationsAsync()
        {
            return dbContext.Applications.AsQueryable();
        }

        public AppApplication Update(AppApplication application)
        {
            dbContext.Applications.Update(application);
            return application;
        }
    }
}
