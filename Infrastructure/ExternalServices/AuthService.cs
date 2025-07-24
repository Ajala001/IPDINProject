using App.Application.IExternalServices;
using App.Application.Services;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;

namespace App.Infrastructure.ExternalServices
{
    public class AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IUnitOfWork unitOfWork,
        IAcademicQualificationRepository qualificationRepository, IConfiguration configuration, IHttpContextAccessor contextAccessor,
        IEmailService emailService, ILevelRepository levelRepository, ITokenService tokenService) : IAuthService
    {
        private static readonly SemaphoreSlim membershipNumberSemaphore = new SemaphoreSlim(1, 1);
        public async Task<ApiResponse<UserResponseDto>> SignUpAsync(SignUpRequestDto request)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = $"User with email {existingUser.Email} already exists"
            };

            var level = await levelRepository.GetLevelAsync(l => l.Id == request.LevelId);
            if (level == null) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "Specified level does not exist."
            };

            using (var transaction = await unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var newUser = new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        UserName = request.Email,
                        LevelId = request.LevelId,
                        Gender = request.Gender,
                        DateOfBirth = request.DateOfBirth,
                        Country = request.Country,
                        StateOfResidence = request.StateOfResidence,
                        DriverLicenseNo = request.DriverLicenseNo,
                        YearIssued = request.YearIssued,
                        ExpiringDate = request.ExpiringDate,
                        YearsOfExperience = request.YearsOfExperience,
                        NameOfCurrentDrivingSchool = request.NameOfCurrentDrivingSchool,
                        CreatedBy = request.Email,
                        CreatedOn = DateTime.Now
                    };
                    var result = await userManager.CreateAsync(newUser, request.Password);
                    if (!result.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse<UserResponseDto>
                        {
                            IsSuccessful = false,
                            Message = string.Join(", ", result.Errors.Select(e => e.Description)) ?? "Registration failed"
                        };
                    }

                    level.Users.Add(newUser);
                    foreach (var qualificationDto in request.AcademicQualifications)
                    {
                        var qualification = new AcademicQualification
                        {
                            Id = Guid.NewGuid(),
                            Degree = qualificationDto.Degree,
                            FieldOfStudy = qualificationDto.FieldOfStudy,
                            Institution = qualificationDto.Institution,
                            YearAttained = qualificationDto.YearAttained,
                            CreatedBy = newUser.Email,
                            CreatedOn = DateTime.Now
                        };
                        await qualificationRepository.CreateAsync(qualification);

                        var userAcademicQualification = new UserAcademicQualifications
                        {
                            Id = Guid.NewGuid(),
                            UserId = newUser.Id,
                            QualificationId = qualification.Id,
                            User = newUser,
                            Qualification = qualification
                        };
                        newUser.UserAcademicQualifications.Add(userAcademicQualification);
                    }

                    await unitOfWork.SaveAsync();
                    await transaction.CommitAsync();
                    await AssignRoleAndSendConfirmationEmailAsync(newUser, "Member");
                    return new ApiResponse<UserResponseDto>
                    {
                        IsSuccessful = true,
                        Message = "Registration successful and a confirmation email sent to you"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ApiResponse<UserResponseDto>
                    {
                        IsSuccessful = false,
                        Message = $"Registration failed due to an error: {ex.Message}"
                    };
                }
            }
        }


        public async Task<ApiResponse<AuthResponse>> SignInAsync(SignInRequestDto request)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.MembershipNumber == request.MembershipNumber);
            if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
                return new ApiResponse<AuthResponse> { IsSuccessful = false, Message = "Invalid credentials. Please check your membership number and password." };


            if (!user.EmailConfirmed) return new ApiResponse<AuthResponse>
            {
                IsSuccessful = false,
                Message = "Please confirm your email before signing in"
            };

            var accessToken = await tokenService.GenerateAccessToken(user);
            return new ApiResponse<AuthResponse>
            {
                IsSuccessful = true,
                Message = "You have Successfully Signed in",
                Data = new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = null!
                }
            };
        }

        public async Task<ApiResponse<string>> SignOutAsync()
        {
            await signInManager.SignOutAsync();
            return new ApiResponse<string>
            {
                IsSuccessful = true,
                Message = "Logged out successfully."
            };
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
            if (result.Succeeded) return new ApiResponse<UserResponseDto>
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

            var replacements = new Dictionary<string, string>
            {
                { "UserName", user.FirstName + " " + user.LastName },
                { "AppName", "IPDIN Driving Institute" },
                { "ConfirmationLink", url },
                { "MembershipNo", user.MembershipNumber! }
            };

            emailService.SendEmail(
                 "ResetPasswordEmail.html",
                 replacements, user.Email!,
                 user.FirstName + " " + user.LastName,
                 "Reset Your Password"
                 );

            return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = true,
                Message = "Reset Password URl has been sent to your Email successfully"
            };
        }


        public async Task<ApiResponse<UserResponseDto>> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await userManager.FindByEmailAsync(request.Email!);
            if (user == null) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "User Not Found",
                Data = null
            };

            if (request.NewPassword != request.ConfirmPassword) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "Password does not match"
            };

            var decodedToken = WebEncoders.Base64UrlDecode(request.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await userManager.ResetPasswordAsync(user, normalToken, request.NewPassword!);

            if (result.Succeeded) return new ApiResponse<UserResponseDto>
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


        private async Task<string> GenerateMembershipNumberAsync()
        {
            await membershipNumberSemaphore.WaitAsync();
            try
            {
                int userCount = await userManager.Users.CountAsync();
                userCount++;
                string year = DateTime.Now.Year.ToString();
                string uniqueId = userCount.ToString("D4");
                return $"MEM/{year}/{uniqueId}";
            }
            finally
            {
                membershipNumberSemaphore.Release();
            }
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

            if (await userManager.IsEmailConfirmedAsync(user)) return new ApiResponse<string>
            {
                IsSuccessful = false,
                Message = "Email already confirmed",
                Data = null
            };

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailToken = Encoding.UTF8.GetBytes(token);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{configuration["AppUrl"]}/api/auth/confirmEmail?email={user.Email}&token={validEmailToken}";
            var replacements = new Dictionary<string, string>
            {
                { "UserName", user.FirstName + " " + user.LastName },
                { "AppName", "IPDIN Driving Institute" },
                { "ConfirmationLink", url },
                { "MembershipNo", user.MembershipNumber! }
            };

            emailService.SendEmail(
                 "ResendConfirmationEmail.html",
                 replacements, user.Email!,
                 user.FirstName + " " + user.LastName,
                 "Confirm Your Email"
                 );

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

            user.MembershipNumber = await GenerateMembershipNumberAsync();
            await userManager.UpdateAsync(user);
            await unitOfWork.SaveAsync();

            var replacements = new Dictionary<string, string>
            {
                { "UserName", userFullName },
                { "MembershipNo", user.MembershipNumber! },
                { "AppName", "IPDIN Driving Institute" },
                { "ConfirmationLink", url },
               
            };
            

            emailService.SendEmail(
                "ConfirmationEmailTemplate.html",
                replacements, user.Email!, userFullName,
                "Confirm Your Email"
                );
        }

        public async Task<ApiResponse<UserResponseDto>> ChangePasswordAsync(ChangePasswordRequestDto request)
        {
            var user = await userManager.FindByEmailAsync(request.Email!);
            if (user == null) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "User not found",
                Data = null
            };

            var isPasswordValid = await userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!isPasswordValid) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "Current password is incorrect"
            };

            if (request.CurrentPassword == request.NewPassword) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "New password cannot be the same as the current password"
            };

            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = true,
                Message = "Password changed successfully"
            };

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = $"Failed to change password: {errors}"
            };
        }

        public async Task<User?> AuthenticateUserAsync(string? token)
        {
            var loggedInEmail = contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(loggedInEmail)) return await userManager.FindByEmailAsync(loggedInEmail);

            if (string.IsNullOrEmpty(token)) return null;

            var (isValid, email) = tokenService.ValidateUserToken(token);
            return isValid ? await userManager.FindByEmailAsync(email!) : null;
        }
        

       

    }
}
