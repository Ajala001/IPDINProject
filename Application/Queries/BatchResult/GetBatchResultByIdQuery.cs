using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.BatchResult
{
    public record GetBatchResultByIdQuery(Guid BatchId) : IRequest<ApiResponse<BulkResultResponseDto>>;

    public class GetBatchResultByIdQueryHandler(IBatchResultService batchResultService)
        : IRequestHandler<GetBatchResultByIdQuery, ApiResponse<BulkResultResponseDto>>
    {
        public async Task<ApiResponse<BulkResultResponseDto>> Handle(GetBatchResultByIdQuery request, CancellationToken cancellationToken)
        {
            return await batchResultService.GetBatchResultAsync(request.BatchId);
        }
    }
}
