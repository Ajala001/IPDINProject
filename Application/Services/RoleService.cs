using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace App.Application.Services
{
    public class RoleService : IRoleService
    {
        public Task<bool> AddUserRoleAsync(User user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(CreateRoleRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<RoleResponseDto>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<RoleResponseDto>> GetRoleAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<RoleResponseDto>>> GetRolesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<RoleResponseDto>> UpdateAsync(UpdateRoleRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
