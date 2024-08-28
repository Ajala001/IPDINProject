using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;

namespace App.Core.Interfaces.Services
{
    public interface IResultService
    {
        Task<ApiResponse<ResultResponseDto>> CreateAsync(CreateResultRequestDto request);
        Task<ApiResponse<ResultResponseDto>> UpdateAsync(Guid id, UpdateResultRequestDto request);
        Task<ApiResponse<ResultResponseDto>> DeleteAsync(Guid id);
        Task<ApiResponse<IEnumerable<ResultResponseDto>>> GetResultsAsync();
        Task<ApiResponse<ResultResponseDto>> GetResultAsync(Guid id);
    }
}
