using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;

namespace App.Core.Interfaces.Services
{
    public interface ITrainingService
    {
        Task<ApiResponse<TrainingResponseDto>> CreateAsync(CreateTrainingRequestDto request);
        Task<ApiResponse<TrainingResponseDto>> UpdateAsync(Guid id, UpdateTrainingRequestDto request);
        Task<ApiResponse<TrainingResponseDto>> DeleteAsync(Guid id);
        Task<ApiResponse<IEnumerable<TrainingResponseDto>>> GetTainingsAsync();
        Task<ApiResponse<TrainingResponseDto>> GetTrainingAsync(Guid id);
        Task<ApiResponse<IEnumerable<TrainingResponseDto>>> SearchTrainingAsync(TrainingSearchRequestDto request);
    }
}
