namespace App.Core.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveAsync();
    }
}
