using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Training
{
    public record GetAllTrainingQuery(int PageSize, int PageNumber) : IRequest<PagedResponse<IEnumerable<TrainingResponseDto>>>;

    public class GetAllTrainingQueryHandler(ITrainingService trainingService)
        : IRequestHandler<GetAllTrainingQuery, PagedResponse<IEnumerable<TrainingResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<TrainingResponseDto>>> Handle(GetAllTrainingQuery request, CancellationToken cancellationToken)
        {
            return await trainingService.GetTainingsAsync(request.PageSize, request.PageNumber);
        }
    }
}
