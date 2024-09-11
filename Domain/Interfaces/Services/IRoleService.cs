using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;

namespace App.Core.Interfaces.Services
{
    public interface IRoleService
    {
        Task<ApiResponse<RoleResponseDto>> CreateAsync(CreateRoleRequestDto request);
        Task<ApiResponse<RoleResponseDto>> AddUserRoleAsync(User user, string roleName);
        Task<ApiResponse<RoleResponseDto>> GetRoleAsync(string roleName);
        Task<ApiResponse<RoleResponseDto>> UpdateAsync(string roleName, UpdateRoleRequestDto request);
        Task<ApiResponse<RoleResponseDto>> DeleteAsync(string roleName);
        Task<ApiResponse<IEnumerable<RoleResponseDto>>> GetRolesAsync();
    }
}
