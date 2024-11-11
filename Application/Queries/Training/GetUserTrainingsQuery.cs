using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Training
{
    public record GetUserTrainingsQuery(int PageSize, int PageNumber) : IRequest<PagedResponse<IEnumerable<TrainingResponseDto>>>;

    public class GetUserTrainingsQueryHandler(ITrainingService trainingService)
        : IRequestHandler<GetAllTrainingQuery, PagedResponse<IEnumerable<TrainingResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<TrainingResponseDto>>> Handle(GetAllTrainingQuery request, CancellationToken cancellationToken)
        {
            return await trainingService.GetUserTainingsAsync(request.PageSize, request.PageNumber);
        }
    }
}
