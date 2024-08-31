using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;

namespace App.Infrastructure.Repositories
{
    public class UnitOfWork(IPDINDbContext dbContext) : IUnitOfWork
    {
        public async Task<int> SaveAsync()
        {
            return await dbContext.SaveChangesAsync();
        }

        public async Task<int> SaveAsync(CancellationToken stoppingToken)
        {
            return await dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}
