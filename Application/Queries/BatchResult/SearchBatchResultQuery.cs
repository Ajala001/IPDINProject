using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.BatchResult
{
    public record SearchBatchResultQuery(SearchQueryRequestDto SearchRequestDto)
        : IRequest<PagedResponse<IEnumerable<BulkResultResponseDto>>>;

    public class SearchBatchResultQueryHandler(IBatchResultService batchResultService)
        : IRequestHandler<SearchBatchResultQuery, PagedResponse<IEnumerable<BulkResultResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<BulkResultResponseDto>>> Handle(SearchBatchResultQuery request, CancellationToken cancellationToken)
        {
            return await batchResultService.SearchBatchResultAsync(request.SearchRequestDto);
        }
    }
}
