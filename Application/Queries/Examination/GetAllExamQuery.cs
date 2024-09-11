using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Examination
{
    public record GetAllExamQuery() : IRequest<ApiResponse<IEnumerable<ExaminationResponseDto>>>;

    public class GetAllExamQueryHandler(IExaminationService examinationService) 
        : IRequestHandler<GetAllExamQuery, ApiResponse<IEnumerable<ExaminationResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<ExaminationResponseDto>>> Handle(GetAllExamQuery request, CancellationToken cancellationToken)
        {
            return await examinationService.GetExaminationsAsync();
        }
    }
}
