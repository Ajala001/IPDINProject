using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Training
{
    public record GetAllTrainingQuery() : IRequest<ApiResponse<IEnumerable<TrainingResponseDto>>>;

    public class GetAllTrainingQueryHandler(ITrainingService trainingService)
        : IRequestHandler<GetAllTrainingQuery, ApiResponse<IEnumerable<TrainingResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<TrainingResponseDto>>> Handle(GetAllTrainingQuery request, CancellationToken cancellationToken)
        {
            return await trainingService.GetTainingsAsync();
        }
    }
}
