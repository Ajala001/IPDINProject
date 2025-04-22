using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Result
{
    public record GetBatchResultQuery(Guid BatchResultId, int PageSize, int PageNumber) 
        : IRequest<PagedResponse<IEnumerable<StudentResultResponseDto>>>;

    public class GetBatchResultQueryHandler(IResultService resultService)
        : IRequestHandler<GetBatchResultQuery, PagedResponse<IEnumerable<StudentResultResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<StudentResultResponseDto>>> Handle(GetBatchResultQuery request, CancellationToken cancellationToken)
        {
            return await resultService.GetBatchResultsAsync(request.BatchResultId, request.PageSize, request.PageNumber);
        }
    }
}
