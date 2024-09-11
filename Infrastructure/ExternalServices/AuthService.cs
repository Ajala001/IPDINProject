using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Infrastructure.ExternalServices
{
    public class AuthService(UserManager<User> userManager, SignInManager<User> signInManager,
        IConfiguration configuration, IEmailService emailService) : IAuthService
    {
        public async Task<ApiResponse<UserResponseDto>> SignUpAsync(SignUpRequestDto request)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = $"User with email {existingUser.Email} already exist"
            };
           
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
            if(result.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, "Member");
                var confirmEmailToken = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"{configuration["AppUrl"]}/api/auth/confirmEmail?email={newUser.Email}&token={validEmailToken}";
                var mailRequestDto = new MailRequestDto
                {
                    ToEmail = newUser.Email,
                    Subject = "Confirm Your Email",
                    Body = "<h1>Welcome to IPDIN</h1>" + "<p>Please confirm your email by <a href=\"" + url + "\">Clicking Here</a></p>"
                };
                emailService.SendEmail(emailService.CreateMailMessage(mailRequestDto));
                newUser.MembershipNumber = GenerateMembershipNumber();
                await userManager.UpdateAsync(newUser);
            }
            return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = true,
                Message = "Registration successful and a confirmation email sent to you"
            };
        }

        public async Task<ApiResponse<string>> SignInAsync(SignInRequestDto request)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == request.MembershipNumber);
            if (user == null) return new ApiResponse<string>
            {
                IsSuccessful = false,
                Message = $"User with {request.MembershipNumber} does not exist"
            };

            if (!user.EmailConfirmed) return new ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "Please confirm your email before signining in"
            };

            var result = await signInManager.PasswordSignInAsync(request.MembershipNumber, request.Password, request.RememberMe, false);
            if (!result.Succeeded) return new ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "Invalid credentials. Please check your membership number and password."
            };

            return new ApiResponse<string>
            {
                IsSuccessful = true,
                Message = "You have Successfully Signed in",
                Data = await GenerateToken(user)
            };
        }

        public async Task SignOutAsync()
        {
            await signInManager.SignOutAsync();
        }


        private async Task<string> GenerateToken(User user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credential = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("Surname", user.LastName),
                new Claim("GivenName", user.FirstName),
                new Claim("NameIdentifier", user.UserName),
                new Claim("Email", user.Email)
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:ValidIssuer"],
                audience: configuration["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credential);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ApiResponse<UserResponseDto>> ConfirmEmailAsync(string email, string token)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "User Not Found",
                Data = null
            };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await userManager.ConfirmEmailAsync(user, normalToken);
            if(result.Succeeded) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = true,
                Message = "Email Confirmation Successfull"
            };

            return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "Email Confirmation Failed",
                Data = null
            };
        }

        public async Task<ApiResponse<UserResponseDto>> ForgetPasswordAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "User Not Found",
                Data = null
            };

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedEmailToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
            string url = $"{configuration["AppUrl"]}/api/auth/resetPassword?email={email}&token={validToken}";

            var mailRequestDto = new MailRequestDto
            {
                ToEmail = email,
                Subject = "Reset Your Password",
                Body = "<h1>Follow the instructions to Reset Your Password</h1>" + "<p>To reset your password: <a href=\"" + url + "\">Click Here</a></p>"
            };
            emailService.SendEmail(emailService.CreateMailMessage(mailRequestDto));

            return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = true,
                Message = "Reset Password URl has been sent to your Email successfully"
            };
        }

        public async Task<ApiResponse<UserResponseDto>> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "User Not Found",
                Data = null
            };

            if(request.NewPassword != request.ConfirmPassword) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "Password does not match"
            };

            var decodedToken = WebEncoders.Base64UrlDecode(request.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await userManager.ResetPasswordAsync(user, normalToken, request.NewPassword); 
            if(result.Succeeded) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = true,
                Message = "Password reset successful"
            };

            return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "Something went wrong"
            };
        }

        private static string GenerateMembershipNumber()
        {
            Random rand = new Random();
            string letters = new string(Enumerable.Range(0, 3)
                .Select(_ => (char)rand.Next('A', 'Z' + 1)).ToArray());

            int year = rand.Next(2000, 2026);
            return $"{letters}/{year}";
        }
    }
}
