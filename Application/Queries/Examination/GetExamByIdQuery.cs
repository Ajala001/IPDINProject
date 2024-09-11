using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.Examination
{
    public record GetExamByIdQuery(Guid ExamId) : IRequest<ApiResponse<ExaminationResponseDto>>;

    public class GetExamByIdQueryHandler(IExaminationService examinationService)
        : IRequestHandler<GetExamByIdQuery, ApiResponse<ExaminationResponseDto>>
{
        public async Task<ApiResponse<ExaminationResponseDto>> Handle(GetExamByIdQuery request, CancellationToken cancellationToken)
        {
            return await examinationService.GetExaminationAsync(request.ExamId);
        }
    }
}
