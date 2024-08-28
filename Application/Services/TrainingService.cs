using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;

namespace App.Application.Services
{
    public class TrainingService : ITrainingService
    {
        public Task<ApiResponse<TrainingResponseDto>> CreateAsync(CreateTrainingRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<TrainingResponseDto>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<TrainingResponseDto>>> GetTainingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<TrainingResponseDto>> GetTrainingAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<TrainingResponseDto>>> SearchTrainingAsync(TrainingSearchRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<TrainingResponseDto>> UpdateAsync(Guid id, UpdateTrainingRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
