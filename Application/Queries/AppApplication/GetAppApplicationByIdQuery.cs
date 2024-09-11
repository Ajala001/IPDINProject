using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.AppApplication
{
    public record GetAppApplicationByIdQuery(Guid ApplicationId) : IRequest<ApiResponse<AppApplicationResponseDto>>;

    public class GetAppApplicationByIdQueryHandler(IAppApplicationService appApplicationService)
        : IRequestHandler<GetAppApplicationByIdQuery, ApiResponse<AppApplicationResponseDto>>
{
        public async Task<ApiResponse<AppApplicationResponseDto>> Handle(GetAppApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            return await appApplicationService.GetAppApplicationAsync(request.ApplicationId);
        }
    }
}
