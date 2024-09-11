using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.AppApplication
{
    public record GetAllAppApplicationsQuery() : IRequest<ApiResponse<IEnumerable<AppApplicationResponseDto>>>;

    public class GetAllAppApplicationHandler(IAppApplicationService appApplicationService)
        : IRequestHandler<GetAllAppApplicationsQuery, ApiResponse<IEnumerable<AppApplicationResponseDto>>>
    {
        public Task<ApiResponse<IEnumerable<AppApplicationResponseDto>>> Handle(GetAllAppApplicationsQuery request, CancellationToken cancellationToken)
        {
            return appApplicationService.GetAppApplicationsAsync();
        }
    }
}
