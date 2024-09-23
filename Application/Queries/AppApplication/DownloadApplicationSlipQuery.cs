using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.AppApplication
{
    public record DownloadApplicationSlipQuery(Guid ApplicationId) : IRequest<ApiResponse<byte[]>>;

    public class DownloadApplicationSlipQueryHandler(IAppApplicationService appApplicationService) : IRequestHandler<DownloadApplicationSlipQuery, ApiResponse<byte[]>>
    {
        public async Task<ApiResponse<byte[]>> Handle(DownloadApplicationSlipQuery request, CancellationToken cancellationToken)
        {
            return await appApplicationService.GenerateApplicationSlipAsync(request.ApplicationId);
        }
    }
}
