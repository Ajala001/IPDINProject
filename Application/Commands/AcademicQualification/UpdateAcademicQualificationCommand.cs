using App.Core.DTOs.Requests.UpdateRequestDtos;
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

namespace App.Application.Commands.AcademicQualification
{
    public record UpdateAcademicQualificationCommand(Guid QualificationId, UpdateAcademicQualificationRequestDto UpdateRequest)
        : IRequest<ApiResponse<AcademicQualificationResponseDto>>;

    public class UpdateAcademicQualificationCommandHandler(IAcademicQualificationService academicQualificationService)
        : IRequestHandler<UpdateAcademicQualificationCommand, ApiResponse<AcademicQualificationResponseDto>>
    {
        public async Task<ApiResponse<AcademicQualificationResponseDto>> Handle(UpdateAcademicQualificationCommand request, CancellationToken cancellationToken)
        {
            return await academicQualificationService.UpdateAsync(request.QualificationId, request.UpdateRequest);
        }
    }
}
