using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Commands.Authentication
{
    public record ResetPasswordCommand(ResetPasswordRequestDto ResetPassword) 
        : IRequest<ApiResponse<UserResponseDto>>;

    public class ResetPasswordCommandHandler(IAuthService authService)
        : IRequestHandler<ResetPasswordCommand, ApiResponse<UserResponseDto>>
    {
        public async Task<ApiResponse<UserResponseDto>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            return await authService.ResetPasswordAsync(request.ResetPassword);
        }
    }
}
