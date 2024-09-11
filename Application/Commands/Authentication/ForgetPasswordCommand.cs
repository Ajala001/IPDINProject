using App.Application.IExternalServices;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Commands.Authentication
{
    public record ForgetPasswordCommand(string Email) : IRequest<ApiResponse<UserResponseDto>>;

    public class ForgetPasswordCommandHandler(IAuthService authService)
        : IRequestHandler<ForgetPasswordCommand, ApiResponse<UserResponseDto>>
    {
        public async Task<ApiResponse<UserResponseDto>> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            return await authService.ForgetPasswordAsync(request.Email);
        }
    }
}
