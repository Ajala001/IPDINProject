using App.Core.DTOs.Responses;
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
    public record DeleteAcademicQualificationCommand(Guid QualificationId)
        : IRequest<ApiResponse<AcademicQualificationResponseDto>>;

    public class DeleteAcademicQualificationCommandHandler(IAcademicQualificationService academicQualificationService)
        : IRequestHandler<DeleteAcademicQualificationCommand, ApiResponse<AcademicQualificationResponseDto>>
    {
        public async Task<ApiResponse<AcademicQualificationResponseDto>> Handle(DeleteAcademicQualificationCommand request, CancellationToken cancellationToken)
        {
            return await academicQualificationService.DeleteAsync(request.QualificationId);
        }
    }
}
