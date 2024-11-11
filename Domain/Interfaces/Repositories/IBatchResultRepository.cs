using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Interfaces.Repositories
{
    public interface IBatchResultRepository
    {
        Task<BatchResult> UploadBatchResultAsync(BatchResult results);
        BatchResult Update(BatchResult result);
        void Delete(BatchResult result);
        Task<IEnumerable<BatchResult>> GetBatchResultsAsync();
        Task<BatchResult> GetBatchResultAsync(Expression<Func<BatchResult, bool>> predicate);
    }
}
