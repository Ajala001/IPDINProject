using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Infrastructure.ExternalServices
{
    public class AuthService(UserManager<User> userManager, SignInManager<User> signInManager,
        IConfiguration configuration) : IAuthService
    {
        public async Task<IdentityResult> SignUpAsync(SignUpRequestDto request)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if(existingUser != null)
            {
                throw new InvalidOperationException("User Already Exist");
            }
              

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                Gender = request.Gender,
                CreatedBy = request.Email,
                CreatedOn = DateTime.Now
            };
            var result = await userManager.CreateAsync(newUser, request.Password);
            return result;
        }

        public async Task<string> SignInAsync(SignInRequestDto request)
        {
            var result = await signInManager.PasswordSignInAsync(request.MembershipNumber, request.Password, request.RememberMe, false);
            if (!result.Succeeded) return null;

            var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Jwt:Key"]));
            var credential = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.MembershipNumber),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:ValidIssuer"],
                audience: configuration["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credential);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task SignOutAsync()
        {
            await signInManager.SignOutAsync();
        }
    }
}
