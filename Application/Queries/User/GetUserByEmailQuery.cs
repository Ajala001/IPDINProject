using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.User
{
    public record GetUserByEmailQuery(string Email) : IRequest<ApiResponse<UserResponseDto>>;

    public class GetUserByEmailHandler(IUserService userService)
        : IRequestHandler<GetUserByEmailQuery, ApiResponse<UserResponseDto>>
    {
        public async Task<ApiResponse<UserResponseDto>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            return await userService.GetUserAsync(request.Email);
        }
    }
}
