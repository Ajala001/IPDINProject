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
    public class CourseService(IHttpContextAccessor httpContextAccessor,
        ICourseRepository courseRepository, IUnitOfWork unitOfWork) : ICourseService
    {
        public async Task<ApiResponse<CourseResponseDto>> CreateAsync(CreateCourseRequestDto request)
          {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var course = await courseRepository.GetCourseAsync(c => 
                         c.CourseTitle == request.CourseTitle && 
                         c.CourseCode == request.CourseCode);
            if (course != null) return new ApiResponse<CourseResponseDto>
            {
                IsSuccessful = false,
                Message = "Course With Same Title and Code Exist",
                Data = null
            };

            var newCourse = new Course
            {
                Id = Guid.NewGuid(),
                CourseTitle = request.CourseTitle,
                CourseCode = request.CourseCode,
                CourseUnit = request.CourseUnit,
                Status = request.Status,
                CreatedBy = loginUser!,
                CreatedOn = DateTime.UtcNow,
            };
            await courseRepository.CreateAsync(newCourse);
            await unitOfWork.SaveAsync();

            return new ApiResponse<CourseResponseDto>
            {
                IsSuccessful = true,
                Message = "New Course Added Successfully",
                Data = new CourseResponseDto
                {
                    Id = newCourse.Id,
                    CourseTitle = newCourse.CourseTitle,
                    CourseCode = newCourse.CourseCode,
                    CourseUnit = newCourse.CourseUnit,
                    Status = newCourse.Status
                }
            };
        }

        public async Task<ApiResponse<CourseResponseDto>> DeleteAsync(Guid id)
        {
            var course = await courseRepository.GetCourseAsync(c => c.Id == id);
            if (course == null) return new ApiResponse<CourseResponseDto>
            {
                IsSuccessful = false,
                Message = "Course Not Found",
                Data = null
            };

            courseRepository.Delete(course);
            await unitOfWork.SaveAsync();

            return new ApiResponse<CourseResponseDto>
            {
                IsSuccessful = true,
                Message = "Course Deleted Successfully",
                Data = null
            };
        }

        public async Task<ApiResponse<CourseResponseDto>> GetCourseAsync(Guid id)
        {
            var course = await courseRepository.GetCourseAsync(c => c.Id == id);
            if (course == null) return new ApiResponse<CourseResponseDto>
            {
                IsSuccessful = false,
                Message = "Course Not Found",
                Data = null
            };

            return new ApiResponse<CourseResponseDto>
            {
                IsSuccessful = true,
                Message = "Course Retrieved Successfully",
                Data = new CourseResponseDto
                {
                    Id = course.Id,
                    CourseTitle = course.CourseTitle,
                    CourseCode = course.CourseCode,
                    CourseUnit = course.CourseUnit,
                    Status = course.Status
                }
            };
        }

        public async Task<PagedResponse<IEnumerable<CourseResponseDto>>> GetCoursesAsync(int pageSize, int pageNumber)
       {
            var courses = await courseRepository.GetCoursesAsync();
            if (courses == null || !courses.Any())
            {
                return new PagedResponse<IEnumerable<CourseResponseDto>>
                {
                    IsSuccessful = false,
                    Message = "Courses Not Found",
                    Data = null
                };
            }

            // If pageSize and pageNumber are not provided (null or 0), return all courses without pagination
            if (pageSize == 0 || pageNumber == 0)
            {
                var responseData = courses.Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    CourseTitle = c.CourseTitle,
                    CourseCode = c.CourseCode,
                    CourseUnit = c.CourseUnit,
                    Status = c.Status
                }).ToList();

                return new PagedResponse<IEnumerable<CourseResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "Courses Retrieved Successfully",
                    TotalRecords = courses.Count(),
                    Data = responseData
                };
            }


            var totalRecords = courses.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // If pageNumber exceeds total pages, return an empty response
            if (pageNumber > totalPages)
            {
                return new PagedResponse<IEnumerable<CourseResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "No more courses available",
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    Data = new List<CourseResponseDto>()
                };
            }

            // Paginate the courses
            var paginatedCourses = courses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var paginatedResponseData = paginatedCourses.Select(c => new CourseResponseDto
            {
                Id = c.Id,
                CourseTitle = c.CourseTitle,
                CourseCode = c.CourseCode,
                CourseUnit = c.CourseUnit,
                Status = c.Status
            }).ToList();

            return new PagedResponse<IEnumerable<CourseResponseDto>>
            {
                IsSuccessful = true,
                Message = "Courses Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = paginatedResponseData
            };
        }


        public async Task<PagedResponse<IEnumerable<CourseResponseDto>>> SearchCourseAsync(SearchQueryRequestDto request)
        {
            var courses = await courseRepository.GetCoursesAsync();
            var searchedCourses = courses.Where(course =>
                                !string.IsNullOrEmpty(request.SearchQuery) &&
                                (course.CourseTitle.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                                || course.CourseCode.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase)
                                || course.Status.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase))
                            ).ToList();


            if (!searchedCourses.Any()) return new PagedResponse<IEnumerable<CourseResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            int pageSize = request.PageSize > 0 ? request.PageSize : 5;
            int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;

            var totalRecords = searchedCourses.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

          
            var paginatedCourses = searchedCourses
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var responseData = paginatedCourses.Select(c => new CourseResponseDto
            {
                Id = c.Id,
                CourseTitle = c.CourseTitle,
                CourseCode = c.CourseCode,
                CourseUnit = c.CourseUnit,
                Status = c.Status
            }).ToList();

            return new PagedResponse<IEnumerable<CourseResponseDto>>
            {
                IsSuccessful = true,
                Message = "Courses Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = responseData
            };
        }

        public async Task<ApiResponse<CourseResponseDto>> UpdateAsync(Guid id, UpdateCourseRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var course = await courseRepository.GetCourseAsync(c => c.Id == id);
            if (course == null) return new ApiResponse<CourseResponseDto>
            {
                IsSuccessful = false,
                Message = "Course Not Found",
                Data = null
            };

            course.CourseTitle = request.CourseTitle ?? course.CourseTitle;
            course.CourseCode = request.CourseCode ?? course.CourseCode;
            course.CourseUnit = request.CourseUnit ?? course.CourseUnit;
            course.Status = request.Status ?? course.Status;
            course.ModifiedBy = loginUser!;
            course.ModifiedOn = DateTime.Now;

            courseRepository.Update(course);
            await unitOfWork.SaveAsync();

            return new ApiResponse<CourseResponseDto>
            {
                IsSuccessful = true,
                Message = "Course Updated Successfully",
                Data = new CourseResponseDto
                {
                    Id = course.Id,
                    CourseTitle = course.CourseTitle,
                    CourseCode = course.CourseCode,
                    CourseUnit = course.CourseUnit,
                    Status = course.Status
                }
            };


        }

    }
}
