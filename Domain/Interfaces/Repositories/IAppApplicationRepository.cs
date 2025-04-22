using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Interfaces.Repositories
{
    public interface IAppApplicationRepository
    {
        Task<AppApplication> CreateAsync(AppApplication application);
        AppApplication Update(AppApplication application);
        void Delete(AppApplication application);
        void DeleteAll(List<AppApplication> applications);
        IQueryable<AppApplication> GetApplicationsAsync();
        IQueryable<AppApplication> GetApplicationsAsync(User user);
        Task<AppApplication> GetApplicationAsync(Guid userId, Guid trainingId, Guid examId);
        Task<AppApplication> GetApplicationAsync(Expression<Func<AppApplication, bool>> predicate);


    }
}
