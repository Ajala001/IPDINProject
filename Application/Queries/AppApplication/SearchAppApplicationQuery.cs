using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.AppApplication
{
    public record SearchAppApplicationQuery(SearchQueryRequestDto SearchRequestDto)
        : IRequest<PagedResponse<IEnumerable<AppApplicationResponseDto>>>;

    public class SearchAppApplicationQueryHandler(IAppApplicationService applicationService)
        : IRequestHandler<SearchAppApplicationQuery, PagedResponse<IEnumerable<AppApplicationResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<AppApplicationResponseDto>>> Handle(SearchAppApplicationQuery request, CancellationToken cancellationToken)
        {
            return await applicationService.SearchApplicationAsync(request.SearchRequestDto);
        }
    }
}
