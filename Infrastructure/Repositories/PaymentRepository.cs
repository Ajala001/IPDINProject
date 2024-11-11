using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Infrastructure.Repositories
{
    public class PaymentRepository(IPDINDbContext dbContext) : IPaymentRepository
    {
        public async Task<Payment> CreateAsync(Payment payment)
        {
            await dbContext.Payments.AddAsync(payment);
            return payment;
        }

        public void Delete(Payment payment)
        {
            dbContext.Payments.Remove(payment); 
        }

        public async Task<Payment> GetPaymentAsync(Expression<Func<Payment, bool>> predicate)
        {
            var response = await dbContext.Payments.Include(p => p.User).FirstOrDefaultAsync(predicate);
            return response;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsAsync()
        {
            return await dbContext.Payments.Include(p => p.User).ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsAsync(User user)
        {
            return await dbContext.Payments
                        .Include(p => p.User)
                        .Where(p => p.UserId == user.Id)    
                        .ToListAsync();
        }

        public Payment Update(Payment payment)
        {
            dbContext.Payments.Update(payment);
            return payment;
        }
    }
}
