using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Interfaces.Repositories
{
    public interface IResultRepository
    {
        Task<Result> CreateAsync(Result result);
        Result Update(Result result);
        void Delete(Result result);
        Task<IEnumerable<Result>> GetResultsAsync();
        Task<Result> GetResultAsync(Expression<Func<Result, bool>> predicate);
    }
}
