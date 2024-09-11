using App.Application.IExternalServices;
using MediatR;

namespace App.Application.Commands.Authentication
{
    public record SignOutCommand() : IRequest;

    public class SignOutCommandHandler(IAuthService authService) : IRequestHandler<SignOutCommand>
    {
        public async Task Handle(SignOutCommand request, CancellationToken cancellationToken)
        {
            await authService.SignOutAsync();
        }
    }

}
