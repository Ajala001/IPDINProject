using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace App.Application.Services
{
    public class AcademicQualificationService(IHttpContextAccessor httpContextAccessor,
        IAcademicQualificationRepository qualificationRepository, IUnitOfWork unitOfWork)
        : IAcademicQualificationService
    {
        public async Task<ApiResponse<AcademicQualificationResponseDto>> CreateAsync(CreateAcademicQualificationRequestDto request)
        {
            //var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var academicQualification = new AcademicQualification
            {
                Id = Guid.NewGuid(),
                Degree = request.Degree,
                Institution = request.Institution,
                FieldOfStudy = request.FieldOfStudy,
                YearAttained = request.YearAttained,
                CreatedBy = "loginUser",
                CreatedOn = DateTime.UtcNow
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Abdbaasit",
                LastName = "Ajala",
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true,
                Gender = Core.Enums.Gender.Male,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now),
                CreatedOn = DateTime.Now,
                CreatedBy = "Admin"
            };

            //var user = await userManager.FindByEmailAsync(loginUser!);
            var userAcademicQualification = new UserAcademicQualifications
            {
                QualificationId = academicQualification.Id,
                Qualification = academicQualification,
                UserId = user!.Id,
                User = user!
            };

            academicQualification.UserAcademicQualifications.Add(userAcademicQualification);
            await qualificationRepository.CreateAsync(academicQualification);
            await unitOfWork.SaveAsync();

            return new ApiResponse<AcademicQualificationResponseDto>
            {
                IsSuccessful = true,
                Message = "Academic Qualification Saved Successfully",
                Data = new AcademicQualificationResponseDto
                {
                    Id = academicQualification.Id,
                    Degree = academicQualification.Degree,
                    Institution = academicQualification.Institution,
                    FieldOfStudy = academicQualification.FieldOfStudy,
                    YearAttained = academicQualification.YearAttained
                }
            };
        }

        public async Task<ApiResponse<AcademicQualificationResponseDto>> DeleteAsync(Guid id)
        {
            var qualification = await qualificationRepository.GetQualificationAsync(q => q.Id == id);
            if (qualification == null) return new ApiResponse<AcademicQualificationResponseDto>
            {
                IsSuccessful = false,
                Message = "Academic Qualification Not Found",
                Data = null
            };

            qualificationRepository.Delete(qualification);
            await unitOfWork.SaveAsync();

            return new ApiResponse<AcademicQualificationResponseDto>
            {
                IsSuccessful = true,
                Message = "Academic Qualification Deleted Successfully",
                Data = null
            };
        }

        public async Task<ApiResponse<AcademicQualificationResponseDto>> GetQualificationAsync(Guid id)
        {
            var qualification =  await qualificationRepository.GetQualificationAsync(q => q.Id == id);
            if (qualification == null) return new ApiResponse<AcademicQualificationResponseDto>
            {
                IsSuccessful = false,
                Message = "Academic Qualification Not Found",
                Data = null
            };

            return new ApiResponse<AcademicQualificationResponseDto>
            {
                IsSuccessful = true,
                Message = "Academic Qualification Retrieved Successfully",
                Data = new AcademicQualificationResponseDto
                {
                    Id = qualification.Id,
                    Degree = qualification.Degree,
                    Institution = qualification.Institution,
                    FieldOfStudy = qualification.FieldOfStudy,
                    YearAttained = qualification.YearAttained
                }
            };
        }

        public async Task<ApiResponse<IEnumerable<AcademicQualificationResponseDto>>> GetQualificationsAsync()
        {
            var qualifications = await qualificationRepository.GetQualificationsAsync();
            if (!qualifications.Any()) return new ApiResponse<IEnumerable<AcademicQualificationResponseDto>>
            {
                IsSuccessful = false,
                Message = "Academic Qualifications Not Found",
                Data = null
            };

            var responseData = qualifications.Select(q => new AcademicQualificationResponseDto
            {
                Id = q.Id,
                Degree = q.Degree,
                Institution = q.Institution,
                FieldOfStudy = q.FieldOfStudy,
                YearAttained = q.YearAttained
            }).ToList();

            return new ApiResponse<IEnumerable<AcademicQualificationResponseDto>>
            {
                IsSuccessful = true,
                Message = "Academic Qualifications Retrieved Successfully",
                Data = responseData
            };
        }

        public async Task<ApiResponse<AcademicQualificationResponseDto>> UpdateAsync(Guid id, UpdateAcademicQualificationRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var qualification = await qualificationRepository.GetQualificationAsync(q => q.Id == id);
            if (qualification == null) return new ApiResponse<AcademicQualificationResponseDto>
            {
                IsSuccessful = false,
                Message = "Academic Qualifications Not Found",
                Data = null
            };

            qualification.Degree = request.Degree ?? qualification.Degree;
            qualification.Institution = request.Institution ?? qualification.Institution;
            qualification.FieldOfStudy = request.FieldOfStudy ?? qualification.FieldOfStudy;
            qualification.YearAttained = request.YearAttained ?? qualification.YearAttained;
            qualification.ModifiedBy = loginUser;
            qualification.ModifiedOn = DateTime.UtcNow;

            qualificationRepository.Update(qualification);
            await unitOfWork.SaveAsync();

            return new ApiResponse<AcademicQualificationResponseDto>
            {
                IsSuccessful = true,
                Message = "Academic Qualification Updated Successfully",
                Data = new AcademicQualificationResponseDto
                {
                    Id = qualification.Id,
                    Degree = qualification.Degree,
                    Institution = qualification.Institution,
                    FieldOfStudy = qualification.FieldOfStudy,
                    YearAttained = qualification.YearAttained
                }
            };
        }
    }
}
