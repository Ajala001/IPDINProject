using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Result
{
    public record SearchResultQuery(SearchQueryRequestDto SearchRequestDto)
       : IRequest<PagedResponse<IEnumerable<StudentResultResponseDto>>>;

    public class SearchResultQueryHandler(IResultService resultService)
        : IRequestHandler<SearchResultQuery, PagedResponse<IEnumerable<StudentResultResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<StudentResultResponseDto>>> Handle(SearchResultQuery request, CancellationToken cancellationToken)
        {
            return await resultService.SearchResultAsync(request.SearchRequestDto);
        }
    }
}
