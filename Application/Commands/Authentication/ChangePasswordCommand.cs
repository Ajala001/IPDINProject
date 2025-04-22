using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Commands.Authentication
{
    public record ChangePasswordCommand(ChangePasswordRequestDto ChangePassword)
        : IRequest<ApiResponse<UserResponseDto>>;

    public class ChangePasswordCommandHandler(IAuthService authService)
        : IRequestHandler<ChangePasswordCommand, ApiResponse<UserResponseDto>>
    {
        public async Task<ApiResponse<UserResponseDto>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            return await authService.ChangePasswordAsync(request.ChangePassword);
        }
    }
}
