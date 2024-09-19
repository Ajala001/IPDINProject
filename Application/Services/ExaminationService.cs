using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace App.Application.Services
{
    public class ExaminationService(IHttpContextAccessor httpContextAccessor, ICourseRepository courseRepository,
        IUnitOfWork unitOfWork, IExaminationRepository examinationRepository) 
        : IExaminationService
    {
        public async Task<ApiResponse<ExaminationResponseDto>> CreateAsync(CreateExaminationRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var examination = await examinationRepository.GetExaminationAsync(e =>
                            e.ExamTitle == request.ExamTitle &&
                            e.ExamYear == request.ExamYear &&
                            e.Courses == request.Courses
                    );
            if(examination != null) return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = false,
                Message = "Examination Already Exist",
                Data = null
            };


            var courses = await courseRepository.GetSelectedAsync(c => request.Courses.Contains(c.Id));
            if (courses.Count != request.Courses.Count)
            {
                return new ApiResponse<ExaminationResponseDto>
                {
                    IsSuccessful = false,
                    Message = "One or more courses are invalid."
                };
            }
            var newExamination = new Examination
            {
                Id = Guid.NewGuid(),
                ExamTitle = request.ExamTitle,
                ExamDateAndTime = request.ExamDateAndTime,
                Fee = request.Fee,
                Courses = courses,
                CreatedBy = loginUser!,
                CreatedOn = DateTime.UtcNow
            };
            await examinationRepository.CreateAsync(newExamination);
            await unitOfWork.SaveAsync();

            return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = true,
                Message = "Examination Added Successfully",
                Data = new ExaminationResponseDto
                {
                    Id = newExamination.Id,
                    ExamTitle = newExamination.ExamTitle,
                    ExamDate = newExamination.ExamDateAndTime.ToString("D"),
                    ExamTime = newExamination.ExamDateAndTime.ToString("t"),
                    ExamYear = newExamination.ExamYear,
                    Fee = newExamination.Fee,
                    Courses = newExamination.Courses.Select(c => new CourseResponseDto
                    {
                        Id = c.Id,
                        CourseTitle = c.CourseTitle,
                        CourseCode = c.CourseCode,
                        CourseUnit = c.CourseUnit
                    }).ToList()
                }
            };
        }

        public async Task<ApiResponse<ExaminationResponseDto>> DeleteAsync(Guid id)
        {
            var examination = await examinationRepository.GetExaminationAsync(e => e.Id == id);
            if (examination == null) return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = false,
                Message = "Examination Not Found",
                Data = null
            };

            examinationRepository.Delete(examination);
            await unitOfWork.SaveAsync();

            return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = false,
                Message = "Examination Deleted Successfully",
                Data = null
            };
        }

        public async Task<ApiResponse<ExaminationResponseDto>> GetExaminationAsync(Guid id)
        {
            var examination = await examinationRepository.GetExaminationAsync(e => e.Id == id);
            if (examination == null) return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = false,
                Message = "Examination Not Found",
                Data = null
            };

            return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = true,
                Message = "Examination Retrieved Successfully",
                Data = new ExaminationResponseDto
                {
                    Id = examination.Id,
                    ExamTitle = examination.ExamTitle,
                    ExamDate = examination.ExamDateAndTime.ToString("D"),
                    ExamTime = examination.ExamDateAndTime.ToString("t"),
                    ExamYear = examination.ExamYear,
                    Fee = examination.Fee,
                    Courses = examination.Courses.Select(c => new CourseResponseDto
                    {
                        Id = c.Id,
                        CourseTitle = c.CourseTitle,
                        CourseCode = c.CourseCode,
                        CourseUnit = c.CourseUnit
                    }).ToList()
                }
            };
        }

        public async Task<ApiResponse<IEnumerable<ExaminationResponseDto>>> GetExaminationsAsync()
        {
            var examinations = await examinationRepository.GetExaminationsAsync();
            if(!examinations.Any()) return new ApiResponse<IEnumerable<ExaminationResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Records For Examinations",
                Data = null
            };

            var responseData = examinations.Select(examination => new ExaminationResponseDto
            {
                Id = examination.Id,
                ExamTitle = examination.ExamTitle,
                ExamDate = examination.ExamDateAndTime.ToString("D"),
                ExamTime = examination.ExamDateAndTime.ToString("t"),
                ExamYear = examination.ExamYear,
                Fee = examination.Fee,
                Courses = examination.Courses.Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    CourseCode = c.CourseCode,
                    CourseUnit = c.CourseUnit
                }).ToList()
            }).ToList();

            return new ApiResponse<IEnumerable<ExaminationResponseDto>>
            {
                IsSuccessful = true,
                Message = "Examinations Retrieved Successfuly",
                Data = responseData
            };
        }

        public async Task<ApiResponse<IEnumerable<ExaminationResponseDto>>> SearchExaminationAsync(ExaminationSearchRequestDto request)
        {
            // Fetch examinations with included courses
            var examinations = await examinationRepository.GetExaminationsAsync();

            // Filter examinations based on the search criteria
            var filteredExaminations = examinations
                .Where(exam =>
                    (string.IsNullOrEmpty(request.CourseTitle) || exam.Courses.Any(c => c.CourseTitle.Contains(request.CourseTitle, StringComparison.OrdinalIgnoreCase)))
                    && (string.IsNullOrEmpty(request.CourseCode) || exam.Courses.Any(c => c.CourseCode.Contains(request.CourseCode, StringComparison.OrdinalIgnoreCase)))
                    && (request.ExamYear == default || exam.ExamYear == request.ExamYear)
                );

            // Check if there are results
            if (!filteredExaminations.Any())
            {
                return new ApiResponse<IEnumerable<ExaminationResponseDto>>
                {
                    IsSuccessful = false,
                    Message = "No Match Found",
                    Data = null
                };
            }

            // Pagination parameters
            int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1; // Default to page 1 if invalid
            int pageSize = request.PageSize > 0 ? request.PageSize : 10; // Default to page size 10 if invalid

            // Paginate the results
            var paginatedExams = filteredExaminations
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Map to DTOs
            var responseData = paginatedExams.Select(MapToExaminationResponseDto).ToList();

            return new ApiResponse<IEnumerable<ExaminationResponseDto>>
            {
                IsSuccessful = true,
                Message = "Examinations Retrieved Successfully",
                Data = responseData
            };
        }

        // Helper method to map Examination to ExaminationResponseDto
        private ExaminationResponseDto MapToExaminationResponseDto(Examination examination)
        {
            return new ExaminationResponseDto
            {
                Id = examination.Id,
                ExamTitle = examination.ExamTitle,
                ExamDate = examination.ExamDateAndTime.ToString("D"),
                ExamTime = examination.ExamDateAndTime.ToString("t"),
                ExamYear = examination.ExamYear,
                Fee = examination.Fee,
                Courses = examination.Courses.Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    CourseCode = c.CourseCode,
                    CourseUnit = c.CourseUnit
                }).ToList()
            };
        }


        public async Task<ApiResponse<ExaminationResponseDto>> UpdateAsync(Guid id, UpdateExaminationRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var examination = await examinationRepository.GetExaminationAsync(e => e.Id == id);
            if (examination == null) return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = false,
                Message = "Examination Not Found",
                Data = null
            };

            examination.ExamTitle = request.ExamTitle ?? examination.ExamTitle;
            examination.ExamYear = request.ExamYear ?? examination.ExamYear;
            examination.Fee = request.Fee ?? examination.Fee;
            examination.ExamDateAndTime = request.ExamDateAndTime ?? examination.ExamDateAndTime;
            examination.ModifiedOn = DateTime.UtcNow;
            examination.ModifiedBy = loginUser!;

            examinationRepository.Update(examination);
            await unitOfWork.SaveAsync();

            return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = true,
                Message = "Examination Updated Successfully",
                Data = new ExaminationResponseDto
                {
                    Id = examination.Id,
                    ExamTitle = examination.ExamTitle,
                    ExamDate = examination.ExamDateAndTime.ToString("D"),
                    ExamTime = examination.ExamDateAndTime.ToString("t"),
                    ExamYear = examination.ExamYear,
                    Fee = examination.Fee,
                    Courses = examination.Courses.Select(c => new CourseResponseDto
                    {
                        Id = c.Id,
                        CourseTitle = c.CourseTitle,
                        CourseCode = c.CourseCode,
                        CourseUnit = c.CourseUnit
                    }).ToList()
                }
            };
        }
    }
}

