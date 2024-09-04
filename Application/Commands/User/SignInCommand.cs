using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Commands.User
{
    public record SignInCommand(SignInRequestDto SignInRequest) : IRequest<string>;

    public class SignInCommandHandler(IAuthService authService) : IRequestHandler<SignInCommand, string>
    {
        public Task<string> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            return authService.SignInAsync(request.SignInRequest);
        }
    }

}
