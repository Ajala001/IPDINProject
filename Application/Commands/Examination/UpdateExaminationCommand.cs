using App.Core.DTOs.Requests.UpdateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Examination
{
    public record UpdateExaminationCommand(Guid ExaminatioId, UpdateExaminationRequestDto UpdateRequest) 
        : IRequest<ApiResponse<ExaminationResponseDto>>;

    public class UpdateExaminationCommandHandler(IExaminationService examinationService)
        : IRequestHandler<UpdateExaminationCommand, ApiResponse<ExaminationResponseDto>>
    {
        public async Task<ApiResponse<ExaminationResponseDto>> Handle(UpdateExaminationCommand request, CancellationToken cancellationToken)
        {
            return await examinationService.UpdateAsync(request.ExaminatioId, request.UpdateRequest);
        }
    }
}
