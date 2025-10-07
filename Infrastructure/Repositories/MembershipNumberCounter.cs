using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;

namespace App.Infrastructure.Repositories
{
    public class MembershipNumberCounter(IPDINDbContext dbContext) : IMembershipNumberCounters
    {
        public async Task AddCounterAsync(Core.Entities.MembershipNumberCounter counter)
        {
            await dbContext.MembershipNumberCounters.AddAsync(counter);
        }

        public async Task<Core.Entities.MembershipNumberCounter> GetMembershipCounterAsync(Expression<Func<Core.Entities.MembershipNumberCounter, bool>> predicate)
        {
            var counter = await dbContext.MembershipNumberCounters.FirstOrDefaultAsync(predicate);
            return counter;
        }
    }
}
