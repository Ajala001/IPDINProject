using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;

namespace App.Application.Services
{
    public class ResultService : IResultService
    {
        public Task<ApiResponse<ResultResponseDto>> CreateAsync(CreateResultRequestDto request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ResultResponseDto>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ResultResponseDto>> GetResultAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<IEnumerable<ResultResponseDto>>> GetResultsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<ResultResponseDto>> UpdateAsync(Guid id, UpdateResultRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
