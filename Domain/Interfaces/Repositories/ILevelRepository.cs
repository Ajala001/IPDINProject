using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Interfaces.Repositories
{
    public interface ILevelRepository
    {
        Task<Level> CreateAsync(Level level);
        Level Update(Level level);
        void Delete(Level level);
        Task<IEnumerable<Level>> GetLevelsAsync();
        Task<Level> GetLevelAsync(Expression<Func<Level, bool>> predicate);
    }
}
