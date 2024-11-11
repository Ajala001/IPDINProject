using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Examination
{
    public record GetUserExamsQuery(int PageSize, int PageNumber) : IRequest<PagedResponse<IEnumerable<ExaminationResponseDto>>>;

    public class GetUserExamsQueryHandler(IExaminationService examinationService)
        : IRequestHandler<GetAllExamQuery, PagedResponse<IEnumerable<ExaminationResponseDto>>>
    {
        public async Task<PagedResponse<IEnumerable<ExaminationResponseDto>>> Handle(GetAllExamQuery request, CancellationToken cancellationToken)
        {
            return await examinationService.GetUserExaminationsAsync(request.PageSize, request.PageNumber);
        }
    }
}
