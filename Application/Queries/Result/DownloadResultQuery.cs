using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Result
{
    public record DownloadResultQuery(Guid ResultId) : IRequest<ApiResponse<byte[]>>;

    public class DownloadResultQueryHandler(IResultService resultService) 
        : IRequestHandler<DownloadResultQuery, ApiResponse<byte[]>>
    {
        public async Task<ApiResponse<byte[]>> Handle(DownloadResultQuery request, CancellationToken cancellationToken)
        {
            return await resultService.GenerateResultAsync(request.ResultId);
        }
    }
}
