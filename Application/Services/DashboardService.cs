using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Services
{
    public class DashboardService(ITrainingRepository trainingRepository, IExaminationRepository examinationRepository,
        UserManager<User> userManager) : IDashboardService
    {
        public async Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            var trainings = await trainingRepository.GetTrainingsAsync();
            var users = await userManager.Users.ToListAsync();
            var examinations = await examinationRepository.GetExaminationsAsync();

            var trainingServices = trainings.Select(tr => $"{tr.Title}|{tr.Fee}|{tr.ApplicationFee}").ToList();
            var examinationServices = examinations.Select(ex => $"{ex.ExamTitle}|{ex.Fee}|{ex.ApplicationFee}").ToList();
            var services = string.Join(",", trainingServices.Concat(examinationServices));

            return new ApiResponse<DashboardStatsDto>
            {
                IsSuccessful = true,
                Message = "Services Retrieved",
                Data = new DashboardStatsDto
                {
                    TrainingsCount = trainings.Count(),
                    ExamsCount = examinations.Count(),
                    UsersCount = users.Count,
                    Services = services
                }
            };
        }
    }
}
