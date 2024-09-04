using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Commands.User
{
    public record SignUpCommand(SignUpRequestDto SignUpRequest) : IRequest<IdentityResult>;

    public class SignUpCommandHandler(IAuthService authService) :
        IRequestHandler<SignUpCommand, IdentityResult>
    {
        public async Task<IdentityResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            return await authService.SignUpAsync(request.SignUpRequest);
        }
    }
}
