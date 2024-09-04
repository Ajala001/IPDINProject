using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.AcademicQualification
{
    public record AddAcademicQualificationCommand(CreateAcademicQualificationRequestDto Qualification)
        : IRequest<ApiResponse<AcademicQualificationResponseDto>>;

    public class AddAcademicQualificationCommandHandler(IAcademicQualificationService academicQualificationService)
        : IRequestHandler<AddAcademicQualificationCommand, ApiResponse<AcademicQualificationResponseDto>>
    {
        public async Task<ApiResponse<AcademicQualificationResponseDto>> Handle(AddAcademicQualificationCommand request, CancellationToken cancellationToken)
        {
            return await academicQualificationService.CreateAsync(request.Qualification);
        }
    }
}
