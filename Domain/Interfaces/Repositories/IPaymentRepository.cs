using App.Core.Entities;
using System.Linq.Expressions;

namespace App.Core.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        void Delete(Payment payment);
        Payment Update(Payment payment);
        Task<IEnumerable<Payment>> GetPaymentsAsync();
        Task<Payment> GetPaymentAsync(Expression<Func<Payment, bool>> predicate);
    }
}
