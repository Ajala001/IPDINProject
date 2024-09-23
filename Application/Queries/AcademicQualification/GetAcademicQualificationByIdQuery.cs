using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Queries.AcademicQualification
{
    public record GetAcademicQualificationByIdQuery(Guid qualificationId) : IRequest<ApiResponse<AcademicQualificationResponseDto>>;

    public class GetAcademicQualificationByIdQueryHandler(IAcademicQualificationService academicQualificationService)
        : IRequestHandler<GetAcademicQualificationByIdQuery, ApiResponse<AcademicQualificationResponseDto>>
    {
        public async Task<ApiResponse<AcademicQualificationResponseDto>> Handle(GetAcademicQualificationByIdQuery request, CancellationToken cancellationToken)
        {
            return await academicQualificationService.GetQualificationAsync(request.qualificationId);
        }
    }
}
