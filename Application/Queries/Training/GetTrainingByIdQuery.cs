using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Training
{
    public record GetTrainingByIdQuery(Guid TrainingId) : IRequest<ApiResponse<TrainingResponseDto>>;

    public class GetTrainingByIdQueryHandler(ITrainingService trainingService)
        : IRequestHandler<GetTrainingByIdQuery, ApiResponse<TrainingResponseDto>>
{
        public async Task<ApiResponse<TrainingResponseDto>> Handle(GetTrainingByIdQuery request, CancellationToken cancellationToken)
        {
            return await trainingService.GetTrainingAsync(request.TrainingId);
        }
    }
}
