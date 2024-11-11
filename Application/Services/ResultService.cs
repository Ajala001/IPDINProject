using App.Core.DTOs.Requests.SearchRequestDtos;
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
    public class ResultService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor,
        IResultRepository resultRepository, IUnitOfWork unitOfWork) : IResultService
    {
        
        public async Task<ApiResponse<StudentResultResponseDto>> DeleteAsync(string membershipNumber)
        {
            var result = await resultRepository.GetResultAsync(r => r.User.MembershipNumber == membershipNumber);
            if (result == null) return new ApiResponse<StudentResultResponseDto>
            {
                IsSuccessful = false,
                Message = "Result not found",
                Data = null
            };

            resultRepository.Delete(result);
            await unitOfWork.SaveAsync();

            return new ApiResponse<StudentResultResponseDto>
            {
                IsSuccessful = true,
                Message = "Result deleted successfully",
                Data = null
            };
        }

        public async Task<ApiResponse<StudentResultResponseDto>> GetResultAsync(Guid resultId)
        {
            var result = await resultRepository.GetResultAsync(r => r.Id == resultId);
            if (result == null) return new ApiResponse<StudentResultResponseDto>
            {
                IsSuccessful = false,
                Message = "Result not found",
                Data = null
            };

            return new ApiResponse<StudentResultResponseDto>
            {
                IsSuccessful = true,
                Message = "Result retrieved successfully",
                Data = new StudentResultResponseDto
                {
                    Id = result.Id,
                    FullName = $"{result.User.FirstName} {result.User.LastName}",
                    ExamTitle = result.BatchResult.Examination.ExamTitle,
                    TotalScore = result.TotalScore,
                    Breakdown = result.Breakdown
                }
            };
        }

        public async Task<ApiResponse<IEnumerable<StudentResultResponseDto>>> GetResultsAsync(string membershipNumber)
        {
            var user = await userManager.Users
                        .Where(u => u.MembershipNumber == membershipNumber)
                        .FirstOrDefaultAsync();

            if (user == null) return new ApiResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = false,
                Message = "User not found",
                Data = null
            };

            // Fetch the results associated with the user
            var results = await resultRepository.GetResultsAsync(user);
            if (results == null || !results.Any()) return new ApiResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = false,
                Message = "No results found for the user",
                Data = null
            };

            // Map results to DTO
            var resultDtos = results.Select(result => new StudentResultResponseDto
            {
                Id = result.Id,
                FullName = $"{result.User.FirstName} {result.User.LastName}",
                ExamTitle = result.BatchResult.Examination.ExamTitle,
                TotalScore = result.TotalScore,
                Breakdown = result.Breakdown
            }).ToList();

            // Return the successful response
            return new ApiResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = true,
                Message = "Results retrieved successfully",
                Data = resultDtos
            };
        }


        public async Task<ApiResponse<StudentResultResponseDto>> UpdateAsync(string membershipNumber, UpdateResultRequestDto request)
        {
            var loginUser = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var result = await resultRepository.GetResultAsync(r => r.User.MembershipNumber == membershipNumber
                            && r.BatchResult.ExaminationId == request.ExaminationId);

            if(result == null) return new ApiResponse<StudentResultResponseDto>
            {
                IsSuccessful = false,
                Message = "Result not found",
                Data = null
            };

            result.TotalScore = request.TotalScore ?? result.TotalScore;
            result.Breakdown = request.Breakdown ?? result.Breakdown;
            result.ModifiedBy = loginUser;
            result.ModifiedOn = DateTime.Now;

            resultRepository.Update(result);
            await unitOfWork.SaveAsync();

            return new ApiResponse<StudentResultResponseDto>
            {
                IsSuccessful = true,
                Message = "Result updated successfully",
                Data = new StudentResultResponseDto
                {
                    Id = result.Id,
                    FullName = $"{result.User.FirstName} {result.User.LastName}",
                    ExamTitle = result.BatchResult.Examination.ExamTitle,
                    TotalScore = result.TotalScore,
                    Breakdown = result.Breakdown
                }
            };
        }


        public async Task<PagedResponse<IEnumerable<StudentResultResponseDto>>> SearchResultAsync(SearchQueryRequestDto request)
        {
            var results = await resultRepository.GetResultsAsync();
            var searchResults = results
                                .Where(result => string.IsNullOrEmpty(request.SearchQuery) ||
                                    result.BatchResult.ExaminationId.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                    result.TotalScore.ToString().Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                    result.User.FirstName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                    result.User.LastName.Contains(request.SearchQuery, StringComparison.OrdinalIgnoreCase))
                                .ToList();

            if (!searchResults.Any()) return new PagedResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = false,
                Message = "No Match Found",
                Data = null
            };

            int pageSize = request.PageSize > 0 ? request.PageSize : 5;
            int pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;

            var totalRecords = searchResults.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);


            var paginatedResults = searchResults
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

            var responseData = paginatedResults.Select(result => new StudentResultResponseDto
            {
                Id = result.Id,
                FullName = $"{result.User.FirstName} {result.User.LastName}",
                ExamTitle = result.BatchResult.Examination.ExamTitle,
                TotalScore = result.TotalScore,
                Breakdown = result.Breakdown
            }).ToList();

            return new PagedResponse<IEnumerable<StudentResultResponseDto>>
            {
                IsSuccessful = true,
                Message = "Results Retrieved Successfully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                Data = responseData
            };
        }

    }
}
