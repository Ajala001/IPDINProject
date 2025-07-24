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
            var response = await dbContext.Applications
                         .Include(a => a.User)
                         .FirstOrDefaultAsync(predicate);

            return response;
        }

        public async Task<AppApplication> GetApplicationAsync(Guid userId, Guid trainingId, Guid examId)
        {
            var response = await dbContext.Applications
                .FirstOrDefaultAsync(a =>
                    a.UserId == userId &&
                    (trainingId != Guid.Empty && a.TrainingId == trainingId ||
                     examId != Guid.Empty && a.ExaminationId == examId)
                );

            return response;
        }

        public async Task<bool> HasPaidAsync(Guid userId, Guid trainingId, Guid examId)
        {
            var application = await dbContext.Applications
                .FirstOrDefaultAsync(a =>
                    a.UserId == userId &&
                    (trainingId != Guid.Empty && a.TrainingId == trainingId ||
                     examId != Guid.Empty && a.ExaminationId == examId)
                );

            return application != null && application.HasPaid;
        }

        public IQueryable<AppApplication> GetApplicationsAsync()
        {
            return dbContext.Applications
                   .Include(a => a.User)
                   .AsQueryable();
        }

        public IQueryable<AppApplication> GetApplicationsAsync(User user)
        {
            return dbContext.Applications
            .Where(app => app.UserId == user.Id);
        }

        public AppApplication Update(AppApplication application)
        {
            dbContext.Applications.Update(application);
            return application;
        }
    }
}
