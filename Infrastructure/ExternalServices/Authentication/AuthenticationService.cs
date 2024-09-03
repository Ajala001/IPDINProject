using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.ExternalServices.Authentication
{
    public class AuthenticationService(UserManager<User> userManager, SignInManager<User> signInManager) : IAuthenticationService
    {
        public async Task<IdentityResult> RegisterUserAsync(RegistrationRequestDto request)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email)
                ?? throw new InvalidOperationException("User Already Exist");

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                CreatedBy = request.Email,
                CreatedOn = DateTime.Now
            };
            var result = await userManager.CreateAsync(newUser, request.Password);
            return result;
        }

        public async Task<SignInResult> SignInAsync(SignInRequestDto request)
        {
            var result = await signInManager.PasswordSignInAsync(request.MembershipNumber, request.Password, request.RememberMe, false);
            var newResult = result;
            return newResult;
        }

        public async Task SignOutAsync()
        {
            await signInManager.SignOutAsync();
        }
    }
}
