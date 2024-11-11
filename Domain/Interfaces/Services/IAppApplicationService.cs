using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;

namespace App.Core.Interfaces.Services
{
    public interface IAppApplicationService
    {
        Task<ApiResponse<AppApplicationResponseDto>> CreateAsync(CreateAppApplicationRequestDto request);
        Task<ApiResponse<AppApplicationResponseDto>> UpdateAsync(Guid id, UpdateAppApplicationRequestDto request);
        Task<ApiResponse<AppApplicationResponseDto>> DeleteAsync(Guid id);
        Task<PagedResponse<IEnumerable<AppApplicationResponseDto>>> GetAppApplicationsAsync(int pageSize, int pageNumber);
        Task<PagedResponse<IEnumerable<AppApplicationResponseDto>>> GetUserAppApplicationsAsync(int pageSize, int pageNumber);
        Task<ApiResponse<AppApplicationResponseDto>> GetAppApplicationAsync(Guid id);
        Task<ApiResponse<AppApplicationResponseDto>> AcceptApplicationAsync(Guid id);
        Task<PagedResponse<IEnumerable<AppApplicationResponseDto>>> SearchApplicationAsync(SearchQueryRequestDto request);
        Task<ApiResponse<string>> RejectApplicationAsync(Guid id, RejectionApplicationRequestDto request);
        Task<ApiResponse<byte[]>> GenerateApplicationSlipAsync(Guid applicationId);
    }
}
