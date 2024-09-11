using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Examination
{
    public record DeleteExaminationCommand(Guid ExaminationId) : IRequest<ApiResponse<ExaminationResponseDto>>;

    public class DeleteExaminationCommandHandler(IExaminationService examinationService) 
        : IRequestHandler<DeleteExaminationCommand, ApiResponse<ExaminationResponseDto>>
    {
        public async Task<ApiResponse<ExaminationResponseDto>> Handle(DeleteExaminationCommand request, CancellationToken cancellationToken)
        {
            return await examinationService.DeleteAsync(request.ExaminationId);
        }
    }

}
