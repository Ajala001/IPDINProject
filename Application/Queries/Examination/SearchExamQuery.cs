using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Examination
{
    public record SearchExamQuery(ExaminationSearchRequestDto SearchRequestDto) 
        : IRequest<ApiResponse<IEnumerable<ExaminationResponseDto>>>;

    public class SearchExamQueryHandler(IExaminationService examinationService)
        : IRequestHandler<SearchExamQuery, ApiResponse<IEnumerable<ExaminationResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<ExaminationResponseDto>>> Handle(SearchExamQuery request, CancellationToken cancellationToken)
        {
            return await examinationService.SearchExaminationAsync(request.SearchRequestDto);
        }
    }
}
