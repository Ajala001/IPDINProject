using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace App.Application.Services
{
    public class ExaminationService(IHttpContextAccessor httpContextAccessor, ICourseRepository courseRepository,
        IUnitOfWork unitOfWork, IExaminationRepository examinationRepository, UserManager<User> userManager)
        : IExaminationService
    {
        public async Task<ApiResponse<ExaminationResponseDto>> CreateAsync(CreateExaminationRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var existingExamination = await examinationRepository.GetExaminationAsync(e =>
                                     e.ExamTitle == request.ExamTitle &&
                                     e.ExamYear == request.ExamYear);

            if (existingExamination != null) return new ApiResponse<ExaminationResponseDto>
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
                ExamYear = request.ExamYear,
                Fee = request.Fee,
                ApplicationFee = request.Fee * 0.1m,
                Status = request.Status,
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
                    Status = newExamination.Status,
                    Fee = newExamination.Fee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                    ApplicationFee = newExamination.ApplicationFee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                    Courses = newExamination.Courses.Select(c => new CourseResponseDto
                    {
                        Id = c.Id,
                        CourseTitle = c.CourseTitle,
                        CourseCode = c.CourseCode,
                        CourseUnit = c.CourseUnit,
                        Status = c.Status
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
                IsSuccessful = true,
                Message = "Examination Deleted Successfully",
                Data = null
            };
        }

        public async Task<ApiResponse<ExaminationResponseDto>> GetExaminationAsync(Guid id)
        {
            var examination = await examinationRepository.GetExaminationAsync(e => e.Id == id, true);
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
                    Status = examination.Status,
                    Fee = examination.Fee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                    ApplicationFee = examination.ApplicationFee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                    Courses = examination.Courses.Select(c => new CourseResponseDto
                    {
                        Id = c.Id,
                        CourseTitle = c.CourseTitle,
                        CourseCode = c.CourseCode,
                        CourseUnit = c.CourseUnit,
                        Status = c.Status
                    }).ToList()
                }
            };
        }

        public async Task<PagedResponse<IEnumerable<ExaminationResponseDto>>> GetExaminationsAsync(int pageSize, int pageNumber)
        {
            var examinations = await examinationRepository.GetExaminationsAsync();
            if (examinations == null || !examinations.Any()) return new PagedResponse<IEnumerable<ExaminationResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Records For Examinations",
                Data = null
            };

            var response = PaginatedResponse(pageSize, pageNumber, examinations.ToList());
            return response;
        }

        public async Task<PagedResponse<IEnumerable<ExaminationResponseDto>>> SearchExaminationAsync(SearchQueryRequestDto request)
        {
            // Fetch examinations with included courses
            var examinations = await examinationRepository.GetExaminationsAsync();
            var filteredExaminations = examinations
                .Where(exam =>
                    !string.IsNullOrEmpty(request.SearchQuery) && (
                    exam.Courses.Any(c => c.CourseTitle.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    exam.Courses.Any(c => c.CourseCode.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)) ||
                    (short.TryParse(request.SearchQuery, out short examYear) && exam.ExamYear == examYear) ||
                    exam.Status.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase))
                ).ToList();

            if (!filteredExaminations.Any()) return new PagedResponse<IEnumerable<ExaminationResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            var response = PaginatedResponse(request.PageSize, request.PageNumber, filteredExaminations);
            return response;
        }

        public async Task<ApiResponse<ExaminationResponseDto>> UpdateAsync(Guid id, UpdateExaminationRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var examination = await examinationRepository.GetExaminationAsync(e => e.Id == id, includeCourses: true);
            if (examination == null) return new ApiResponse<ExaminationResponseDto>
            {
                IsSuccessful = false,
                Message = "Examination Not Found",
                Data = null
            };

            // Update examination details
            examination.ExamTitle = request.ExamTitle ?? examination.ExamTitle;
            examination.ExamYear = request.ExamYear ?? examination.ExamYear;
            examination.Fee = request.Fee ?? examination.Fee;
            examination.ExamDateAndTime = request.ExamDateAndTime ?? examination.ExamDateAndTime;
            examination.Status = request.Status ?? examination.Status;
            examination.ModifiedOn = DateTime.UtcNow;
            examination.ModifiedBy = loginUser!;

            // Update courses
            if (request.SelectedCourses != null && request.SelectedCourses.Any())
            {
                // Get all the courses from the repository
                var allCourses = await courseRepository.GetCoursesAsync();
                var selectedCourses = allCourses.Where(c => request.SelectedCourses.Contains(c.Id)).ToList();

                // Replace the courses with the selected ones
                examination.Courses.Clear();
                foreach (var course in selectedCourses)
                {
                    examination.Courses.Add(course);
                }
            }

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
                    Status = examination.Status,
                    Fee = examination.Fee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                    ApplicationFee = examination.ApplicationFee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                    Courses = examination.Courses.Select(c => new CourseResponseDto
                    {
                        Id = c.Id,
                        CourseTitle = c.CourseTitle,
                        CourseCode = c.CourseCode,
                        CourseUnit = c.CourseUnit,
                        Status = c.Status
                    }).ToList()
                }
            };
        }

        public async Task<PagedResponse<IEnumerable<ExaminationResponseDto>>> GetUserExaminationsAsync(int pageSize, int pageNumber)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(loginUser!);
            var userExaminations = await examinationRepository.GetExaminationsAsync(user!);
            if (!userExaminations.Any()) return new PagedResponse<IEnumerable<ExaminationResponseDto>>
            {
                IsSuccessful = false,
                Message = "You have no examinations",
                Data = null
            };

            var response = PaginatedResponse(pageSize, pageNumber, userExaminations.ToList());
            return response;
        }

        private static PagedResponse<IEnumerable<ExaminationResponseDto>> PaginatedResponse(int requestPageSize, int requestPageNumber, List<Examination> examinations)
        {
            // Handle cases where request page size or page number is zero
            if (requestPageSize == 0 || requestPageNumber == 0)
            {
                return new PagedResponse<IEnumerable<ExaminationResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "All examinations available",
                    Data = examinations.Select(examination => new ExaminationResponseDto
                    {
                        Id = examination.Id,
                        ExamTitle = examination.ExamTitle,
                        ExamDate = examination.ExamDateAndTime.ToString("D"),
                        ExamTime = examination.ExamDateAndTime.ToString("t"),
                        ExamYear = examination.ExamYear,
                        Status = examination.Status,
                        Fee = examination.Fee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                        ApplicationFee = examination.ApplicationFee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                        Courses = examination.Courses.Select(c => new CourseResponseDto
                        {
                            Id = c.Id,
                            CourseTitle = c.CourseTitle,
                            CourseCode = c.CourseCode,
                            CourseUnit = c.CourseUnit,
                            Status = c.Status
                        }).ToList()
                    }).ToList()
                };
            }

            // Pagination parameters
            int pageNumber = requestPageNumber > 0 ? requestPageNumber : 1; // Default to page 1 if invalid
            int pageSize = requestPageSize > 0 ? requestPageSize : 5; // Default to page size 5 if invalid

            var totalRecords = examinations.Count;
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // If pageNumber exceeds total pages, return an empty response
            if (pageNumber > totalPages)
            {
                return new PagedResponse<IEnumerable<ExaminationResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "No more examinations available",
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    Data = new List<ExaminationResponseDto>()
                };
            }

            // Paginate the results
            var paginatedExams = examinations
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResponseData = paginatedExams.Select(examination => new ExaminationResponseDto
            {
                Id = examination.Id,
                ExamTitle = examination.ExamTitle,
                ExamDate = examination.ExamDateAndTime.ToString("D"),
                ExamTime = examination.ExamDateAndTime.ToString("t"),
                ExamYear = examination.ExamYear,
                Status = examination.Status,
                Fee = examination.Fee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                ApplicationFee = examination.ApplicationFee.ToString("C2", new System.Globalization.CultureInfo("en-NG")),
                Courses = examination.Courses.Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    CourseCode = c.CourseCode,
                    CourseUnit = c.CourseUnit,
                    Status = c.Status
                }).ToList()
            }).ToList();

            return new PagedResponse<IEnumerable<ExaminationResponseDto>>
            {
                IsSuccessful = true,
                Message = "Examinations retrieved successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = paginatedResponseData
            };
        }
    }
}

