using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Training
{
    public record SearchTrainingQuery(SearchQueryRequestDto SearchRequestDto) 
        : IRequest<PagedResponse<IEnumerable<TrainingResponseDto>>>;

    public class SearchTrainingQueryHandler(ITrainingService trainingService)
        : IRequestHandler<SearchTrainingQuery, PagedResponse<IEnumerable<TrainingResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<TrainingResponseDto>>> Handle(SearchTrainingQuery request, CancellationToken cancellationToken)
        {
            return await trainingService.SearchTrainingAsync(request.SearchRequestDto);
        }
    }
}
