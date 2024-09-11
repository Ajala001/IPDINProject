using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Training
{
    public record AddTrainingCommand(CreateTrainingRequestDto Training) : IRequest<ApiResponse<TrainingResponseDto>>;

    public class AddTrainingCommandHandler(ITrainingService trainingService)
        : IRequestHandler<AddTrainingCommand, ApiResponse<TrainingResponseDto>>
    {
        public async Task<ApiResponse<TrainingResponseDto>> Handle(AddTrainingCommand request, CancellationToken cancellationToken)
        {
            return await trainingService.CreateAsync(request.Training);
        }
    }
}
