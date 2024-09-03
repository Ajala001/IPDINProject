using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace App.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApiResponse<UserResponseDto>> GetUserAsync(string email);
        Task<ApiResponse<IEnumerable<UserResponseDto>>> GetUsersAsync();
        Task<ApiResponse<UserResponseDto>> DeleteAsync(string email);
        Task<ApiResponse<UserResponseDto>> UpdateAsync(string email, UpdateUserRequestDto request);
    }
}
