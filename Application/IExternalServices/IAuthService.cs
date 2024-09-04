using App.Core.DTOs.Requests.CreateRequestDtos;
using Microsoft.AspNetCore.Identity;

namespace App.Application.IExternalServices
{
    public interface IAuthService
    {
        Task<string> SignInAsync(SignInRequestDto request);
        Task SignOutAsync();
        Task<IdentityResult> SignUpAsync(SignUpRequestDto request);
    }
}
