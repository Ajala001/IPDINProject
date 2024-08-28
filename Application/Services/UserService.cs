using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Services
{
    public class UserService : IUserService
    {
        public Task<IdentityResult> CreateAsync(CreateUserRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<UserResponseDto>> DeleteAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRolesAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<UserResponseDto>> GetUserAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<UserResponseDto>>> GetUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task LogOutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> PasswordSignInAsync(User user, LoginRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(User user, UpdateUserRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
