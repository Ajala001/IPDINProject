using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using Microsoft.AspNetCore.Http;

namespace App.Core.Interfaces.Services
{
    public interface IResultService
    {
        Task<ApiResponse<StudentResultResponseDto>> UpdateAsync(string membershipNumber, UpdateResultRequestDto request);
        Task<ApiResponse<StudentResultResponseDto>> DeleteAsync(string membershipNumber);
        Task<ApiResponse<IEnumerable<StudentResultResponseDto>>> GetResultsAsync(string membershipNumber);
        Task<ApiResponse<StudentResultResponseDto>> GetResultAsync(Guid resultId);
        Task<PagedResponse<IEnumerable<StudentResultResponseDto>>> SearchResultAsync(SearchQueryRequestDto request);
    }
}
