using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Training
{
    public record DeleteTrainingCommand(Guid TrainingId) : IRequest<ApiResponse<TrainingResponseDto>>;

    public class DeleteTrainingCommandHandler(ITrainingService trainingService)
        : IRequestHandler<DeleteTrainingCommand, ApiResponse<TrainingResponseDto>>
    {
        public async Task<ApiResponse<TrainingResponseDto>> Handle(DeleteTrainingCommand request, CancellationToken cancellationToken)
        {
            return await trainingService.DeleteAsync(request.TrainingId);
        }
    }
}
