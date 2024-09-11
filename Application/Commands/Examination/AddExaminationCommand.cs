using App.Core.DTOs.Requests.CreateRequestDtos;
using App.Core.DTOs.Responses;
using App.Core.Interfaces.Services;
using MediatR;

namespace App.Application.Commands.Examination
{
    public record AddExaminationCommand(CreateExaminationRequestDto Examination) 
        : IRequest<ApiResponse<ExaminationResponseDto>>;

    public class AddExaminationCommandHandler(IExaminationService examinationService)
        : IRequestHandler<AddExaminationCommand, ApiResponse<ExaminationResponseDto>>
    {
        public async Task<ApiResponse<ExaminationResponseDto>> Handle(AddExaminationCommand request, CancellationToken cancellationToken)
        {
            return await examinationService.CreateAsync(request.Examination);
        }
    }
}
