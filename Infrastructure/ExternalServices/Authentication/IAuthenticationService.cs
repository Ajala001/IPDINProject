using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.ExternalServices.Authentication
{
    public interface IAuthenticationService
    {
        Task<SignInResult> SignInAsync(SignInRequestDto request);
        Task SignOutAsync();
        Task<IdentityResult> RegisterUserAsync(RegistrationRequestDto request);
    }
}
