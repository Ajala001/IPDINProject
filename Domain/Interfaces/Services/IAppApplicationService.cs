using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Entities;

namespace App.Core.Interfaces.Services
{
    public interface IAppApplicationService
    {
        Task<ApiResponse<AppApplicationResponseDto>> CreateAsync(CreateAppApplicationRequestDto request);
        Task<ApiResponse<AppApplicationResponseDto>> UpdateAsync(Guid id, UpdateAppApplicationRequestDto request);
        Task<ApiResponse<AppApplicationResponseDto>> DeleteAsync(Guid id);
        Task<ApiResponse<IEnumerable<AppApplicationResponseDto>>> GetAppApplicationsAsync();
        Task<ApiResponse<AppApplicationResponseDto>> GetAppApplicationAsync(Guid id);
        Task<ApiResponse<byte[]>> GenerateApplicationSlipAsync(Guid applicationId);
    }
}
