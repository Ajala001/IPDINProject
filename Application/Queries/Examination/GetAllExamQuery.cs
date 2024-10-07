using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Examination
{
    public record GetAllExamQuery(int PageSize, int PageNumber) : IRequest<PagedResponse<IEnumerable<ExaminationResponseDto>>>;

    public class GetAllExamQueryHandler(IExaminationService examinationService) 
        : IRequestHandler<GetAllExamQuery, PagedResponse<IEnumerable<ExaminationResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<ExaminationResponseDto>>> Handle(GetAllExamQuery request, CancellationToken cancellationToken)
        {
            return await examinationService.GetExaminationsAsync(request.PageSize, request.PageNumber);
        }
    }
}
