using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Infrastructure.Repositories
{
    public class ResultRepository(IPDINDbContext dbContext) : IResultRepository
    {
        public async Task<IEnumerable<Result>> UploadResultAsync(List<Result> results)
        {
            await dbContext.Results.AddRangeAsync(results);
            return results;  
        }

        public void Delete(Result result)
        {
            dbContext.Results.Remove(result);
        }

        public async Task<Result> GetResultAsync(Expression<Func<Result, bool>> predicate)
        {
            return await dbContext.Results.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Result>> GetResultsAsync()
        {
            var result = await dbContext.Results.Include(r => r.Examination)
                                                 .ToListAsync();
            return result;
        }

        public Result Update(Result result)
        {
            dbContext.Results.Update(result);
            return result;
        }
    }
}
