using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Interfaces.Repositories
{
    public interface IMembershipNumberCounters
    {
        Task AddCounterAsync(MembershipNumberCounter counter);
        Task<MembershipNumberCounter> GetMembershipCounterAsync(Expression<Func<MembershipNumberCounter, bool>> predicate);
    }
}
