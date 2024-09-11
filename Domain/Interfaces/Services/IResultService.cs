using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using Microsoft.AspNetCore.Http;

namespace App.Core.Interfaces.Services
{
    public interface IResultService
    {
        Task<ApiResponse<IEnumerable<ResultResponseDto>>> UploadResultAsync(IFormFile file);
        Task<ApiResponse<ResultResponseDto>> UpdateAsync(string membershipNumber, UpdateResultRequestDto request);
        Task<ApiResponse<ResultResponseDto>> DeleteAsync(string membershipNumber);
        Task<ApiResponse<IEnumerable<ResultResponseDto>>> GetResultsAsync();
        Task<ApiResponse<ResultResponseDto>> GetResultAsync(string membershipNumber);
    }
}
