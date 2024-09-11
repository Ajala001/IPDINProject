using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.User
{
    public record UpdateUserCommand(string Email, UpdateUserRequestDto UpdateUserRequest) 
        : IRequest<ApiResponse<UserResponseDto>>;

    public class UpdateUserCommandHandler(IUserService userService)
        : IRequestHandler<UpdateUserCommand, ApiResponse<UserResponseDto>>
    {
        public async Task<ApiResponse<UserResponseDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return await userService.UpdateAsync(request.Email, request.UpdateUserRequest);
        }
    }
}
