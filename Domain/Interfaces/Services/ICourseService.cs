﻿using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;

namespace App.Core.Interfaces.Services
{
    public interface ICourseService
    {
        Task<ApiResponse<CourseResponseDto>> CreateAsync(CreateCourseRequestDto request);
        Task<ApiResponse<CourseResponseDto>> UpdateAsync(Guid id, UpdateCourseRequestDto request);
        Task<ApiResponse<CourseResponseDto>> DeleteAsync(Guid id);
        Task<ApiResponse<IEnumerable<CourseResponseDto>>> GetCoursesAsync();
        Task<ApiResponse<CourseResponseDto>> GetCourseAsync(Guid id);
        Task<ApiResponse<IEnumerable<CourseResponseDto>>> SearchCourseAsync(CourseSearchRequestDto request);
    }
}