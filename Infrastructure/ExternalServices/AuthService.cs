using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
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
        IConfiguration configuration, IEmailService emailService, IUnitOfWork unitOfWork) : IAuthService
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
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                UserName = request.Email,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                Country = request.Country,
                StateOfResidence = request.StateOfResidence,
                DriverLicenseNo = request.DriverLicenseNo,
                YearIssued = request.YearIssued,
                ExpiringDate = request.ExpiringDate,
                YearsOfExperience = request.YearsOfExperience,
                NameOfCurrentDrivingSchool = request.NameOfCurrentDrivingSchool,
                RegistrationTypeId = request.RegistrationTypeId,
                CreatedBy = request.Email,
                CreatedOn = DateTime.Now,
            };

            foreach (var qualificationDto in request.AcademicQualifications)
            {
                var qualification = new AcademicQualification
                {
                    Degree = qualificationDto.Degree,
                    FieldOfStudy = qualificationDto.FieldOfStudy,
                    Institution = qualificationDto.Institution,
                    YearAttained = qualificationDto.YearAttained,
                    CreatedBy = newUser.Email,
                    CreatedOn = DateTime.Now
                };

                var userAcademicQualification = new UserAcademicQualifications
                {
                    UserId = newUser.Id,
                    User = newUser,
                    QualificationId = qualification.Id,
                    Qualification = qualification
                };
                newUser.UserAcademicQualifications.Add(userAcademicQualification);
            }
            var result = await userManager.CreateAsync(newUser, request.Password);
            if(result.Succeeded)
            {
                await AssignRoleAndSendConfirmationEmailAsync(newUser, "Member");
                return new ApiResponse<UserResponseDto>
                {
                    IsSuccessful = true,
                    Message = "Registration successful and a confirmation email sent to you"
                };
            }
            return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "Registration Failed"
            };
        }


        public async Task<ApiResponse<string>> SignInAsync(SignInRequestDto request)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.MembershipNumber == request.MembershipNumber);
            if (user == null) return new ApiResponse<string>
            {
                IsSuccessful = false,
                Message = $"User with {request.MembershipNumber} does not exist"
            };

            if (!user.EmailConfirmed) return new ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "Please confirm your email before signing in"
            };

            var result = await signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);
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
                Message = "Email Confirmation Successful"
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
            string url = $"{configuration["AngularUrl"]}/reset-password?email={email}&token={validToken}";

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
                new Claim(ClaimTypes.Email, user.Email)
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

        private async Task<string> GenerateMembershipNumberAsync()
        {
            int userCount = await userManager.Users.CountAsync();
            userCount++;

            string year = DateTime.Now.Year.ToString();
            string uniqueId = userCount.ToString("D4");
            return $"MEM/{year}/{uniqueId}";
        }

        public async Task<ApiResponse<string>> ResendEmailConfirmationToken(string email)
        {
            if (string.IsNullOrEmpty(email)) return new ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "Email is required",
                Data = null
            };

            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return new ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "Email not registered",
                Data = null
            };

            if(await userManager.IsEmailConfirmedAsync(user)) return new ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "Email already confirmed",
                Data = null
            };

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailToken = Encoding.UTF8.GetBytes(token);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{configuration["AppUrl"]}/api/auth/confirmEmail?email={user.Email}&token={validEmailToken}";
            var mailRequestDto = new MailRequestDto
            {
                ToEmail = user.Email!,
                Subject = "Confirm Your Email",
                Body = "<h1>Welcome to IPDIN</h1>" + "<p>Please confirm your email by <a href=\"" + url + "\">Clicking Here</a></p>"
            };
            emailService.SendEmail(emailService.CreateMailMessage(mailRequestDto));
            return new ApiResponse<string>
            {
                IsSuccessful = true,
                Message = "A confirmation link as been sent to you"
            };
        }

        private async Task AssignRoleAndSendConfirmationEmailAsync(User user, string role)
        {
            await userManager.AddToRoleAsync(user, role);
            var confirmEmailToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{configuration["AppUrl"]}/api/auth/confirmEmail?email={user.Email}&token={validEmailToken}";
            string userFullName = $"{user.FirstName} {user.LastName}";
            var mailRequestDto = new MailRequestDto
            {
                ToEmail = user.Email,
                Subject = "Confirm Your Email",
                Body = emailService.CreateBody(userFullName, "IPDIN DrivingSchool", url)
            };
            emailService.SendEmail(emailService.CreateMailMessage(mailRequestDto));
            user.MembershipNumber = await GenerateMembershipNumberAsync();
            await userManager.UpdateAsync(user);
            await unitOfWork.SaveAsync();
        }
    }
}
