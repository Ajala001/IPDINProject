using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Training
{
    public record UpdateTrainingCommand(Guid TrainingId, UpdateTrainingRequestDto UpdateRequest) 
        : IRequest<ApiResponse<TrainingResponseDto>>;

    public class UpdateTrainingCommandHandler(ITrainingService trainingService)
        : IRequestHandler<UpdateTrainingCommand, ApiResponse<TrainingResponseDto>>
    {
        public async Task<ApiResponse<TrainingResponseDto>> Handle(UpdateTrainingCommand request, CancellationToken cancellationToken)
        {
            return await trainingService.UpdateAsync(request.TrainingId, request.UpdateRequest);
        }
    }
}
