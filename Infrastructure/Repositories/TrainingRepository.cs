using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace App.Infrastructure.Repositories
{
    public class TrainingRepository(IPDINDbContext dbContext) : ITrainingRepository
    {
        public async Task<Training> CreateAsync(Training training)
        {
            await dbContext.Trainings.AddAsync(training);  
            return training;
        }

        public void Delete(Training training)
        {
            dbContext.Trainings.Remove(training);
        }

        public async Task<Training> GetTrainingAsync(Expression<Func<Training, bool>> predicate)
        {
            return await dbContext.Trainings.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Training>> GetTrainingsAsync()
        {
            return await dbContext.Trainings.Include(t => t.Trainings).ToListAsync();
        }

        public async Task<IEnumerable<Training>> SearchTrainingAsync(string trainingTitle)
        {
            return await dbContext.Trainings.Where(t => t
                  .Title.Contains(trainingTitle, StringComparison.OrdinalIgnoreCase)).ToListAsync();
        }

        public Training Update(Training training)
        {
            dbContext.Trainings.Update(training);
            return training;
        }
    }
}
