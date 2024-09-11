using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Course
{
    public record GetAllCourseQuery() : IRequest<ApiResponse<IEnumerable<AppApplicationResponseDto>>>;

    public class GetAllCourseQueryHandler(IAppApplicationService appApplicationService)
        : IRequestHandler<GetAllCourseQuery, ApiResponse<IEnumerable<AppApplicationResponseDto>>>
{
        public async Task<ApiResponse<IEnumerable<AppApplicationResponseDto>>> Handle(GetAllCourseQuery request, CancellationToken cancellationToken)
        {
            return await appApplicationService.GetAppApplicationsAsync();
        }
    }
}
