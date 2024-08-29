using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;

namespace App.Core.Interfaces.Services
{
    public interface IExaminationService
    {
        Task<ApiResponse<ExaminationResponseDto>> CreateAsync(CreateExaminationRequestDto request);
        Task<ApiResponse<ExaminationResponseDto>> UpdateAsync(Guid id, UpdateExaminationRequestDto request);
        Task<ApiResponse<ExaminationResponseDto>> DeleteAsync(Guid id);
        Task<ApiResponse<IEnumerable<ExaminationResponseDto>>> GetExaminationsAsync();
        Task<ApiResponse<ExaminationResponseDto>> GetExaminationAsync(Guid id);
        Task<ApiResponse<IEnumerable<ExaminationResponseDto>>> SearchExaminationAsync(ExaminationSearchRequestDto request);
    }
}
