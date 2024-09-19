using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Interfaces.Repositories
{
    public interface IRegistrationTypeRepository
    {
        Task<RegistrationType> CreateAsync(RegistrationType registrationType);
        RegistrationType Update(RegistrationType registrationType);
        void Delete(RegistrationType registrationType);
        Task<IEnumerable<RegistrationType>> GetRegistrationTypesAsync();
        Task<RegistrationType> GetRegistrationTypeAsync(Expression<Func<RegistrationType, bool>> predicate);
    }
}
