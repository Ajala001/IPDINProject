using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.BatchResult
{
    public record GetAllBatchResultQuery(int PageSize, int PageNumber) 
        : IRequest<PagedResponse<IEnumerable<BulkResultResponseDto>>>;

    public class GetAllBatchResultQueryHandler(IBatchResultService batchResultService)
        : IRequestHandler<GetAllBatchResultQuery, PagedResponse<IEnumerable<BulkResultResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<BulkResultResponseDto>>> Handle(GetAllBatchResultQuery request, CancellationToken cancellationToken)
        {
            return await batchResultService.GetBatchResultsAsync(request.PageSize, request.PageNumber);
        }
    }
}
