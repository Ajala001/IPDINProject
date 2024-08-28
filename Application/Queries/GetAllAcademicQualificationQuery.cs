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

namespace App.Application.Queries
{
    public record GetAllAcademicQualificationQuery() : IRequest<ApiResponse<IEnumerable<AcademicQualificationResponseDto>>>;

    public class GetAllAcademicQualificationQueryHandler(IAcademicQualificationService academicQualificationService)
        : IRequestHandler<GetAllAcademicQualificationQuery, ApiResponse<IEnumerable<AcademicQualificationResponseDto>>>
    {
        public async Task<ApiResponse<IEnumerable<AcademicQualificationResponseDto>>> Handle(GetAllAcademicQualificationQuery request, CancellationToken cancellationToken)
        {
            return await academicQualificationService.GetQualificationsAsync();
        }
    }
}
