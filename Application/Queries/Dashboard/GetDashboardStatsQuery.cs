using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Dashboard
{
    public record GetDashboardStatsQuery() : IRequest<ApiResponse<DashboardStatsDto>>;

    public class GetDashboardStatsQueryHandler(IDashboardService dashboardService)
        : IRequestHandler<GetDashboardStatsQuery, ApiResponse<DashboardStatsDto>>
    {
        public async Task<ApiResponse<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            return await dashboardService.GetDashboardStatsAsync();
        }
    }
}
