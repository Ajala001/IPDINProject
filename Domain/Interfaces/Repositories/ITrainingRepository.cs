using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Interfaces.Repositories
{
    public interface ITrainingRepository
    {
        Task<Training> CreateAsync(Training training);
        Training Update(Training training);
        void Delete(Training training);
         IQueryable<Training> Query();
        Task<IEnumerable<Training>> GetTrainingsAsync();
        Task<IEnumerable<Training>> GetTrainingsAsync(User user);
        Task<IEnumerable<Training>> SearchTrainingAsync(string trainingTitle);
        Task<Training> GetTrainingAsync(Expression<Func<Training, bool>> predicate);
    }
}
