using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;

namespace App.Core.Interfaces.Services
{
    public interface IAcademicQualificationService
    {
        Task<ApiResponse<AcademicQualificationResponseDto>> CreateAsync(CreateAcademicQualificationRequestDto request);
        Task<ApiResponse<AcademicQualificationResponseDto>> UpdateAsync(Guid id, UpdateAcademicQualificationRequestDto request);
        Task<ApiResponse<AcademicQualificationResponseDto>> DeleteAsync(Guid id);
        Task<ApiResponse<IEnumerable<AcademicQualificationResponseDto>>> GetQualificationsAsync();
        Task<ApiResponse<AcademicQualificationResponseDto>> GetQualificationAsync(Guid id);
    }
}
