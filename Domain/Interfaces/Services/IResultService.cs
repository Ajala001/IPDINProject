using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using Microsoft.AspNetCore.Http;

namespace App.Core.Interfaces.Services
{
    public interface IResultService
    {
        Task<ApiResponse<StudentResultResponseDto>> UpdateAsync(string membershipNumber, UpdateResultRequestDto request);
        Task<ApiResponse<StudentResultResponseDto>> DeleteAsync(Guid resultId);
        Task<PagedResponse<IEnumerable<StudentResultResponseDto>>> GetMemberResultsAsync(string membershipNumber, int pageSize, int pageNumber);
        Task<PagedResponse<IEnumerable<StudentResultResponseDto>>> GetBatchResultsAsync(Guid batchResultId, int pageSize, int pageNumber);
        Task<ApiResponse<StudentResultResponseDto>> GetResultAsync(Guid resultId);
        Task<ApiResponse<byte[]>> GenerateResultAsync(Guid resultId);
        Task<PagedResponse<IEnumerable<StudentResultResponseDto>>> SearchResultAsync(SearchQueryRequestDto request);
    }
}
