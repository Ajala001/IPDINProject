using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Interfaces.Services
{
    public interface IRoleService
    {
        Task<IdentityResult> CreateAsync(CreateRoleRequestDto request);
        Task<bool> AddUserRoleAsync(User user, string roleName);
        Task<ApiResponse<RoleResponseDto>> GetRoleAsync(Guid id);
        Task<ApiResponse<RoleResponseDto>> UpdateAsync(UpdateRoleRequestDto request);
        Task<ApiResponse<RoleResponseDto>> DeleteAsync(Guid id);
        Task<ApiResponse<IEnumerable<RoleResponseDto>>> GetRolesAsync();
    }
}
