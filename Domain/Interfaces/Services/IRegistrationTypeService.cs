using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;

namespace App.Core.Interfaces.Services
{
    public interface IRegistrationTypeService
    {
        Task<ApiResponse<RegistrationTypeResponseDto>> CreateAsync(CreateRegistrationTypeRequestDto request);
        Task<ApiResponse<RegistrationTypeResponseDto>> UpdateAsync(Guid id, UpdateRegistrationTypeRequestDto request);
        Task<ApiResponse<RegistrationTypeResponseDto>> DeleteAsync(Guid id);
        Task<ApiResponse<IEnumerable<RegistrationTypeResponseDto>>> GetRegistrationTypesAsync();
        Task<ApiResponse<RegistrationTypeResponseDto>> GetGetRegistrationTypeAsync(Guid id);
    }
}
