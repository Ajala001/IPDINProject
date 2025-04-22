using App.Application.IExternalServices;
using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;

namespace App.Application.Services
{
    public class UserService(UserManager<User> userManager, IFileRepository fileRepository,
        IHttpContextAccessor httpContextAccessor, ILevelRepository levelRepository, IConfiguration configuration, IEmailService emailService,
        IUnitOfWork unitOfWork, IAcademicQualificationRepository qualificationRepository) : IUserService
    {
        private static readonly SemaphoreSlim membershipNumberSemaphore = new SemaphoreSlim(1, 1);
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
            var user = await userManager.Users
                    .Include(u => u.UserAcademicQualifications)
                    .ThenInclude(uaq => uaq.Qualification)
                    .Include(u => u.Level) // Assuming there's a navigation property for Level
                    .FirstOrDefaultAsync(u => u.Email == email);

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
                Data = await UserResponseDto(user, await userManager.GetRolesAsync(user))
            };
        }

        public async Task<PagedResponse<IEnumerable<UserResponseDto>>> GetUsersAsync(int pageSize, int pageNumber)
        {
            var users = await userManager.Users
                   .Include(u => u.UserAcademicQualifications)
                   .ThenInclude(uaq => uaq.Qualification)
                   .Include(u => u.Level) // Assuming there's a navigation property for Level
                   .ToListAsync()
                ?? throw new Exception("No User Found");

            var responseData = new List<UserResponseDto>();
            // If pageSize and pageNumber are not provided (null or 0), return all courses without pagination
            if (pageSize == 0 || pageNumber == 0)
            {
                foreach (var user in users)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    responseData.Add(await UserResponseDto(user, roles));
                }

                return new PagedResponse<IEnumerable<UserResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "Users Retrieved Successfully",
                    TotalRecords = users.Count(),
                    Data = responseData
                };
            }


            var totalRecords = users.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // If pageNumber exceeds total pages, return an empty response
            if (pageNumber > totalPages)
            {
                return new PagedResponse<IEnumerable<UserResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "No more user available",
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    Data = new List<UserResponseDto>()
                };
            }

            // Paginate the users
            var paginatedUsers = users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResponseData = new List<UserResponseDto>();
            foreach (var user in paginatedUsers)
            {
                var roles = await userManager.GetRolesAsync(user);
                paginatedResponseData.Add(await UserResponseDto(user, roles));
            }

            return new PagedResponse<IEnumerable<UserResponseDto>>
            {
                IsSuccessful = true,
                Message = "Users Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = paginatedResponseData
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
                var imageUpload = await fileRepository.UploadAsync(request.ProfilePic, httpContextAccessor.HttpContext.Request);
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
                Data = await UserResponseDto(user, await userManager.GetRolesAsync(user))
            };
        }



        private async Task<UserResponseDto> UserResponseDto(User user, IList<string> roles)
        {
            var userLevel = await levelRepository.GetLevelAsync(l => l.Id == user.LevelId);
            return new UserResponseDto
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                RoleNames = roles.ToList(),
                MembershipNumber = user.MembershipNumber ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Gender = user.Gender,
                Level = userLevel.Name,
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

        public async Task<PagedResponse<IEnumerable<UserResponseDto>>> SearchUserAsync(SearchQueryRequestDto request)
        {
            var users = await userManager.Users
                    .Include(u => u.UserAcademicQualifications)
                    .ThenInclude(uaq => uaq.Qualification)
                    .Include(u => u.Level) // Assuming there's a navigation property for Level
                    .ToListAsync();

            var searchedUsers = users.Where(user =>
                !string.IsNullOrEmpty(request.SearchQuery) &&
                (
                    user.FirstName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    user.LastName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||  
                    user.MembershipNumber != null && user.MembershipNumber.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) || 
                    user.StateOfResidence.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) || 
                    (user.City != null && user.City.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)) || 
                    (user.LocalGovt != null && user.LocalGovt.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)) || 
                    (user.StateOfOrigin != null && user.StateOfOrigin.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)) || 
                    user.Country.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    user.DriverLicenseNo.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    user.Gender.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    user.Level!.ToString()!.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                )
            ).ToList();


            if (!searchedUsers.Any()) return new PagedResponse<IEnumerable<UserResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            int pageSize = request.PageSize > 0 ? request.PageSize : 5;
            int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;

            var totalRecords = searchedUsers.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);


            var paginatedUsers = searchedUsers
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();


            var paginatedResponseData = new List<UserResponseDto>();
            foreach (var user in paginatedUsers)
            {
                var roles = await userManager.GetRolesAsync(user);
                paginatedResponseData.Add(await UserResponseDto(user, roles));
            }

            return new PagedResponse<IEnumerable<UserResponseDto>>
            {
                IsSuccessful = true,
                Message = "Users Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = paginatedResponseData
            };
        }

        public async Task<ApiResponse<UserResponseDto>> AddAdminAsync(AddAdminDto addAdminDto)
        {
            var existingUser = await userManager.FindByEmailAsync(addAdminDto.Email);
            if (existingUser != null)
            {
                return new ApiResponse<UserResponseDto>
                {
                    IsSuccessful = false,
                    Message = $"User with email {existingUser.Email} already exists"
                };
            }

            using (var transaction = await unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var newUser = new User
                    {
                        Id = Guid.NewGuid(),
                        FirstName = addAdminDto.FirstName,
                        LastName = addAdminDto.LastName,
                        Email = addAdminDto.Email,
                        UserName = addAdminDto.Email,
                        LevelId = addAdminDto.LevelId,
                        Gender = addAdminDto.Gender,
                        StateOfResidence = addAdminDto.StateOfResidence,
                        Country = addAdminDto.Country,
                        DriverLicenseNo = addAdminDto.DriverLicenseNo,
                        YearsOfExperience = addAdminDto.YearsOfExperience,
                        NameOfCurrentDrivingSchool = addAdminDto.NameOfCurrentDrivingSchool,
                        CreatedBy = addAdminDto.Email,
                        CreatedOn = DateTime.Now
                    };

                    var result = await userManager.CreateAsync(newUser, "Admin@01");
                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
                    }

                    // Add qualifications logic
                    foreach (var qualificationDto in addAdminDto.AcademicQualifications)
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

                        newUser.UserAcademicQualifications.Add(new UserAcademicQualifications
                        {
                            Id = Guid.NewGuid(),
                            UserId = newUser.Id,
                            QualificationId = qualification.Id,
                            User = newUser,
                            Qualification = qualification
                        });
                    }

                    await unitOfWork.SaveAsync();
                    await transaction.CommitAsync();

                    await AssignRoleAndSendConfirmationEmailAsync(newUser, "Admin");

                    return new ApiResponse<UserResponseDto>
                    {
                        IsSuccessful = true,
                        Message = "Registration successful and a confirmation email sent to you"
                    };
                }
                catch (Exception ex)
                {
                    if (transaction.GetDbTransaction().Connection != null)
                    {
                        await transaction.RollbackAsync();
                    }
                    return new ApiResponse<UserResponseDto>
                    {
                        IsSuccessful = false,
                        Message = $"Registration failed due to an error: {ex.Message}"
                    };
                }
            }
        }


        private async Task AssignRoleAndSendConfirmationEmailAsync(User user, string role)
        {
            await userManager.AddToRoleAsync(user, role);
            var confirmEmailToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

            string url = $"{configuration["AppUrl"]}/api/auth/confirmEmail?email={user.Email}&token={validEmailToken}";
            string userFullName = $"{user.FirstName} {user.LastName}";

            var replacements = new Dictionary<string, string>
            {
                { "UserName", userFullName },
                { "AppName", "IPDIN Driving Institute" },
                { "ConfirmationLink", url },
                { "MembershipNo", user.MembershipNumber! },
                { "Password", configuration["AdminDefaultPass"]! }
            };

            var membershipNum = await GenerateMembershipNumberAsync();
            user.MembershipNumber = membershipNum;
            await userManager.UpdateAsync(user);
            await unitOfWork.SaveAsync();

            emailService.SendEmail(
                "AdminConfirmationEmail.html", 
                replacements, user.Email!, 
                userFullName, "Confirm Your Email"
                );
        }

        private async Task<string> GenerateMembershipNumberAsync()
        {
            await membershipNumberSemaphore.WaitAsync();
            try
            {
                // Fetch the list of users in the "Admin" role
                var adminUsers = await userManager.GetUsersInRoleAsync("Admin");
                int adminCount = adminUsers.Count + 1; // New admin's number is count + 1

                string year = DateTime.Now.Year.ToString();
                string uniqueId = adminCount.ToString("D4"); // Zero-padded four-digit unique ID

                return $"ADM/{year}/{uniqueId}";
            }
            finally
            {
                membershipNumberSemaphore.Release();
            }
        }
    }
}