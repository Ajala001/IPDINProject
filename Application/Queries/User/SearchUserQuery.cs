using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.User
{
    public record SearchUserQuery(SearchQueryRequestDto SearchRequestDto)
       : IRequest<PagedResponse<IEnumerable<UserResponseDto>>>;

    public class SearchUserQueryHandler(IUserService userService)
        : IRequestHandler<SearchUserQuery, PagedResponse<IEnumerable<UserResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<UserResponseDto>>> Handle(SearchUserQuery request, CancellationToken cancellationToken)
        {
            return await userService.SearchUserAsync(request.SearchRequestDto);
        }
    }
}
