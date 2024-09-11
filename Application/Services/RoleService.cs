using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Services;
using App.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Application.Services
{
    public class RoleService(RoleManager<Role> roleManager, IHttpContextAccessor contextAccessor, 
            UserManager<User> userManager) : IRoleService
    {
        public async Task<ApiResponse<RoleResponseDto>> AddUserRoleAsync(User user, string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null) return new ApiResponse<RoleResponseDto>
            {
                IsSuccessful = false,
                Message = "Role not found"
            };

            var result = await userManager.AddToRoleAsync(user, roleName);
            if(result.Succeeded) return new ApiResponse<RoleResponseDto>
            {
                IsSuccessful = true,
                Message = "Role assigned to User successfully"
            };

            return new ApiResponse<RoleResponseDto>
            {
                IsSuccessful = false,
                Message = "Role not assigned"
            };
        }

        public async Task<ApiResponse<RoleResponseDto>> CreateAsync(CreateRoleRequestDto request)
        {
            var loginUser = contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var role = new Role()
            {
                Id = Guid.NewGuid(),
                Name = request.RoleName,
                Description = request.Description,
                CreatedBy = loginUser!,
                CreatedOn = DateTime.Now
            };
            var roleCreated = await roleManager.CreateAsync(role);
            if(roleCreated.Succeeded) return new ApiResponse<RoleResponseDto>
            {
                IsSuccessful = true,
                Message = "Role created successfully"
            };

            return new ApiResponse<RoleResponseDto>
            {
                IsSuccessful = false,
                Message = "Role not created"
            };

        }

        public async Task<ApiResponse<RoleResponseDto>> DeleteAsync(string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null) return new ApiResponse<RoleResponseDto>
            {
                IsSuccessful = false,
                Message = "Role not found",
                Data = null
            };

            var result = await roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return new ApiResponse<RoleResponseDto>
                {
                    IsSuccessful = true,
                    Message = "Role deleted successfully",
                    Data = null
                };
            }

            return new ApiResponse<RoleResponseDto>
            {
                IsSuccessful = false,
                Message = "Failed to delete role",
                Data = null
            };

        }

        public async Task<ApiResponse<RoleResponseDto>> GetRoleAsync(string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null) return new ApiResponse<RoleResponseDto>
            {
                IsSuccessful = false,
                Message = "Role not found",
                Data = null
            };

            var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
            var fullNames = usersInRole.Select(u => $"{u.FirstName} {u.LastName}").ToList();

            return new ApiResponse<RoleResponseDto>
            {
                IsSuccessful = true,
                Message = "Role Found Successfully",
                Data = new RoleResponseDto
                {
                    Id = role.Id,
                    RoleName = role.Name!,
                    Description = role.Description!,
                    FullNames = fullNames
                }
            };
        }

        public async Task<ApiResponse<IEnumerable<RoleResponseDto>>> GetRolesAsync()
        {
            var roles = await roleManager.Roles.ToListAsync();
            if (roles == null || !roles.Any())
            {
                return new ApiResponse<IEnumerable<RoleResponseDto>>
                {
                    IsSuccessful = false,
                    Message = "No roles found",
                    Data = null
                };
            }

            var roleDtos = new List<RoleResponseDto>();
            foreach (var role in roles)
            {
                var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);
                var fullNames = usersInRole.Select(user => $"{user.FirstName} {user.LastName}").ToList();
                var roleDto = new RoleResponseDto
                {
                    Id = role.Id,
                    RoleName = role.Name!,
                    Description = role.Description!,
                    FullNames = fullNames
                };
                roleDtos.Add(roleDto);
            }

            return new ApiResponse<IEnumerable<RoleResponseDto>>
            {
                IsSuccessful = true,
                Message = "Roles retrieved successfully",
                Data = roleDtos
            };
        }


        public async Task<ApiResponse<RoleResponseDto>> UpdateAsync(string roleName, UpdateRoleRequestDto request)
        {
            var loginUser = contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var role = await roleManager.FindByNameAsync(roleName);
            if(role == null) return new ApiResponse<RoleResponseDto>
            {
                IsSuccessful = false,
                Message = "No roles found",
                Data = null
            };

            role.Name = request.RoleName ?? role.Name;
            role.Description = request.Description ?? role.Description;
            role.ModifiedBy = loginUser!;
            role.ModifiedOn = DateTime.Now;

            var result = await roleManager.UpdateAsync(role);
            if (!result.Succeeded) return new ApiResponse<RoleResponseDto>
            {
                IsSuccessful = false,
                Message = "Failed to update the role",
                Data = null
            };

            return new ApiResponse<RoleResponseDto>
            {
                IsSuccessful = true,
                Message = "Role updated successfully",
                Data = new RoleResponseDto
                {
                    Id = role.Id,
                    RoleName = role.Name!,
                    Description = role.Description!
                }
            };
        }
    }
}
