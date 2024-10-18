using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Infrastructure.Repositories
{
    internal class LevelRepository(IPDINDbContext dbContext) : ILevelRepository
    {
        public async Task<Level> CreateAsync(Level level)
        {
            await dbContext.Levels.AddAsync(level);
            return level;
        }

        public void Delete(Level level)
        {
            dbContext.Levels.Remove(level);
        }

        public async Task<Level> GetLevelAsync(Expression<Func<Level, bool>> predicate)
        {
            return await dbContext.Levels.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Level>> GetLevelsAsync()
        {
            return await dbContext.Levels.Include(r => r.Users).ToListAsync();
        }

        public Level Update(Level level)
        {
            dbContext.Levels.Update(level);
            return level;
        }
    }
}
