using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Infrastructure.Repositories
{
    public class ResultRepository(IPDINDbContext dbContext) : IResultRepository
    {
       

        public void Delete(Result result)
        {
            dbContext.Results.Remove(result);
        }

        public async Task<Result> GetResultAsync(Expression<Func<Result, bool>> predicate)
        {
            var result = await dbContext.Results
                        .Include(r => r.User)
                        .Include(r => r.BatchResult)
                        .ThenInclude(br => br.Examination)
                        .FirstOrDefaultAsync(predicate);
            return result;
        }

        public async Task<IEnumerable<Result>> GetResultsAsync()
        {
            var result = await dbContext.Results
                         .Include(r => r.User)
                        .Include(r => r.BatchResult)
                        .ThenInclude(br => br.Examination)
                        .ToListAsync();
            return result;
        }

        public Result Update(Result result)
        {
            dbContext.Results.Update(result);
            return result;
        }

        public async Task<IEnumerable<Result>> GetResultsAsync(User user)
        {
            return await dbContext.Results
                                .Include(r => r.User)
                                .Include(r => r.BatchResult)
                                .ThenInclude(br => br.Examination)
                                .Where(r => r.UserId == user.Id)
                                .ToListAsync();
        }

        public async Task<IEnumerable<Result>> GetResultsAsync(BatchResult batchResult)
        {
            return await dbContext.Results
                                .Include(r => r.User)
                                .Include(r => r.BatchResult)
                                .ThenInclude(br => br.Examination)
                                .Where(r => r.BatchId == batchResult.Id)
                                .ToListAsync();
        }

        public Task<IEnumerable<Result>> UploadResultAsync(List<Result> results)
        {
            throw new NotImplementedException();
        }
    }
}
