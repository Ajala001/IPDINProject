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
    }
}
