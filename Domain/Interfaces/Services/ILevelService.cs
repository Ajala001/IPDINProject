using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;

namespace App.Core.Interfaces.Services
{
    public interface ILevelService
    {
        Task<ApiResponse<LevelRequestResponseDto>> CreateAsync(CreateLevelRequestDto request);
        Task<ApiResponse<LevelRequestResponseDto>> UpdateAsync(Guid id, UpdateLevelRequestDto request);
        Task<ApiResponse<LevelRequestResponseDto>> DeleteAsync(Guid id);
        Task<ApiResponse<IEnumerable<LevelRequestResponseDto>>> GetLevelsAsync();
        Task<ApiResponse<LevelRequestResponseDto>> GetLevelAsync(Guid id);
    }
}
