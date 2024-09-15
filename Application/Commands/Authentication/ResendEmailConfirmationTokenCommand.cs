using App.Application.IExternalServices;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Commands.Authentication
{
    public record ResendEmailConfirmationTokenCommand(string Email) : IRequest<ApiResponse<string>>;

    public class ResendEmailConfirmationTokenCommandHandler(IAuthService authService)
        : IRequestHandler<ResendEmailConfirmationTokenCommand, ApiResponse<string>>
{
        public async Task<ApiResponse<string>> Handle(ResendEmailConfirmationTokenCommand request, CancellationToken cancellationToken)
        {
            return await authService.ResendEmailConfirmationToken(request.Email);
        }
    }
}
