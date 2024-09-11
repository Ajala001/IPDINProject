using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Training
{
    public record SearchTrainingQuery(TrainingSearchRequestDto SearchRequestDto) 
        : IRequest<ApiResponse<IEnumerable<TrainingResponseDto>>>;

    public class SearchTrainingQueryHandler(ITrainingService trainingService)
        : IRequestHandler<SearchTrainingQuery, ApiResponse<IEnumerable<TrainingResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<TrainingResponseDto>>> Handle(SearchTrainingQuery request, CancellationToken cancellationToken)
        {
            return await trainingService.SearchTrainingAsync(request.SearchRequestDto);
        }
    }
}
