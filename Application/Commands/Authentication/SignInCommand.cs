using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Commands.Authentication
{
    public record SignInCommand(SignInRequestDto SignInRequest) : IRequest<ApiResponse<string>>;

    public class SignInCommandHandler(IAuthService authService) : IRequestHandler<SignInCommand, ApiResponse<string>>
    {
        public Task<ApiResponse<string>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            return authService.SignInAsync(request.SignInRequest);
        }
    }

}
