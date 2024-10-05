using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Application.Services
{
    public class UserService(UserManager<User> userManager, IFileRepository fileRepository,
        IHttpContextAccessor httpContextAccessor) : IUserService
    {
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
            if (!result.Succeeded) return new ApiResponse<UserResponseDto>
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

        public async Task<ApiResponse<UserResponseDto>> GetUserAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "User not found",
                Data = null
            };

            return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = true,
                Message = "User retrieved successfully",
                Data = UserResponseDto(user, await userManager.GetRolesAsync(user))
            };
        }

        public async Task<ApiResponse<IEnumerable<UserResponseDto>>> GetUsersAsync()
        {
            var users = await userManager.Users.ToListAsync()
                ?? throw new Exception("No User Found");

            var userResponseDtos = new List<UserResponseDto>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userResponseDtos.Add(UserResponseDto(user, roles));
            }

            return new ApiResponse<IEnumerable<UserResponseDto>>
            {
                IsSuccessful = true,
                Message = "Users retrieved successfully",
                Data = userResponseDtos
            };
        }

        public async Task<ApiResponse<UserResponseDto>> UpdateAsync(string email, UpdateUserRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = false,
                Message = "User not found",
                Data = null
            };

            if (request.ProfilePic != null)
            {
                var imageUpload = await fileRepository.UploadAsync(request.ProfilePic);
                if (imageUpload != null)
                {
                    user.ProfilePic = imageUpload;
                }
            }

            user.DateOfBirth = request.DateOfBirth ?? user.DateOfBirth;
            user.StreetNo = request.StreetNo ?? user.StreetNo;
            user.StreetName = request.StreetName ?? user.StreetName;
            user.City = request.City ?? user.City;
            user.StateOfResidence = request.StateOfResidence ?? user.StateOfResidence;
            user.LocalGovt = request.LocalGovt ?? user.LocalGovt;
            user.StateOfOrigin = request.StateOfOrigin ?? user.StateOfOrigin;
            user.Country = request.Country ?? user.Country;
            user.DriverLicenseNo = request.DriverLicenseNo ?? user.DriverLicenseNo;
            user.YearIssued = request.YearIssued ?? user.YearIssued;
            user.ExpiringDate = request.ExpiringDate ?? user.ExpiringDate;
            user.YearsOfExperience = request.YearsOfExperience ?? user.YearsOfExperience;
            user.NameOfCurrentDrivingSchool = request.NameOfCurrentDrivingSchool ?? user.NameOfCurrentDrivingSchool;

            user.ModifiedBy = loginUser;
            user.ModifiedOn = DateTime.Now;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new ApiResponse<UserResponseDto> { IsSuccessful = false, Message = "Update not Successful" };

            return new ApiResponse<UserResponseDto>
            {
                IsSuccessful = true,
                Message = "User updated successfully",
                Data = UserResponseDto(user, await userManager.GetRolesAsync(user))
            };
        }



        private static UserResponseDto UserResponseDto(User user, IList<string> roles)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                RoleNames = roles.ToList(),
                MembershipNumber = user.MembershipNumber ?? string.Empty,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                ProfilePic = user.ProfilePic ?? string.Empty,
                Address = $"{user.StreetNo?.ToString() ?? string.Empty}, {user.StreetName} {user.City} {user.StateOfResidence}, {user.Country}",
                LocalGovt = user.LocalGovt ?? string.Empty,
                StateOfOrigin = user.StateOfOrigin ?? string.Empty,
                DriverLicenseNo = user.DriverLicenseNo ?? string.Empty,
                YearIssued = user.YearIssued,
                ExpiringDate = user.ExpiringDate,
                YearsOfExperience = user.YearsOfExperience,
                NameOfCurrentDrivingSchool = user.NameOfCurrentDrivingSchool ?? string.Empty,
                AcademicQualifications = user.UserAcademicQualifications?.Select(u => new AcademicQualificationInfo
                {
                    Degree = u.Qualification.Degree ?? string.Empty,
                    FieldOfStudy = u.Qualification.FieldOfStudy ?? string.Empty
                }).ToList()
            };
        }
    }
}