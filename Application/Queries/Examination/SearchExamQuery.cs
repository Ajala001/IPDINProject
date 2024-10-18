using App.Core.DTOs.Requests.SearchRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Examination
{
    public record SearchExamQuery(SearchQueryRequestDto SearchRequestDto) 
        : IRequest<PagedResponse<IEnumerable<ExaminationResponseDto>>>;

    public class SearchExamQueryHandler(IExaminationService examinationService)
        : IRequestHandler<SearchExamQuery, PagedResponse<IEnumerable<ExaminationResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<ExaminationResponseDto>>> Handle(SearchExamQuery request, CancellationToken cancellationToken)
        {
            return await examinationService.SearchExaminationAsync(request.SearchRequestDto);
        }
    }
}
