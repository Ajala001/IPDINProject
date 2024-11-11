using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Responses;
using Microsoft.AspNetCore.Http;

namespace App.Core.Interfaces.Services
{
    public interface IBatchResultService
    {
        Task<ApiResponse<BulkResultResponseDto>> UploadBatchResultAsync(IFormFile file, Guid examId);
        Task<PagedResponse<IEnumerable<BulkResultResponseDto>>> GetBatchResultsAsync(int pageSize, int pageNumber);
        Task<PagedResponse<IEnumerable<BulkResultResponseDto>>> SearchBatchResultAsync(SearchQueryRequestDto request);
        Task<ApiResponse<BulkResultResponseDto>> GetBatchResultAsync(Guid batchId);
        Task<ApiResponse<BulkResultResponseDto>> DeleteAsync(Guid batchId);
    }
}


//Task<ApiResponse<StudentResultResponseDto>> UpdateAsync(string membershipNumber, UpdateResultRequestDto request);
//Task<ApiResponse<StudentResultResponseDto>> DeleteAsync(string membershipNumber);