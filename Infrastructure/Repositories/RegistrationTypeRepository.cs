using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Infrastructure.Repositories
{
    internal class RegistrationTypeRepository(IPDINDbContext dbContext) : IRegistrationTypeRepository
    {
        public async Task<RegistrationType> CreateAsync(RegistrationType registrationType)
        {
            await dbContext.RegistrationTypes.AddAsync(registrationType);
            return registrationType;
        }

        public void Delete(RegistrationType registrationType)
        {
            dbContext.RegistrationTypes.Remove(registrationType);
        }

        public async Task<RegistrationType> GetRegistrationTypeAsync(Expression<Func<RegistrationType, bool>> predicate)
        {
            return await dbContext.RegistrationTypes.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<RegistrationType>> GetRegistrationTypesAsync()
        {
            return await dbContext.RegistrationTypes.Include(r => r.Users).ToListAsync();
        }

        public RegistrationType Update(RegistrationType registrationType)
        {
            dbContext.RegistrationTypes.Update(registrationType);
            return registrationType;
        }
    }
}
