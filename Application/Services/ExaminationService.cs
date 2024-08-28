using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;

namespace App.Application.Services
{
    public class ExaminationService : IExaminationService
    {
        public Task<ApiResponse<ExaminationResponseDto>> CreateAsync(CreateExaminationRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ExaminationResponseDto>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ExaminationResponseDto>> GetExaminationAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<ExaminationResponseDto>>> GetExaminationsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ExaminationResponseDto>> UpdateAsync(Guid id, UpdateExaminationRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
