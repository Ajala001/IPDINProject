using App.Core.DTOs.Responses;

namespace App.Core.Interfaces.Services
{
    public  interface IDashboardService
    {
        Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync();
    }
}
