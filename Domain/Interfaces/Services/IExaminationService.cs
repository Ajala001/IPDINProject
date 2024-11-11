using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;

namespace App.Core.Interfaces.Services
{
    public interface IExaminationService
    {
        Task<ApiResponse<ExaminationResponseDto>> CreateAsync(CreateExaminationRequestDto request);
        Task<ApiResponse<ExaminationResponseDto>> UpdateAsync(Guid id, UpdateExaminationRequestDto request);
        Task<ApiResponse<ExaminationResponseDto>> DeleteAsync(Guid id);
        Task<PagedResponse<IEnumerable<ExaminationResponseDto>>> GetExaminationsAsync(int pageSize, int pageNumber);
        Task<PagedResponse<IEnumerable<ExaminationResponseDto>>> GetUserExaminationsAsync(int pageSize, int pageNumber);
        Task<ApiResponse<ExaminationResponseDto>> GetExaminationAsync(Guid id);
        Task<PagedResponse<IEnumerable<ExaminationResponseDto>>> SearchExaminationAsync(SearchQueryRequestDto request);
    }
}
