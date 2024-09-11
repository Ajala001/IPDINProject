using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using MediatR;

namespace App.Application.Commands.Authentication
{
    public record SignUpCommand(SignUpRequestDto SignUpRequest) : IRequest<ApiResponse<UserResponseDto>>;

    public class SignUpCommandHandler(IAuthService authService) :
        IRequestHandler<SignUpCommand, ApiResponse<UserResponseDto>>
    {
        public async Task<ApiResponse<UserResponseDto>> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            return await authService.SignUpAsync(request.SignUpRequest);
        }
    }
}
