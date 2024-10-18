using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.User
{
    public record GetAllUserQuery(int PageSize, int PageNumber) : IRequest<PagedResponse<IEnumerable<UserResponseDto>>>;

    public class GetAllUserQueryHandler(IUserService userService)
        : IRequestHandler<GetAllUserQuery, PagedResponse<IEnumerable<UserResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<UserResponseDto>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            return await userService.GetUsersAsync(request.PageSize, request.PageNumber);
        }
    }
}
