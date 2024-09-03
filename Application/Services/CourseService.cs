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
                CourseTitle = request.CourseTitle,
                CourseCode = request.CourseCode,
                CourseUnit = request.CourseUnit,
                Status = Core.Enums.CourseStatus.Active,
                CreatedBy = loginUser!,
                CreatedOn = DateTime.UtcNow,
            };
            await courseRepository.CreateAsync(newCourse);
            await unitOfWork.SaveAsync();

            return new ApiResponse<CourseResponseDto>
            {
                IsSuccessful = true,
                Message = "New Course Addded Successfully",
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

        public async Task<ApiResponse<IEnumerable<CourseResponseDto>>> GetCoursesAsync()
        {
            var courses = await courseRepository.GetCoursesAsync();
            if (!courses.Any()) return new ApiResponse<IEnumerable<CourseResponseDto>>
            {
                IsSuccessful = false,
                Message = "Courses Not Found",
                Data = null
            };

            var responseData = courses.Select(c => new CourseResponseDto
            {
                Id = c.Id,
                CourseTitle = c.CourseTitle,
                CourseCode = c.CourseCode,
                CourseUnit = c.CourseUnit,
                Status = c.Status
            }).ToList();

            return new ApiResponse<IEnumerable<CourseResponseDto>>
            {

                IsSuccessful = true,
                Message = "Courses Retrieved Successfully",
                Data = responseData
            };
        }

        public async Task<ApiResponse<IEnumerable<CourseResponseDto>>> SearchCourseAsync(CourseSearchRequestDto request)
        {
            var courses = await courseRepository.GetCoursesAsync();
            var searchedCourses = courses.Where(course =>
            (!string.IsNullOrEmpty(request.CourseTitle) && course.CourseTitle.Contains(request.CourseTitle, StringComparison.OrdinalIgnoreCase))
            || (!string.IsNullOrEmpty(request.CourseCode) && course.CourseCode.Contains(request.CourseCode, StringComparison.OrdinalIgnoreCase))
             );

            if (!searchedCourses.Any()) return new ApiResponse<IEnumerable<CourseResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            int pageNumber = 1;
            int pageSize = 10;
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

            return new ApiResponse<IEnumerable<CourseResponseDto>>
            {
                IsSuccessful = true,
                Message = "Courses Retrieved Successfully",
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
