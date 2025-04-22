using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Commands.Authentication
{
    public record SignInCommand(SignInRequestDto SignInRequest) : IRequest<ApiResponse<AuthResponse>>;

    public class SignInCommandHandler(IAuthService authService) : IRequestHandler<SignInCommand, ApiResponse<AuthResponse>>
    {
        public Task<ApiResponse<AuthResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            return authService.SignInAsync(request.SignInRequest);
        }
    }

}
