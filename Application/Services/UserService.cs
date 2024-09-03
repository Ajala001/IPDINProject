using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Services
{
    public class UserService(SignInManager<User> signInManager, UserManager<User> userManager,
        IFileRepository fileRepository, IHttpContextAccessor _contextAccessor) : IUserService
    {
        public Task<IdentityResult> CreateAsync(CreateUserRequestDto request)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<UserResponseDto>> DeleteAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "User not found",
                Data = null
            };

            var result = await userManager.DeleteAsync(user);
            if(!result.Succeeded) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "Failed to delete",
                Data = null
            };

            return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = true,
                Message = "User Deleted Successfuly",
                Data = null
            };

        }

        public Task<string> GetRolesAsync(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<UserResponseDto>> GetUserAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "User not found",
                Data = null
            };

            var userRole = await userManager.GetRolesAsync(user);
            return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = true,
                Message = "User found",
                Data = new UserResponseDto
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    DriverLicenseNo = user.DriverLicenseNo,
                    
                }
            };
        }

        public Task<ApiResponse<IEnumerable<UserResponseDto>>> GetUsersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task LogOutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public async Task<SignInResult> PasswordSignInAsync(User user, LoginRequestDto request)
        {
            var result = await signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);
            var newResult = result;
            return newResult;
        }

        public async Task<IdentityResult> UpdateAsync(User user, UpdateUserRequestDto request)
        {
            var imageUpload = fileRepository.UploadAsync();
        }
    }
}
