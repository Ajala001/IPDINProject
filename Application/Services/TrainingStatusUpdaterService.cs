using App.Core.Enums;
using App.Core.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App.Application.Services
{
    public class TrainingStatusUpdaterService(IServiceProvider serviceProvider) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var trainingRepository = scope.ServiceProvider.GetRequiredService<ITrainingRepository>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var now = DateTime.Now;

                    var trainings = await trainingRepository.GetTrainingsAsync();
                    var relevantTrainings = trainings
                        .Where(t => (now >= t.StartingDateAndTime && t.Status == TrainingStatus.Scheduled) ||
                                    (now >= t.EndingDateAndTime && t.Status == TrainingStatus.Ongoing))
                        .ToList();

                    foreach (var training in relevantTrainings)
                    {
                        if (now >= training.EndingDateAndTime && training.Status == TrainingStatus.Ongoing)
                        {
                            training.Status = TrainingStatus.Completed;
                        }
                        else if (now >= training.StartingDateAndTime && training.Status == TrainingStatus.Scheduled)
                        {
                            training.Status = TrainingStatus.Ongoing;
                        }
                    }

                    if (relevantTrainings.Any())
                    {
                        await unitOfWork.SaveAsync(stoppingToken);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
