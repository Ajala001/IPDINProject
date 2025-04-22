using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.AppApplication
{
    public record GetUserApplicationsQuery(int PageSize, int PageNumber) : IRequest<PagedResponse<IEnumerable<AppApplicationResponseDto>>>;

    public class GetUserApplicationsQueryHandler(IAppApplicationService appApplicationService)
        : IRequestHandler<GetUserApplicationsQuery, PagedResponse<IEnumerable<AppApplicationResponseDto>>>
    {
        public Task<PagedResponse<IEnumerable<AppApplicationResponseDto>>> Handle(GetUserApplicationsQuery request, CancellationToken cancellationToken)
        {
            return appApplicationService.GetUserAppApplicationsAsync(request.PageSize, request.PageNumber);
        }
    }
}
