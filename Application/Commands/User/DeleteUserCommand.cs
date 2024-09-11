using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.User
{
    public record DeleteUserCommand(string Email) : IRequest<ApiResponse<UserResponseDto>>;

    public class DeleteUserCommandHandler(IUserService userService)
        : IRequestHandler<DeleteUserCommand, ApiResponse<UserResponseDto>>
    {
        public async Task<ApiResponse<UserResponseDto>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await userService.DeleteAsync(request.Email);
        }
    }
}
