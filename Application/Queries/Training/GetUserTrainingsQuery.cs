using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Training
{
    public record GetUserTrainingsQuery(int PageSize, int PageNumber) : IRequest<PagedResponse<IEnumerable<TrainingResponseDto>>>;

    public class GetUserTrainingsQueryHandler(ITrainingService trainingService)
        : IRequestHandler<GetUserTrainingsQuery, PagedResponse<IEnumerable<TrainingResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<TrainingResponseDto>>> Handle(GetUserTrainingsQuery request, CancellationToken cancellationToken)
        {
            return await trainingService.GetUserTainingsAsync(request.PageSize, request.PageNumber);
        }
    }
}
