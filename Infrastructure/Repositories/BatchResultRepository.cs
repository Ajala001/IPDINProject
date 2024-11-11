using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Infrastructure.Repositories
{
    public class BatchResultRepository(IPDINDbContext dbContext) : IBatchResultRepository
    {
        public async Task<BatchResult> UploadBatchResultAsync(BatchResult result)
        {
            await dbContext.BatchResults.AddAsync(result);
            return result;
        }

        public void Delete(BatchResult result)
        {
            dbContext.BatchResults.Remove(result);
        }

        public async Task<BatchResult> GetBatchResultAsync(Expression<Func<BatchResult, bool>> predicate)
        {
            return await dbContext.BatchResults
                .Include(br => br.Examination)
                .Include(br => br.Results)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<BatchResult>> GetBatchResultsAsync()
        {
            var result = await dbContext.BatchResults
                            .Include(br => br.Results)
                            .ThenInclude(r => r.User)
                            .Include(br => br.Examination)
                            .ToListAsync();
            return result;
        }

        public BatchResult Update(BatchResult result)
        {
            dbContext.BatchResults.Update(result);
            return result;
        }

    }
}
