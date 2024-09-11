using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Commands.Authentication
{
    public record ConfirmEmailCommand(string userEmail, string token) : IRequest<ApiResponse<UserResponseDto>>;

    public class ConfirmEmailCommandHandler(IAuthService authService)
        : IRequestHandler<ConfirmEmailCommand, ApiResponse<UserResponseDto>>
    {
        public async Task<ApiResponse<UserResponseDto>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            return await authService.ConfirmEmailAsync(request.userEmail, request.token);
        }
    }
}
