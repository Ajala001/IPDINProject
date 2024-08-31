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
            var course = await courseRepository.GetCourseAsync(c => c.Id == request.CourseId);
            if (course == null) return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = false,
                Message = "Course Not Found",
                Data = null
            };

            var examination = await examinationRepository.GetExaminationAsync(e =>
                            e.ExamTitle == request.ExamTitle &&
                            e.ExamYear == request.ExamYear &&
                            e.CourseId == request.CourseId
                    );
            if(examination != null) return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = false,
                Message = "Examination Already Exist",
                Data = null
            };

            var newExamination = new Examination
            {
                Id = Guid.NewGuid(),
                ExamTitle = request.ExamTitle,
                ExamDateAndTime = request.ExamDateAndTime,
                Fee = request.Fee,
                CourseId = request.CourseId,
                Course = course,
                CreatedBy = loginUser!,
                CreatedOn = DateTime.UtcNow
            };

            await examinationRepository.CreateAsync(newExamination);
            await unitOfWork.SaveAsync();

            return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = true,
                Message = "Examination Added Succesfully",
                Data = new ExaminationResponseDto
                {
                    Id = newExamination.Id,
                    ExamTitle = newExamination.ExamTitle,
                    ExamDate = newExamination.ExamDateAndTime.ToString("D"),
                    ExamTime = newExamination.ExamDateAndTime.ToString("t"),
                    CourseTitle = newExamination.Course.CourseTitle,
                    CourseCode = newExamination.Course.CourseCode,
                    Fee = newExamination.Fee
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
                Message = "Examination Retrieved Succesfully",
                Data = new ExaminationResponseDto
                {
                    Id = examination.Id,
                    ExamTitle = examination.ExamTitle,
                    ExamDate = examination.ExamDateAndTime.ToString("D"),
                    ExamTime = examination.ExamDateAndTime.ToString("t"),
                    CourseTitle = examination.Course.CourseTitle,
                    CourseCode = examination.Course.CourseCode,
                    Fee = examination.Fee
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
                CourseTitle = examination.Course.CourseTitle,
                CourseCode = examination.Course.CourseCode,
                Fee = examination.Fee
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
            var examinations = await examinationRepository.GetExaminationsAsync();
            var searchedExaminations = examinations.Where(exam =>
            (!string.IsNullOrEmpty(request.CourseTitle) && exam.Course.CourseTitle.Contains(request.CourseTitle, StringComparison.OrdinalIgnoreCase))
            || (!string.IsNullOrEmpty(request.CourseCode) && exam.Course.CourseCode.Contains(request.CourseCode, StringComparison.OrdinalIgnoreCase))
            || (request.ExamYear != default && exam.ExamYear == request.ExamYear)
             );

            if (!searchedExaminations.Any()) return new ApiResponse<IEnumerable<ExaminationResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            int pageNumber = 1;
            int pageSize = 10;
            var paginatedExams = searchedExaminations
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var responseData = paginatedExams.Select(examination => new ExaminationResponseDto
            {
                Id = examination.Id,
                ExamTitle = examination.ExamTitle,
                ExamDate = examination.ExamDateAndTime.ToString("D"),
                ExamTime = examination.ExamDateAndTime.ToString("t"),
                CourseTitle = examination.Course.CourseTitle,
                CourseCode = examination.Course.CourseCode,
                Fee = examination.Fee
            }).ToList();

            return new ApiResponse<IEnumerable<ExaminationResponseDto>>
            {
                IsSuccessful = true,
                Message = "Examinations Retrieved Successfully",
                Data = responseData
            };
        }

        public async Task<ApiResponse<ExaminationResponseDto>> UpdateAsync(Guid id, UpdateExaminationRequestDto request)
        {
            var examination = await examinationRepository.GetExaminationAsync(e => e.Id == id);
            if (examination == null) return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = false,
                Message = "Examination Not Found",
                Data = null
            };

            examination.CourseId = request.CourseId ?? examination.CourseId;
            examination.ExamTitle = request.ExamTitle ?? examination.ExamTitle;
            examination.ExamYear = request.ExamYear ?? examination.ExamYear;
            examination.Fee = request.Fee ?? examination.Fee;
            examination.ExamDateAndTime = request.ExamDateAndTime ?? examination.ExamDateAndTime;

            examinationRepository.Update(examination);
            await unitOfWork.SaveAsync();

            return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = true,
                Message = "Examination Updated Succesfully",
                Data = new ExaminationResponseDto
                {
                    Id = examination.Id,
                    ExamTitle = examination.ExamTitle,
                    ExamDate = examination.ExamDateAndTime.ToString("D"),
                    ExamTime = examination.ExamDateAndTime.ToString("t"),
                    CourseTitle = examination.Course.CourseTitle,
                    CourseCode = examination.Course.CourseCode,
                    Fee = examination.Fee
                }
            };
        }
    }
}

