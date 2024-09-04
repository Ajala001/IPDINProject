using App.Application.Predicates;
using App.Core.DTOs.Responses;
using App.Core.Entities;
using App.Core.Interfaces.Repositories;
using App.Core.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
